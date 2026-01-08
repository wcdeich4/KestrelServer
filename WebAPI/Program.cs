using System;
using System.IO;
using System.Security.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDAL;
using BasicObjects;

//create WebApplicationBuilder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Microsoft's AddKeyedSingleton method takes a non-singleton class and treats it as a singleton for each key
builder.Services.AddKeyedSingleton<IFlexibleDictionary<string, FileRecord>, FlexibleDictionary<string, FileRecord>>("FileDictionary"); 

// specify OpenApi3_1
builder.Services.AddOpenApi(options =>  //works despite red in VSCode & Rider
{
  // Specify the OpenAPI version to use
  options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1; //works despite red in VSCode & Rider
});

builder.Services.AddRouting(r => r.SuppressCheckForUnhandledSecurityMetadata = true); //hack to avoid "InvalidOperationException: No authenticationScheme was specified, and there was no DefaultChallengeScheme found." when using CORS with minimal APIs
builder.Services.AddCors(options => //add CORS policy to allow any header and method
{
  options.AddDefaultPolicy(builder =>
  {
    builder.AllowAnyOrigin();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
  });
});

builder.WebHost.ConfigureKestrel(serverOptions => //set TLS 1.3
{
  serverOptions.ConfigureHttpsDefaults(listenOptions =>
  {
    listenOptions.SslProtocols = SslProtocols.Tls13;
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) //based on ASPNETCORE_ENVIRONMENT in launchSettings.json
{
  app.MapOpenApi("/interface/openapi.json");  //works despite red in VSCode & Rider
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseDefaultFiles(); // Rewrites requests to directories to point to default files
StaticFileOptions staticFileOptions = new StaticFileOptions();
staticFileOptions.ServeUnknownFileTypes = true;
app.UseStaticFiles(staticFileOptions); // Serves the static files (including those specified by UseDefaultFiles)
app.Use((context, next) => //hack to avoid "InvalidOperationException: No authenticationScheme was specified, and there was no DefaultChallengeScheme found." with older .NET SDKs
{
  context.Items["__CorsMiddlewareInvoked"] = true;
  return next();
});
app.UseCors();

/// <summary>
/// Define a minimal API endpoint to list of filenames to get a list of filenames in the FilesFolder.
/// <param name="pattern">"/localfilelist" is the API path</param>
/// <param name="handler">([FromKeyedServices("FileDictionary")] IFlexibleDictionary<string, FileRecord> fileCache, IConfiguration config) Lambda function to handle the request</param>
/// </summary>
app.MapGet("/localfilelist", ([FromKeyedServices("FileDictionary")] IFlexibleDictionary<string, FileRecord> fileCache, IConfiguration config) =>
{
  //app.Environment.WebRootPath would gives you the current wwwroot folder path, regardless of how/if it was set above, but using wwwroot is not safe
  string[] fileNameArray = [];
  string? folderPath = config["FilesFolder"];
  if(!string.IsNullOrWhiteSpace(folderPath))
  {
    readFileNames(folderPath, fileCache);
    string[] files = [.. fileCache.Keys];
    for(int i=0; i<files.Length; i++)
    {
      fileCache.AddOrUpdate(files[i], new FileRecord(Path.GetFileName(files[i])));
      files[i] = Path.GetFileName(files[i]);
    }
    fileNameArray = files;
  }
  return Results.Ok(fileNameArray);
})
.WithName("GetFileList");

/// <summary>
/// Define a minimal API endpoint to get file contents.
/// <param name="pattern">"/localfilecontent/{fileName}" is the API path with fileName API parameter</param>
/// <param name="handler">([FromRoute] string fileName, [FromKeyedServices("FileDictionary")] IFlexibleDictionary<string, FileRecord> fileCache, IConfiguration config) Lambda function to handle the request</param>
/// </summary>
app.MapGet("/localfilecontent/{fileName}", ([FromRoute] string fileName, [FromKeyedServices("FileDictionary")] IFlexibleDictionary<string, FileRecord> fileCache, IConfiguration config) =>
{
  //check if the file is in the cache, look for it
  if(!fileCache.ContainsKeyEndingWith(fileName))
  {
    //app.Environment.WebRootPath would gives you the current wwwroot folder path, regardless of how/if it was set above, but using wwwroot is not safe
    string? folderPath = config["FilesFolder"];
    if (!string.IsNullOrWhiteSpace(folderPath))
    {
      readFileNames(folderPath, fileCache);
    }
  }
  //if the file is still not found, it does not exist
  string fullPath = fileCache.GetKeyEndingWith(fileName);
  if(string.IsNullOrWhiteSpace(fullPath))
  {
    return Results.NotFound($"File '{fileName}' not found.");
  }
  else
  {
    //if found in the cache, return the contents
    var record = fileCache.GetValueByKeyEndingWith(fileName);
    if(record.Contents == null)
    {
      //load the file contents
      record.Contents = File.ReadAllText(fullPath);
      //update the record in the cache
      fileCache.AddOrUpdate(fullPath, record);
    }
    return Results.Ok(record.Contents);
  }
})
.WithName("GetFileContent");

/// <summary>
/// Define a minimal API endpoint to get file contents from MongoDB.
/// <param name="pattern">"/GetMongoFileContent/{fileName}" is the API path</param>
/// <param name="handler">([FromRoute] string fileName, IConfiguration config) Lambda function to handle the request</param>
/// </summary>
app.MapGet("/GetMongoFileContent/{fileName}", ([FromRoute] string fileName, IConfiguration config) =>
{
  string? MongoConnectionUri = config["ConnectionStrings:MongoConnectionUri"];
  if(string.IsNullOrWhiteSpace(MongoConnectionUri))
  {
    string errorMessage = "config[\"ConnectionStrings:MongoConnectionUri\"] was null/empty/whitespace";
    Console.WriteLine(errorMessage);
    return Results.NotFound(errorMessage);
  }
  else
  {
    var mongoContext = new MongoContext(MongoConnectionUri);
    var filter = Builders<FileRecord>.Filter.Eq("Name", fileName);
    string? MongoDBAtlasDatabaseName = config["MongoDBAtlasDatabaseName"];
    string? MongoDBAtlasCollectionName = config["MongoDBAtlasCollectionName"];
    if(string.IsNullOrWhiteSpace(MongoDBAtlasDatabaseName) || string.IsNullOrWhiteSpace(MongoDBAtlasCollectionName))
    {
      string errorMessage = "config[\"MongoDBAtlasDatabaseName\"] or config[\"MongoDBAtlasCollectionName\"] was null/empty/whitespace";
      Console.WriteLine(errorMessage);
      return Results.InternalServerError(errorMessage);
    }
    else
    {
      FileRecord findResult = mongoContext.FindOne<FileRecord>(MongoDBAtlasDatabaseName, MongoDBAtlasCollectionName, filter);
      if (findResult == null || string.IsNullOrWhiteSpace(findResult.Contents))
      {
        return Results.NotFound($"File '{fileName}' not found in MongoDB.");
      }
      else
      {
        return Results.Ok(findResult.Contents);
      }
    }
  }
})
.WithName("GetMongoFileContent"); //does not have to be exact match, but must be unique

/// <summary>
/// Define a minimal API endpoint to delete from MongoDB.
/// <param name="pattern">"/GetMongoFileContent/{fileName}" is the API path</param>
/// <param name="handler">([FromRoute] string fileName, IConfiguration config) Lambda function to handle the request</param>
/// </summary>
app.MapGet("/DeleteMongoFile/{fileName}", ([FromRoute] string fileName, IConfiguration config) =>
{
  string? MongoConnectionUri = config["ConnectionStrings:MongoConnectionUri"];
  if(string.IsNullOrWhiteSpace(MongoConnectionUri))
  {
    string errorMessage = "config[\"ConnectionStrings:MongoConnectionUri\"] was null/empty/whitespace";
    Console.WriteLine(errorMessage);
    return Results.NotFound(errorMessage);
  }
  else
  {
    var mongoContext = new MongoContext(MongoConnectionUri);
    var filter = Builders<FileRecord>.Filter.Eq("Name", fileName);
    string? MongoDBAtlasDatabaseName = config["MongoDBAtlasDatabaseName"];
    string? MongoDBAtlasCollectionName = config["MongoDBAtlasCollectionName"];
    if(string.IsNullOrWhiteSpace(MongoDBAtlasDatabaseName) || string.IsNullOrWhiteSpace(MongoDBAtlasCollectionName))
    {
      string errorMessage = "config[\"MongoDBAtlasDatabaseName\"] or config[\"MongoDBAtlasCollectionName\"] was null/empty/whitespace";
      Console.WriteLine(errorMessage);
      return Results.InternalServerError(errorMessage);
    }
    else
    {
      DeleteResult deleteResult = mongoContext.Delete<FileRecord>(MongoDBAtlasDatabaseName, MongoDBAtlasCollectionName, filter);
      if (deleteResult == null)
      {
        return Results.NotFound($"File '{fileName}' not found in MongoDB.");
      }
      else
      {
        return Results.Ok();
      }
    }
  }
})
.WithName("DeleteMongoFile"); //does not have to be exact match, but must be unique








































/// <summary>
/// Define a minimal API endpoint to Post file contents.
/// <param name="pattern">"/PostMongoFileContent" </param>
/// <param name="handler">(FileRecord fileDataToInsert, IConfiguration config) Lambda function to handle the request</param>
/// </summary>
app.MapPost("/PostMongoFileContent", (FileRecord fileDataToInsert, IConfiguration config) => 
{
  string? MongoConnectionUri = config["ConnectionStrings:MongoConnectionUri"];
  if(string.IsNullOrWhiteSpace(MongoConnectionUri))
  {
    string errorMessage = "config[\"ConnectionStrings:MongoConnectionUri\"] was null/empty/whitespace";
    Console.WriteLine(errorMessage);
    return Results.NotFound(errorMessage);
  }
  else
  {
    var mongoContext = new MongoContext(MongoConnectionUri);
    try
    {
      string? MongoDBAtlasDatabaseName = config["MongoDBAtlasDatabaseName"];
      string? MongoDBAtlasCollectionName = config["MongoDBAtlasCollectionName"];
      if(string.IsNullOrWhiteSpace(MongoDBAtlasDatabaseName) || string.IsNullOrWhiteSpace(MongoDBAtlasCollectionName))
      {
        string errorMessage = "config[\"MongoDBAtlasDatabaseName\"] or config[\"MongoDBAtlasCollectionName\"] was null/empty/whitespace";
        Console.WriteLine(errorMessage);
        return Results.InternalServerError(errorMessage);
      }
      else
      {
        var filter = Builders<FileRecord>.Filter.Eq("Name", fileDataToInsert.Name);
        mongoContext.Upsert(MongoDBAtlasDatabaseName, MongoDBAtlasCollectionName, filter, fileDataToInsert);
        return Results.Created("record inserted", fileDataToInsert);
      }
    }
    catch(Exception ex)
    {
      Console.WriteLine("PostMongoFileContent error", ex.ToString());
      return Results.InternalServerError(ex);
    }
  }
})
.WithName("PostMongoFileContent");


app.Run();



//helper methods
//------------------------------------------------------------------------------ 
/// <summary>
/// Read all filenames from the specified folder and add them to the fileCache dictionary.
/// <param name="fileFolder">The folder to read files from.</param>
/// <param name="fileCache">The dictionary to store the file records.</param>
/// </summary>
void readFileNames(string fileFolder, IFlexibleDictionary<string, FileRecord> fileCache)
{
  string[] files = Directory.GetFiles(fileFolder, "*.*", SearchOption.TopDirectoryOnly);
  for(int i=0; i<files.Length; i++)
  {
    fileCache.AddOrUpdate(files[i], new FileRecord(Path.GetFileName(files[i])));
  }
}