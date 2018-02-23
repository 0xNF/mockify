namespace Mockify.Models {
    public class RedirectURI {
        public int Id { get; set; }
        public string URI { get; set; }

        public string RegisteredApplicationId { get; set; }
        public RegisteredApplication RegisteredApplication { get; set; }

        public RedirectURI() {

        }

        public RedirectURI(string rdi) {
            this.URI = rdi;
        }
    }

}