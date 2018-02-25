using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mockify.Models.Spotify {
    public class SpotifyAccountTypes {

        public int Id;
        public string Name;

        public static Dictionary<int, SpotifyAccountTypes> IdMap = new Dictionary<int, SpotifyAccountTypes>();
        public static List<SpotifyAccountTypes> AllValues = new List<SpotifyAccountTypes>();

        public SpotifyAccountTypes(int id, string name) {
            this.Id = id;
            this.Name = name;
            IdMap.Add(id, this);
            AllValues.Add(this);
        }

        public static SpotifyAccountTypes Free = new SpotifyAccountTypes(0, "Free");
        public static SpotifyAccountTypes Open = new SpotifyAccountTypes(1, "Open");
        public static SpotifyAccountTypes Premium = new SpotifyAccountTypes(2, "Premium");

    }

    public static class SpotifyConstants {
        public const int port = 44345;
        public const string host = "localhost";
        public const string ApiHost = "api";
        public const string AccountHost = "accounts";
    }
}
