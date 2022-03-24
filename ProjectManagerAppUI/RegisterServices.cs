
namespace ProjectManagerAppUI;

public static class RegisterServices
{
   public static void ConfigureServices(this WebApplicationBuilder builder)
   {
      // Add services to the container.
      builder.Services.AddRazorPages();
      builder.Services.AddServerSideBlazor();
      builder.Services.AddMemoryCache();

      builder.Services.AddSingleton<IDbConnection, DbConnection>();
      builder.Services.AddSingleton<ICategoryData, MongoCategoryData>();
      builder.Services.AddSingleton<IStatusData, MongoStatusData>();
      builder.Services.AddSingleton<IProjectInfoData, MongoProjectInfoData>();
      builder.Services.AddSingleton<IUserData, MongoUserData>();
   }
}
