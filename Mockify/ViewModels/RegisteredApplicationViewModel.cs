using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mockify.ViewModels {
    public class RegisteredApplicationViewModel {
        [Required]
        [StringLength(60, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Application Name")]
        public string ApplicationName { get; set; }

        [StringLength(250, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 0)]
        [DataType(DataType.Text)]
        [Display(Name = "Application Description")]
        public string ApplicationDescription { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        [Display(Name = "Redirect URIs")]
        public IEnumerable<string> RedirectURIs { get; set; }
    }

}
