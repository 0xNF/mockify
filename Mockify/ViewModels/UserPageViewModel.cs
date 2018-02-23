using Mockify.Models;

namespace Mockify.ViewModels {
    // For the render partials
    public class UserPageViewModel {
        public Paging<ApplicationUser> UserListViewModel { get; set; }
        public CreateUserViewModel CreateSingleViewModel { get; set; }
        public CreateManyUsersViewModel CreateManyViewModel { get; set; }
    }
}
