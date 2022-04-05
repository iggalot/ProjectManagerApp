using ProjectManagerAppUI.Models;

namespace ProjectManagerAppUI.Pages;

public partial class Create
 {
     private CreateProjectInfoModel projectinfo = new();
     private List<CategoryModel> categories;
     private UserModel loggedInUser;
     protected async override Task OnInitializedAsync()
     {
         categories = await categoryData.GetAllCategories();
         //var authState = await authProvider.GetAuthenticationStateAsync();
         //string objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;
         //loggedInUser = await userData.GetUserFromAuthentication(objectId);
         loggedInUser = await authProvider.GetUserFromAuth(userData);
     }

     private void ClosePage()
     {
         navManager.NavigateTo("/");
     }

     private async Task CreateProjectInfo()
     {
         ProjectInfoModel p = new();
         p.ProjectName = projectinfo.ProjectInfo;
         p.Description = projectinfo.Description;
         p.Author = new BasicUserModel(loggedInUser);
         p.Category = categories.Where(c => c.Id == projectinfo.CategoryId).FirstOrDefault();
         if (p.Category is null)
         {
             projectinfo.CategoryId = "";
             return;
         }

         await projectinfoData.CreateProjectInfo(p);
         projectinfo = new();
         ClosePage();
     }
 }
