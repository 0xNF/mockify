using System.Collections.Generic;

namespace Mockify.Models.Spotify {
    public class SpotifyScope {

        public string Name;
        public string Description;

        public static Dictionary<string, SpotifyScope> IdMap = new Dictionary<string, SpotifyScope>();
        public static List<SpotifyScope> AllValues = new List<SpotifyScope>();

        public SpotifyScope(string name, string description) {
            this.Name = name;
            this.Description = description;
            IdMap.Add(name, this);
            AllValues.Add(this);
        }

        public static SpotifyScope Public = new SpotifyScope("user-info-public", "Read your publicly available information");

        public static SpotifyScope playlistreadprivate = new SpotifyScope("playlist-read-private", "Access your private playlists");
        public static SpotifyScope playlistreadcollaborative = new SpotifyScope("playlist-read-collaborative", "Access your collaborative playlists");
        public static SpotifyScope playlistmodifypublic = new SpotifyScope("playlist-modify-public", "Manage your public playlists");
        public static SpotifyScope playlistmodifyprivate = new SpotifyScope("playlist-modify-private", "Manage your private playlists");

        public static SpotifyScope streaming = new SpotifyScope("streaming", "Play music and control playback on your other devices");

        public static SpotifyScope ugcimageupload = new SpotifyScope("ugc-image-upload", "Upload images to Spotify on your behalf");

        public static SpotifyScope userfollowmodify = new SpotifyScope("user-follow-modify", "Manage who you are following");
        public static SpotifyScope userfollowread = new SpotifyScope("user-follow-read", "Access your followers and who you are following");

        public static SpotifyScope userlibraryread = new SpotifyScope("user-library-read", "Access your saved tracks and albums");
        public static SpotifyScope userlibrarymodify = new SpotifyScope("user-library-modify", "Manage your saved tracks and albums");

        public static SpotifyScope userreadprivate = new SpotifyScope("user-read-private", "Access your subscription details");
        public static SpotifyScope userreadbirthdate = new SpotifyScope("user-read-birthdate", "Receive your birthdate");
        public static SpotifyScope userreademail = new SpotifyScope("user-read-email", "Get your real email address");

        public static SpotifyScope usertopread = new SpotifyScope("user-top-read", "Read your top artists and tracks");

        public static SpotifyScope userreadplaybackstate = new SpotifyScope("user-read-playback-state", "Read your currently playing track and Spotify Connect devices information");
        public static SpotifyScope usermodifyplaybackstate = new SpotifyScope("user-modify-playback-state", "Control playback on your Spotify clients and Spotify Connect devices");
        public static SpotifyScope userreadcurrentlyplaying = new SpotifyScope("user-read-currently-playing", "Read your currently playing track");
        public static SpotifyScope userreadrecentlyplayed = new SpotifyScope("user-read-recently-played", "Access your recently played items");
    }
}
