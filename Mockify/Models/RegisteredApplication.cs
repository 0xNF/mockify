using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Mockify.Models {
    public class RegisteredApplication {

        private static Random random = new Random();
        private static char[] availablechars = "abcdefghijklmnopqrsytuvwxyz0123456789".ToCharArray();


        [Key]
        public string ClientId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationDescription { get; set; }
        public string ClientSecret { get; set; }
        public List<RedirectURI> RedirectURIs { get; set; } = new List<RedirectURI>();
        public List<UserApplicationToken> UserApplicationTokens { get; set; } = new List<UserApplicationToken>();

        private static string GetRandomString(int length, char[] charset) {
            string str = "";
            for (int i = 0; i < 32; i++) {
                int r = random.Next() % 32;
                str += availablechars[r];
            }
            return str;
        }

        public static async Task<string> CreateClientId(string name) {
            // TODO should eventually query the Database to make sure no conflicting name
            await Task.Delay(0);
            return GetRandomString(32, availablechars);
        }

        public static async Task<string> CreateClientSecret(string name) {
            await Task.Delay(0);
            return GetRandomString(32, availablechars);
        }
    }
}