using System.ComponentModel.DataAnnotations;

namespace ProjectManagerAppUI.Models;

public class CreateProjectInfoModel
{
   [Required]
   [MaxLength(75)]
   public string ProjectInfo { get; set; }

   [Required]
   [MinLength(1)]
   [Display(Name = "Category")]
   public string CategoryId { get; set; }

   [MaxLength(500)]
   public string Description { get; set; }


}
