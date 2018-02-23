using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Mockify.ViewModels {
    public class CreateUserViewModel {

        [Key]
        //[Required]
        public string UserId { get; set; }

        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [EmailAddress]
        [StringLength(128, MinimumLength = 0)]
        public string Email { get; set; }

        [Display(Name = "Birth Date (mm/dd/yy)")]
        [StringLength(10, MinimumLength = 0)]
        public string BirthDate { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 2)]
        public string Country { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string Product { get; set; }

    }

}
