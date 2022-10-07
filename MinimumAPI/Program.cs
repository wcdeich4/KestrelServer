using System.Text.Json;

//create web application builder with web root path for static files
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot"
});

//specify configuration file
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json",
                       optional: true,
                       reloadOnChange: true);
});

//enable CORS
const string CORSAllowAllPolicy = "_CORSAllowAllPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORSAllowAllPolicy,
                      builder =>
                      {
                          builder.WithOrigins("*");
                      });
});
builder.Services.AddRouting(r => r.SuppressCheckForUnhandledSecurityMetadata = true);

//build application variable
var app = builder.Build();

//tell app to actually use cors
app.UseCors();
app.Use((context, next) =>
{
    context.Items["__CorsMiddlewareInvoked"] = true;
    return next();
});

//tell web app variable to serve static files in the web root folder
app.UseDefaultFiles();
StaticFileOptions staticFileOptions = new StaticFileOptions();
staticFileOptions.ServeUnknownFileTypes = true;
app.UseStaticFiles(staticFileOptions);

//JSON serialization options
var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

//specify API path to get JSON data
app.MapGet("/testdata", () => Results.Json(new string[] {"one", "two", "three"}, options));

//run the web application
app.Run();