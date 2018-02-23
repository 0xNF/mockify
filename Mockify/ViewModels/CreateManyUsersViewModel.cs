using System.ComponentModel.DataAnnotations;

namespace Mockify.ViewModels {
    public class CreateManyUsersViewModel {

        [Key]
        public int Id { get; set; } // ????
        
        [Display(Name = "Create This Many Random Users")]
        public int CreateThisManyUsers { get; set; }


    }

}
