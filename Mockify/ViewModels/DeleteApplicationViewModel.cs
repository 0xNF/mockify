using System.ComponentModel.DataAnnotations;

namespace Mockify.ViewModels {
    public class DeleteApplicationViewModel {
        [Required]
        [StringLength(32)]
        public string ApplicationClientId { get; set; }
    }
}
