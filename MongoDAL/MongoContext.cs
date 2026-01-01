using MongoDB.Driver;
using System.Reflection;
namespace MongoDAL;

public class MongoContext : IMongoContext
{
  private IMongoClient client;
  //todo: move connection string to settings file
  //= "mongodb+srv://wcdeich4_db_user:8L#v3M#ng#DB@cluster0.x01hubz.mongodb.net/?appName=Cluster0";

  public string connectionUri { get; set; }


  public MongoContext() { }

  public MongoContext(string uri)
  {
    this.connectionUri = uri;
  }

  /// <summary>
  /// Insert multiple documents into collection
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="databaseName"></param>
  /// <param name="collectionName"></param>
  /// <param name="entityList"></param>
  public void Insert<T>(string databaseName, string collectionName, IEnumerable<T> entityList)
  {
    var settings = MongoClientSettings.FromConnectionString(connectionUri);
    settings.ServerApi = new ServerApi(ServerApiVersion.V1);
    var client = new MongoClient(settings);
    var database = client.GetDatabase(databaseName); //creates database if it does not exist
    var collection = database.GetCollection<T>(collectionName);
    collection.InsertMany(entityList);
  }

  /// <summary>
  /// Find documents matching filter
  /// </summary>
  /// <typeparam name="T">document type</typeparam>
  /// <param name="databaseName"></param>
  /// <param name="collectionName"></param>
  /// <param name="filter"></param>
  /// <returns></returns>
  public List<T> Find<T>(string databaseName, string collectionName, FilterDefinition<T> filter)
  {
    var settings = MongoClientSettings.FromConnectionString(connectionUri);
    settings.ServerApi = new ServerApi(ServerApiVersion.V1);
    this.client = new MongoClient(settings);
    var database = this.client.GetDatabase(databaseName); //creates database if it does not exist
    var collection = database.GetCollection<T>(collectionName);
    return collection.Find(filter).ToList();
  }

  /// <summary>
  /// Delete documents matching filter
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="databaseName"></param>
  /// <param name="collectionName"></param>
  /// <param name="filter"></param>
  /// <returns></returns>
  public DeleteResult Delete<T>(string databaseName, string collectionName, FilterDefinition<T> filter)
  {
    var settings = MongoClientSettings.FromConnectionString(connectionUri);
    settings.ServerApi = new ServerApi(ServerApiVersion.V1);
    this.client = new MongoClient(settings);
    var database = this.client.GetDatabase(databaseName); //creates database if it does not exist
    var collection = database.GetCollection<T>(collectionName);
    return collection.DeleteMany(filter);
  }


  //TODO: UpdateMany, ReplaceMany

}