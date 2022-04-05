using Microsoft.AspNetCore.Components;

namespace ProjectManagerAppUI.Pages;

public partial class Details
{
   [Parameter]
   public string Id { get; set; }

   private ProjectInfoModel project;
   private UserModel loggedInUser;
   private List<StatusModel> statuses;
   private string settingStatus = "";
   private string urlText = "";
   protected async override Task OnInitializedAsync()
   {
      project = await projectinfoData.GetProjectInfo(Id);
      loggedInUser = await authProvider.GetUserFromAuth(userData);
      statuses = await statusData.GetAllStatuses();
   }

   private async Task CompleteSetStatus()
   {
      switch (settingStatus)
      {
         case "completed":
            if (string.IsNullOrWhiteSpace(urlText))
            {
               return;
            }

            project.ProjectStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
            project.OwnerNotes = $"You are right, this is an important topic for developers. We created a resource about it here: <a href='{urlText}' target='_blank'>{urlText}</a>";
            break;
         case "watching":
            project.ProjectStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
            project.OwnerNotes = "We noticed the interest this suggestion is getting! If more people are interested we may address this topic in an upcoming resource.";
            break;
         case "upcoming":
            project.ProjectStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
            project.OwnerNotes = "Great suggestion!  We have a resource in the pipeline to address this topic.";
            break;
         case "dismissed":
            project.ProjectStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
            project.OwnerNotes = "Sometimes a good idea doesn't fit within out scope and vision. This is one of those ideas.";
            break;
         default:
            return;
      }

      settingStatus = null;
      await projectinfoData.UpdateProjectInfo(project);
   }

   private void ClosePage()
   {
      navManager.NavigateTo("/");
   }

   private string GetUpvoteTopText()
   {
      if (project.UserVotes?.Count > 0)
      {
         return project.UserVotes.Count.ToString("00");
      }
      else
      {
         if (project.Author.Id == loggedInUser?.Id)
         {
            return "Awaiting";
         }
         else
         {
            return "Click To";
         }
      }
   }

   private string GetUpvoteBottomText()
   {
      if (project.UserVotes?.Count > 1)
      {
         return "Upvotes";
      }
      else
      {
         return "Upvote";
      }
   }

   private async Task VoteUp()
   {
      if (loggedInUser is not null)
      {
         if (project.Author.Id == loggedInUser.Id)
         {
            // Can't vote on yor own suggestion
            return;
         }

         if (project.UserVotes.Add(loggedInUser.Id) == false)
         {
            project.UserVotes.Remove(loggedInUser.Id);
         }

         await projectinfoData.UpvoteProjectInfo(project.Id, loggedInUser.Id);
      }
      else
      {
         navManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
      }
   }

   private string GetVoteClass()
   {
      if (project.UserVotes is null || project.UserVotes.Count == 0)
      {
         return "project-detail-no-votes";
      }
      else if (project.UserVotes.Contains(loggedInUser?.Id))
      {
         return "project-detail-voted";
      }
      else
      {
         return "project-detail-not-voted";
      }
   }

   private string GetStatusClass()
   {
      if (project is null || project.ProjectStatus is null)
      {
         return "project-detail-status-none";
      }

      string output = project.ProjectStatus.StatusName switch
      {
         "Completed" => "project-detail-status-completed",
         "Watching" => "project-detail-status-watching",
         "Upcoming" => "project-detail-status-upcoming",
         "Dismissed" => "project-detail-status-dismissed",
         _ => "suggestion-detail-status-none",
      };
      return output;
   }
}
