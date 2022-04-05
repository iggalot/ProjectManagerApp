namespace ProjectManagerAppUI.Pages;

public partial class Index
 {
     private UserModel loggedInUser;
     private List<ProjectInfoModel> projects;
     private List<CategoryModel> categories;
     private List<StatusModel> statuses;
     private ProjectInfoModel archivingProject;
     private string selectedCategory = "All";
     private string selectedStatus = "All";
     private string searchText = "";
     private bool isSortedByNew = true;
     private bool showCategories = false;
     private bool showStatuses = false;
     protected async override Task OnInitializedAsync()
     {
         categories = await categoryData.GetAllCategories();
         statuses = await statusData.GetAllStatuses();
         await LoadAndVerifyUser();
     }

     private async Task ArchiveProject()
     {
         archivingProject.Archived = true;
         await projectinfoData.UpdateProjectInfo(archivingProject);
         projects.Remove(archivingProject);
         archivingProject = null;
     //await FilterSuggestions();
     }

     private void LoadCreatePage()
     {
         if (loggedInUser is not null)
         {
             navManager.NavigateTo("/Create");
         }
         else
         {
             navManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
         }
     }

     private async Task LoadAndVerifyUser()
     {
         var authState = await authProvider.GetAuthenticationStateAsync();
         string objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;
         if (string.IsNullOrWhiteSpace(objectId) == false)
         {
             loggedInUser = await userData.GetUserFromAuthentication(objectId) ?? new();
             string firstName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value;
             string lastName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value;
             string displayName = authState.User.Claims.FirstOrDefault(c => c.Type.Equals("name"))?.Value;
             string email = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;
             bool isDirty = false;
             if (objectId.Equals(loggedInUser.ObjectIdentifier) == false)
             {
                 isDirty = true;
                 loggedInUser.ObjectIdentifier = objectId;
             }

             if (firstName.Equals(loggedInUser.FirstName) == false)
             {
                 isDirty = true;
                 loggedInUser.FirstName = firstName;
             }

             if (lastName.Equals(loggedInUser.LastName) == false)
             {
                 isDirty = true;
                 loggedInUser.LastName = lastName;
             }

             if (displayName.Equals(loggedInUser.DisplayName) == false)
             {
                 isDirty = true;
                 loggedInUser.DisplayName = displayName;
             }

             if (email.Equals(loggedInUser.EmailAddress) == false)
             {
                 isDirty = true;
                 loggedInUser.EmailAddress = email;
             }

             if (isDirty)
             {
                 if (string.IsNullOrWhiteSpace(loggedInUser.Id))
                 {
                     await userData.CreateUser(loggedInUser);
                 }
                 else
                 {
                     await userData.UpdateUser(loggedInUser);
                 }
             }
         }
     }

     protected async override Task OnAfterRenderAsync(bool firstRender)
     {
         if (firstRender)
         {
             await LoadFilterState();
             await FilterSuggestions();
             StateHasChanged();
         }
     }

     private async Task LoadFilterState()
     {
         var stringResults = await sessionStorage.GetAsync<string>(nameof(selectedCategory));
         selectedCategory = stringResults.Success ? stringResults.Value : "All";
         stringResults = await sessionStorage.GetAsync<string>(nameof(selectedStatus));
         selectedStatus = stringResults.Success ? stringResults.Value : "All";
         stringResults = await sessionStorage.GetAsync<string>(nameof(searchText));
         searchText = stringResults.Success ? stringResults.Value : "";
         var boolResults = await sessionStorage.GetAsync<bool>(nameof(isSortedByNew));
         isSortedByNew = boolResults.Success ? boolResults.Value : true;
     }

     private async Task SaveFilterState()
     {
         await sessionStorage.SetAsync(nameof(selectedCategory), selectedCategory);
         await sessionStorage.SetAsync(nameof(selectedStatus), selectedStatus);
         await sessionStorage.SetAsync(nameof(searchText), searchText);
         await sessionStorage.SetAsync(nameof(isSortedByNew), isSortedByNew);
     }

     private async Task FilterSuggestions()
     {
         var output = await projectinfoData.GetAllApprovedProjectInfos();
         if (selectedCategory != "All")
         {
             output = output.Where(s => s.Category?.CategoryName == selectedCategory).ToList();
         }

         if (selectedStatus != "All")
         {
             output = output.Where(s => s.ProjectStatus?.StatusName == selectedStatus).ToList();
         }

         if (string.IsNullOrWhiteSpace(searchText) == false)
         {
             output = output.Where(s => s.ProjectName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) || s.Description.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
         }

         if (isSortedByNew)
         {
             output = output.OrderByDescending(s => s.DateCreated).ToList();
         }
         else
         {
             output = output.OrderByDescending(s => s.UserVotes.Count).ThenByDescending(s => s.DateCreated).ToList();
         }

         projects = output;
         await SaveFilterState();
     }

     private async Task OrderByNew(bool isNew)
     {
         isSortedByNew = isNew;
         await FilterSuggestions();
     }

     private async Task OnSearchInput(string searchInput)
     {
         searchText = searchInput;
         await FilterSuggestions();
     }

     private async Task OnCategoryClick(string category = "All")
     {
         selectedCategory = category;
         showCategories = false;
         await FilterSuggestions();
     }

     private async Task OnStatusClick(string status = "All")
     {
         selectedStatus = status;
         showStatuses = false;
         await FilterSuggestions();
     }

     private async Task VoteUp(ProjectInfoModel project)
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
             if (isSortedByNew == false)
             {
                 projects = projects.OrderByDescending(s => s.UserVotes.Count).ThenByDescending(s => s.DateCreated).ToList();
             }
         }
         else
         {
             navManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
         }
     }

     private string GetUpvoteTopText(ProjectInfoModel project)
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

     private string GetUpvoteBottomText(ProjectInfoModel project)
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

     private void OpenDetails(ProjectInfoModel project)
     {
         navManager.NavigateTo($"/Details/{project.Id}");
     }

     private string SortedByNewClass(bool isNew)
     {
         if (isNew == isSortedByNew)
         {
             return "sort-selected";
         }
         else
         {
             return "";
         }
     }

     private string GetVoteClass(ProjectInfoModel project)
     {
         if (project.UserVotes is null || project.UserVotes.Count == 0)
         {
             return "project-entry-no-votes";
         }
         else if (project.UserVotes.Contains(loggedInUser?.Id))
         {
             return "project-entry-voted";
         }
         else
         {
             return "project-entry-not-voted";
         }
     }

     private string GetStatusClass(ProjectInfoModel project)
     {
         if (project is null || project.ProjectStatus is null)
         {
             return "project-entry-status-none";
         }

         string output = project.ProjectStatus.StatusName switch
         {
             "Completed" => "project-entry-status-completed",
             "Watching" => "project-entry-status-watching",
             "Upcoming" => "project-entry-status-upcoming",
             "Dismissed" => "project-entry-status-dismissed",
             _ => "suggestion-entry-status-none",
         };
         return output;
     }

     private string GetSelectedCategory(string category = "All")
     {
         if (category == selectedCategory)
         {
             return "selected-category";
         }
         else
         {
             return "";
         }
     }

     private string GetSelectedStatus(string status = "All")
     {
         if (status == selectedStatus)
         {
             return "selected-status";
         }
         else
         {
             return "";
         }
     }
 }
