namespace ProjectManagerAppUI.Pages;

public partial class Profile
 {
     private UserModel loggedInUser;
     private List<ProjectInfoModel> submissions;
     private List<ProjectInfoModel> approved;
     private List<ProjectInfoModel> archived;
     private List<ProjectInfoModel> pending;
     private List<ProjectInfoModel> rejected;
     protected async override Task OnInitializedAsync()
     {
         loggedInUser = await authProvider.GetUserFromAuth(userData);
         var results = await projectinfoData.GetUsersProjectInfos(loggedInUser.Id);
         if (loggedInUser is not null && results is not null)
         {
             submissions = results.OrderByDescending(s => s.DateCreated).ToList();
             approved = submissions.Where(s => s.ApprovedForRelease && s.Archived == false && s.Rejected == false).ToList();
             archived = submissions.Where(s => s.Archived && s.Rejected == false).ToList();
             pending = submissions.Where(s => s.ApprovedForRelease == false && s.Rejected == false).ToList();
             rejected = submissions.Where(s => s.Rejected).ToList();
         }
     }

     private void ClosePage()
     {
         navManager.NavigateTo("/");
     }
 }
