namespace ProjectManagerAppLibrary.Models;

public class BasicProjectInfoModel
{
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; }
   public string Project { get; set; }

   public BasicProjectInfoModel()
   {

   }

   public BasicProjectInfoModel(ProjectInfoModel project)
   {
      Id = project.Id;
      Project = project.ProjectName;
   }
}
