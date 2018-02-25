using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mockify.Models {

    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser {

        public string Birthdate { get; set; }
        public string Country { get; set; }

        public string Product { get; set; }

        [StringLength(32, MinimumLength = 0)]
        public string DisplayName { get; set; }

        public List<UserApplicationToken> UsersApplications { get; set; } = new List<UserApplicationToken>();
        public List<ExternalUrl> Externalurls { get; set; } = new List<ExternalUrl>();


        public int Followers { get; set; } = 0;

        /// <summary>
        /// The Rate Limits for the user, including whether they are limited, when they can make calls again, and the tally of their calls vs max allowed per window.
        /// </summary>
        RateLimits RateLimits { get; set; }

        public List<UserApplicationToken> UserApplicationTokens { get; set; } = new List<UserApplicationToken>();


        private static Random r = new Random();
        public static string RandomPass() {
            string s = "";
            string valids = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_- ";
            for (int i = 0; i < r.Next(8, 32); i++) {
                int whichIdx = r.Next(0, valids.Length);
                s += valids[whichIdx];
            }
            return s;
        }

        public static ApplicationUser Randomize() {
            string randomBday() {
                string month = "" + r.Next(1, 13);
                string day = "" + r.Next(1, 29);
                string year = "" + r.Next(1900, 2018);
                return $"{(month.Length == 1 ? "0" + month : month)}/{(day.Length == 1 ? "0"+day : day)}/{year}"; // MM/d/yyyy
            }
            string randomName() {
                string s = "";
                string valids = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_- ";
                for(int i = 0; i < r.Next(0, 16); i++) {
                    int whichIdx = r.Next(0, valids.Length);
                    s += valids[whichIdx];
                }
                return s;
            }
            string randomEmail() {
                string local = randomName();
                string domain = randomName();
                string route = "";
                string valids = "abcdefghijklmnopqrstuvwxyz0123456789-";
                for (int i = 0; i < r.Next(0, 16); i++) {
                    int whichIdx = r.Next(0, valids.Length);
                    route += valids[whichIdx];
                }
                return $"{local}@{domain}.{route}";
            }

            return new ApplicationUser() {
                Birthdate = randomBday(),
                Country = Spotify.Country.SpotifyMarkets[r.Next(0, Spotify.Country.SpotifyMarkets.Count)].FormalName,
                Product = Spotify.SpotifyAccountTypes.AllValues[r.Next(0, Spotify.SpotifyAccountTypes.AllValues.Count)].Name,
                DisplayName = r.Next(0, 2) == 0 ? randomName() : null,
                Followers = 0,
                UserName = Guid.NewGuid().ToString(),
                Email = randomEmail()
            };
        }
    }

    public class UserApplicationToken {

        [Key]
        public string TokenId { get; set; }

        public string ClientId { get; set; }

        public string TokenType { get; set; } // access, refresh,

        public string TokenValue { get; set; }

        public DateTime ExpiresAt { get; set; }
    }

}
