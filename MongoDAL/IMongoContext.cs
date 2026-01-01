using MongoDB.Driver;
using System.Reflection;
namespace MongoDAL;

public interface IMongoContext
{
  string connectionUri { get; set; }

  DeleteResult DeleteMany<T>(string databaseName, string collectionName, FilterDefinition<T> filter);
  List<T> Find<T>(string databaseName, string collectionName, FilterDefinition<T> filter);
  void Insert<T>(string databaseName, string collectionName, IEnumerable<T> entityList);
}