
namespace ProjectManagerAppLibrary.DataAccess;

public interface IProjectInfoData
{
   Task CreateProjectInfo(ProjectInfoModel project);
   Task<List<ProjectInfoModel>> GetAllApprovedProjectInfos();
   Task<List<ProjectInfoModel>> GetAllProjectInfos();
   Task<List<ProjectInfoModel>> GetAllProjectsWaitingforApproval();
   Task<ProjectInfoModel> GetProjectInfo(string id);
   Task UpdateProjectInfo(ProjectInfoModel project);
   Task UpvoteProjectInfo(string projectId, string userId);
}