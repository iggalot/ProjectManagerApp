namespace ProjectManagerAppUI.Pages;

public partial class AdminApproval
 {
     private List<ProjectInfoModel> submissions;
     private ProjectInfoModel editingModel;
     private string currentEditingTitle = "";
     private string editedTitle = "";
     private string currentEditingDescription = "";
     private string editedDescription = "";
     protected async override Task OnInitializedAsync()
     {
         submissions = await projectinfoData.GetAllProjectsWaitingforApproval();
     }

     private async Task ApproveSubmission(ProjectInfoModel submission)
     {
         submission.ApprovedForRelease = true;
         submissions.Remove(submission);
         await projectinfoData.UpdateProjectInfo(submission);
     }

     private async Task RejectSubmission(ProjectInfoModel submission)
     {
         submission.Rejected = true;
         submissions.Remove(submission);
         await projectinfoData.UpdateProjectInfo(submission);
     }

     private void EditTitle(ProjectInfoModel model)
     {
         editingModel = model;
         editedTitle = model.ProjectName;
         currentEditingTitle = model.Id;
         currentEditingDescription = "";
     }

     private async Task SaveTitle(ProjectInfoModel model)
     {
         currentEditingTitle = string.Empty;
         model.ProjectName = editedTitle;
         await projectinfoData.UpdateProjectInfo(model);
     }

     private void EditDescription(ProjectInfoModel model)
     {
         editingModel = model;
         editedDescription = model.Description;
         currentEditingTitle = "";
         currentEditingDescription = model.Id;
     }

     private async Task SaveDescription(ProjectInfoModel model)
     {
         currentEditingDescription = string.Empty;
         model.Description = editedDescription;
         await projectinfoData.UpdateProjectInfo(model);
     }

     private void ClosePage()
     {
         navManager.NavigateTo("/");
     }
 }
