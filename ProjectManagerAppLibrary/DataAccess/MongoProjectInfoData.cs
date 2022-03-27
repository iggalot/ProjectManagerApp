using Microsoft.Extensions.Caching.Memory;

namespace ProjectManagerAppLibrary.DataAccess;

public class MongoProjectInfoData : IProjectInfoData
{
   private readonly IDbConnection _db;
   private readonly IUserData _userData;
   private readonly IMemoryCache _cache;
   private readonly IMongoCollection<ProjectInfoModel> _projects;
   private const string CacheName = "ProjectInfoData";

   public MongoProjectInfoData(IDbConnection db, IUserData userData, IMemoryCache cache)
   {
      _db = db;
      _userData = userData;
      _cache = cache;
      _projects = db.ProjectInfoCollection;
   }

   public async Task<List<ProjectInfoModel>> GetAllProjectInfos()
   {
      var output = _cache.Get<List<ProjectInfoModel>>(CacheName);
      if (output is null)
      {
         var results = await _projects.FindAsync(p => p.Archived == false);
         output = results.ToList();

         _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
      }

      return output;
   }

   public async Task<List<ProjectInfoModel>> GetUsersProjectInfos(string userId)
   {
      var output = _cache.Get<List<ProjectInfoModel>>(userId);
      if (output is null)
      {
         var results = await _projects.FindAsync(s => s.Author.Id == userId);
         output = results.ToList();

         _cache.Set(userId, output, TimeSpan.FromMinutes(1));
      }

      return output;
   }
   public async Task<List<ProjectInfoModel>> GetAllApprovedProjectInfos()
   {
      var output = await GetAllProjectInfos();
      return output.Where(x => x.ApprovedForRelease).ToList();
   }

   public async Task<ProjectInfoModel> GetProjectInfo(string id)
   {
      var results = await _projects.FindAsync(p => p.Id == id);
      return results.FirstOrDefault();
   }

   public async Task<List<ProjectInfoModel>> GetAllProjectsWaitingforApproval()
   {
      var output = await GetAllProjectInfos();
      return output.Where(x =>
         x.ApprovedForRelease == false
         && x.Rejected == false).ToList();
   }

   public async Task UpdateProjectInfo(ProjectInfoModel project)
   {
      await _projects.ReplaceOneAsync(p => p.Id == project.Id, project);
      _cache.Remove(CacheName);
   }

   public async Task UpvoteProjectInfo(string projectId, string userId)
   {
      var client = _db.Client;

      using var session = await client.StartSessionAsync();

      session.StartTransaction();

      try
      {
         var db = client.GetDatabase(_db.DbName);
         var projectinfosInTransaction = db.GetCollection<ProjectInfoModel>(_db.ProjectInfoCollectionName);
         var project = (await projectinfosInTransaction.FindAsync(p => p.Id == projectId)).First();

         bool isUpvote = project.UserVotes.Add(userId);
         if (isUpvote == false)
         {
            project.UserVotes.Remove(userId);
         }

         await projectinfosInTransaction.ReplaceOneAsync(p => p.Id == projectId, project);

         var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
         var user = await _userData.GetUser(project.Author.Id);

         if (isUpvote)
         {
            user.VotedOnProjectInfos.Add(new BasicProjectInfoModel(project));
         }
         else
         {
            var projectinfoToRemove = user.VotedOnProjectInfos.Where(p => p.Id == projectId).First();
            user.VotedOnProjectInfos.Remove(new BasicProjectInfoModel(project));
         }
         await usersInTransaction.ReplaceOneAsync(u => u.Id == userId, user);

         await session.CommitTransactionAsync();

         _cache.Remove(CacheName);
      }
      catch (Exception ex)
      {
         await session.AbortTransactionAsync();
         throw;
      }
   }

   public async Task CreateProjectInfo(ProjectInfoModel project)
   {
      var client = _db.Client;

      using var session = await client.StartSessionAsync();

      session.StartTransaction();

      try
      {
         var db = client.GetDatabase(_db.DbName);
         var projectinfosInTransaction = db.GetCollection<ProjectInfoModel>(_db.ProjectInfoCollectionName);
         await projectinfosInTransaction.InsertOneAsync(project);

         var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
         var user = await _userData.GetUser(project.Author.Id);
         user.AuthoredProjects.Add(new BasicProjectInfoModel(project));
         await usersInTransaction.ReplaceOneAsync(u => u.Id == user.Id, user);

         await session.CommitTransactionAsync();

      }
      catch (Exception ex)
      {
         await session.AbortTransactionAsync();
         throw;
      }
   }
}
