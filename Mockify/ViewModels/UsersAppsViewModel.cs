using Mockify.Models;
using System.Collections.Generic;

namespace Mockify.ViewModels {
    public class UsersAppsViewModel {
        public ApplicationUser User { get; set; }
        public List<RegisteredApplication> Applications { get; set; }
    }
}
