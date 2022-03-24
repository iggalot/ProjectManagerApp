namespace ProjectManagerAppLibrary.Models
{
    public class ProjectInfoModel
    {
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string Id { get; set; }

      public string ProjectName { get; set; }
      public string Description { get; set; }
      public DateTime DateCreated { get; set; } = DateTime.UtcNow;
      public CategoryModel Category { get; set; }
      public BasicUserModel Author { get; set; }
      public HashSet<string> AssignedDesigner { get; set; } = new();
      public HashSet<string> UserVotes { get; set; } = new();
      public StatusModel ProjectStatus { get; set; }
      public string OwnerNotes { get; set; }
      public bool ApprovedForRelease { get; set; } = false;
      public bool Archived { get; set; } = false;
      public bool Rejected { get; set; } = false;
   }
}
