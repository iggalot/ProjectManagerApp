using MongoDB.Driver;

namespace ProjectManagerAppLibrary.DataAccess;

public interface IDbConnection
{
   IMongoCollection<CategoryModel> CategoryCollection { get; }
   string CategoryCollectionName { get; }
   MongoClient Client { get; }
   string DbName { get; }
   IMongoCollection<ProjectInfoModel> ProjectInfoCollection { get; }
   string ProjectInfoCollectionName { get; }
   IMongoCollection<StatusModel> StatusCollection { get; }
   string StatusCollectionName { get; }
   IMongoCollection<UserModel> UserCollection { get; }
   string UserCollectionName { get; }
}