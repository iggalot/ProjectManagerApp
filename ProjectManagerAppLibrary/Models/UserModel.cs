namespace ProjectManagerAppLibrary.Models;
public class UserModel
{
   [BsonId]
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; }
   public string ObjectIdentifier { get; set; }
   public string FirstName { get; set; }
   public string LastName { get; set; }
   public string DisplayName { get; set; }

   public string EmailAddress { get; set; }
   public List<BasicProjectInfoModel> AuthoredProjects { get; set; } = new();
   public List<BasicProjectInfoModel> AssignedToProjects { get; set; } = new();
   public List<BasicProjectInfoModel> VotedOnProjectInfos { get; set; } = new();


}
