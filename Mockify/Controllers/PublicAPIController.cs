using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Mockify.Models;

namespace Mockify.Controllers {
    [Route("/v1/")]
    public class PublicAPIController : Controller {

        public const string defaultLocale = "en_US";
        public const string defaultMarket = "US";
        public const string defaultCountry = "US";

        public static List<Endpoint> Endpoints = new List<Endpoint>() {
            //new Endpoint("")
        };

        public PublicAPIController() {

        }

        #region Album Endpoints
        // https://beta.developer.spotify.com/documentation/web-api/reference/albums/get-album/
        [HttpGet("albums/{id}"), ActionName("Get_An_Album")]
        public IActionResult GetAnAlbum(string id) {
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/albums/get-albums-tracks/
        [HttpGet("albums/{id}/tracks"), ActionName("Get_An_Albums_Tracks")]
        public IActionResult GetAnAlbumsTracks(string id, int limit = 20, int offset = 0, string market = defaultMarket) {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/albums/get-several-albums/
        [HttpGet("albums"), ActionName("Get_Several_Albums")]
        public IActionResult GetSeveralAlbums(string ids, string market = defaultMarket) {
            int maxLimit = 20;
            int minLimit = 1;
            return NoContent();
        }
        #endregion

        #region Artist Endpoints
        // https://beta.developer.spotify.com/documentation/web-api/reference/artists/get-artist/
        [HttpGet("artists/{id}"), ActionName("Get_An_Artist")]
        public IActionResult GetAnArtist(string id) {
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/artists/get-artists-albums/
        [HttpGet("artists/{id}/albums"), ActionName("Get_An_Artists_Albums")]
        public IActionResult GetAnArtistsAlbums(string id, string album_type = "all", string market = defaultMarket, string country = defaultCountry, int limit = 20, int offset = 0) {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/artists/get-artists-top-tracks/
        [HttpGet("artists/{id}/top-tracks"), ActionName("Get_An_Artists_TopTracks")]
        public IActionResult GetAnArtistsTopTracks(string id, string country) {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/artists/get-related-artists/
        [HttpGet("artists/{id}/related-artists"), ActionName("Get_An_Artists_Related_Artists")]
        public IActionResult GetAnArtistsRelatedArtists(string id) {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/artists/get-several-artists/
        [HttpGet("artists/{id}"), ActionName("Get_Several_Artists")]
        public IActionResult GeSeveralArtists(string ids) {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }
        #endregion

        #region Browse Endpoints
        // https://beta.developer.spotify.com/documentation/web-api/reference/browse/get-category/
        [HttpGet("browse/{category_id}"), ActionName("Get_A_Category")]
        public IActionResult GetACategory(string category_id, string country = defaultCountry, string locale = defaultLocale) {
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/browse/get-categorys-playlists/
        [HttpGet("browse/{category_id}/playlists"), ActionName("Get_A_Categorys_Playlists")]
        public IActionResult GetACategorysPlaylists(string category_id, string country = defaultCountry, int limit = 20, int offet = 0) {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/browse/get-list-categories/
        [HttpGet("browse/categories"), ActionName("Get_A_List_Of_Categories")]
        public IActionResult GetAListOfCategories(string country = defaultCountry, string locale = defaultLocale, int limit = 20, int offset = 0) {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/browse/get-list-featured-playlists/
        [HttpGet("browse/featured-playlists"), ActionName("Get_A_List_Of_Featured_Playlists")]
        public IActionResult GetAListOfFeaturedCategories(string locale = defaultLocale, string country = defaultCountry, string timestamp = null, int limit = 20, int offset = 0) {
            int maxLimit = 50;
            int minLimit = 1;
            DateTime dt = DateTime.UtcNow;
            if (timestamp != null) {
                /// TODO parse timestamp....
            }
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/browse/get-list-new-releases/
        [HttpGet("browse/new-releases"), ActionName("Get_A_List_Of_New_Releases")]
        public IActionResult GetAListOfNewReleases(string country = defaultCountry, int limit = 20, int offset = 0) {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/browse/get-recommendations/
        [HttpGet("recommendations"), ActionName("Get_Recommendations_Based_On_Seeds")]
        public IActionResult GetRecommendations() {
            // This is too complicated to leave to the method params.
            // Have to parse the whole thing manually.
            int maxLimit = 100;
            int minLimit = 1;
            return NoContent();
        }
        #endregion

        #region Follow Endpoints
        // https://beta.developer.spotify.com/documentation/web-api/reference/follow/check-current-user-follows/
        [HttpGet("me/following/contains"), ActionName("Check_If_User_Follows_Users_Or_Artists")]
        public IActionResult FollowingContains(string type, string ids) {
            //Type is either 'artist' or 'user'
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/follow/check-user-following-playlist/
        [HttpGet("users/{owner_id}/playlists/{playlist_id}/followers/contains"), ActionName("Check_If_User_Follows_Playlist")]
        public IActionResult CheckUserFollowsPlaylist(string owner_id, string playlist_id, string ids) {
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/follow/follow-artists-users/
        [HttpPut("me/following"), ActionName("Follow_Artist_Or_User")]
        public IActionResult FollowArtistOrUser(string type, string ids = "") {
            // type is either 'artist' or 'user'
            // note: uses request-body, content-type of application/json, but ignored if ids is supplied in query
            int maxItems = 50;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/follow/follow-playlist/
        [HttpPut("users/{owner_id}/playlists/{playlist_id}/followers"), ActionName("Follow_A_Playlist")]
        public IActionResult FollowPlaylist(string owner_id, string playlist_id) {
            // requires content-type The content type of the request body: application/json
            // is_public is a bool in the body
            int maxItems = 50;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/follow/get-followed/
        [HttpGet("me/following"), ActionName("Get_Users_Followed_Artists")]
        public IActionResult GetUsersFollowedArtists(string type, int limit = 20, string after = "") {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/follow/unfollow-artists-users/
        [HttpDelete("me/following"), ActionName("Unfollow_Artists_Or_Users")]
        public IActionResult UnfollowArtistsOrUsers(string type, string ids = "") {
            // type is either 'artist' or 'user'
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/follow/unfollow-playlist/
        [HttpDelete("users/{owner_id}/playlists/{playlist_id}/followers"), ActionName("Unfollow_A_Playlist")]
        public IActionResult UnfollowPlaylist(string owner_id, string playlist_id) {
            int maxLimit = 50;
            int minLimit = 1;
            return NoContent();
        }
        #endregion

        #region Library Endpoints
        // https://beta.developer.spotify.com/documentation/web-api/reference/library/check-users-saved-albums/
        [HttpGet("me/albums/contains"), ActionName("Check_Users_Saved_Albums")]
        public IActionResult CheckUsersSavedAlbums(string ids) {
            int maxItems = 50;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/library/check-users-saved-tracks/
        [HttpGet("me/tracks/contains"), ActionName("Check_Users_Saved_Tracks")]
        public IActionResult CheckUsersSavedTracks(string ids) {
            int maxItems = 50;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/library/get-users-saved-albums/
        [HttpGet("me/albums"), ActionName("Get_Users_Saved_Albums")]
        public IActionResult GetUsersSavedAlbums(int limit = 20, int offset = 0, string market = defaultMarket) {
            int maxItems = 50;
            int minItems = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/library/get-users-saved-tracks/
        [HttpGet("me/tracks"), ActionName("Get_Users_Saved_Tracks")]
        public IActionResult GetUsesSavedTracks(int limit = 20, int offet = 0, string market = defaultMarket) {
            int maxItems = 50;
            int minItems = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/library/remove-albums-user/
        [HttpDelete("me/albums"), ActionName("Remove_Albums_For_Current_User")]
        public IActionResult RemoveSavedAlbums(string ids = null) {
            int maxItems = 50;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/library/remove-tracks-user/
        [HttpDelete("me/tracks"), ActionName("Remove_Tracks_For_Current_User")]
        public IActionResult RemoveSavedTracks(string ids = null) {
            int maxItems = 50;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/library/save-albums-user/
        [HttpPut("me/albums"), ActionName("Add_Saved_Albums_For_User")]
        public IActionResult AddSavedAlbums(string ids = null) {
            int maxItems = 50;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/library/save-tracks-user/
        [HttpPut("me/tracks"), ActionName("Get_A_Category")]
        public IActionResult AddSavedTracks(string ids = null) {
            int maxItems = 50;
            return NoContent();
        }
        #endregion

        #region Personalization Endpoints
        // https://beta.developer.spotify.com/documentation/web-api/reference/personalization/get-users-top-artists-and-tracks/
        [HttpGet("me/top/{type}"), ActionName("Get_Users_Top_Tracks_And_Artists")]
        public IActionResult CheckUsersSavedAlbums(string type, int limit = 20, int offet = 0, string time_range = "medium_term") {
            int maxLimit = 50;
            int minLimit = 1;
            int maxItems = 50;
            // type is either 'artist' or 'track'
            // time_range is either 'long_term', 'medium_term' or 'short_term'
            return NoContent();
        }
        #endregion

        #region Player Region NYI BECAUSE BETA
        //[HttpGet("TEMPLATE"), ActionName("TEMPLATE")]
        //public IActionResult TEMPLATE() {
        //    return NoContent();
        //}
        #endregion

        #region Playlist Endpoints
        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/add-tracks-to-playlist/
        [HttpPost("users/{user_id}/playlists/{playlist_id}/tracks"), ActionName("Add_Tracks_To_A_Playlist")]
        public IActionResult AddTracksToAPlaylist(string user_id, string playlist_id, string uris = null, int? position = null) {
            int maxItems = 50;
            // type is either 'artist' or 'track'
            // time_range is either 'long_term', 'medium_term' or 'short_term'
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/change-playlist-details/
        [HttpPut("users/{user_id}/playlists/{playlist_id}"), ActionName("Change_A_Playlists_Details")]
        public IActionResult ChangePlaylistsDetails(string user_id, string playlist_id) {
            // Everything is in the Body
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/create-playlist/
        [HttpPost("users/{user_id}/playlists"), ActionName("Create_A_Playlist")]
        public IActionResult CreatePlaylist(string user_id, string playlist_id) {
            // Everything is in the Body
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/get-a-list-of-current-users-playlists/
        [HttpGet("me/playlists"), ActionName("Get_Current_Users_Playlists")]
        public IActionResult GetCurrentUsersPlaylists(int limit = 20, int offset = 0) {
            int maxLimit = 50;
            int minLimit = 1;
            int maxOffset = 100000;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/get-list-users-playlists/
        [HttpGet("users/{user_id}/playlists"), ActionName("Get_A_Users_Playlists")]
        public IActionResult GetAUsersPlaylists(string user_id, int limit = 20, int offset = 0) {
            int maxLimit = 50;
            int minLimit = 1;
            int maxOffset = 100000;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/get-playlist-cover/
        [HttpGet("users/{user_id}/playlists/{playlist_id}/images"), ActionName("Get_Playlist_Cover_Image")]
        public IActionResult GetPlaylistCoverImage(string user_id, string playlist_id) {
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/get-playlist/
        [HttpGet("users/{user_id}/playlists/{playlist_id}"), ActionName("Get_A_Playlist")]
        public IActionResult GetPlaylistCoverImage(string user_id, string playlist_id, string market = defaultMarket, string fields = null) {
            // fields too complicated to parse in params, will parse in method
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/get-playlists-tracks/
        [HttpGet("users/{user_id}/playlists/{playlist_id}/tracks"), ActionName("Get_A_Playlists_Tracks")]
        public IActionResult GetPlaylistTracks(string user_id, string playlist_id, int limit = 20, int offset = 0, string market = defaultMarket, string fields = null) {
            // fields too complicated to parse in params, will parse in method
            int maxLimit = 100;
            int minLimit = 1;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/reorder-playlists-tracks/
        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/replace-playlists-tracks/
        [HttpPut("users/{user_id}/playlists/{playlist_id}/tracks"), ActionName("Reorder_Or_Replace_Playlists_Tracks")]
        public IActionResult ReorderPlaylistsTracks(string user_id, string playlist_id) {
            // All in body
            // Could be a Replace or a Reorder, depending on body structure
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/playlists/upload-custom-playlist-cover/
        [HttpPut("users/{user_id}/playlists/{playlist_id}/images"), ActionName("Upload_Custom_Playlist_Cover_Image")]
        public IActionResult UploadCustomPlaylistCoverImage(string user_id, string playlist_id) {
            // All in body
            return NoContent();
        }
        #endregion

        #region Search Endpoints
        // https://beta.developer.spotify.com/documentation/web-api/reference/search/search/
        [HttpGet("search"), ActionName("search")]
        public IActionResult Search(string q, string type, string market = defaultMarket, int limit = 20, int offset = 0) {
            int maxLimit = 50;
            int minLimit = 1;
            int maxOffset = 100_000;
            // type is a comma sep list of 'artist', 'playlist', 'track , 'album'
            return NoContent();
        }
        #endregion

        #region Track Endpoints

        // https://beta.developer.spotify.com/documentation/web-api/reference/tracks/get-audio-analysis/
        [HttpGet("audio-analysis/{id}"), ActionName("Get_Tracks_Audio_Analysis_Object")]
        public IActionResult GetTrackAudioAnalysis(string id) {
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/tracks/get-audio-features/
        [HttpGet("audio-features/{id}"), ActionName("Get_Tracks_Audio_Features")]
        public IActionResult GetTrackAudioFeatures(string id) {
            return NoContent();
        }


        // https://beta.developer.spotify.com/documentation/web-api/reference/tracks/get-several-audio-features/
        [HttpGet("audio-features"), ActionName("Get_Several_Tracks_Audio_Features")]
        public IActionResult GetSeveralAudioFeatures(string ids) {
            int maxItems = 100;
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/tracks/get-track/
        [HttpGet("tracks/{id}"), ActionName("Get_A_Track")]
        public IActionResult GetATrack(string id, string market = defaultMarket) {
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/tracks/get-several-tracks/
        [HttpGet("tracks"), ActionName("Get_Several_Tracks")]
        public IActionResult GetSeveralTracks(string ids, string market = defaultMarket) {
            int maxItems = 50;
            return NoContent();
        }

        #endregion

        #region User Endpoints
        // https://beta.developer.spotify.com/documentation/web-api/reference/users-profile/get-current-users-profile/
        [HttpGet("me"), ActionName("Get_Current_Users_Profile")]
        public IActionResult GetCurrentUserProfile() {
            return NoContent();
        }

        // https://beta.developer.spotify.com/documentation/web-api/reference/users-profile/get-users-profile/
        [HttpGet("users/{user_id}"), ActionName("Get_A_Users_Profile")]
        public IActionResult GetUserProfile(string user_id) {
            return NoContent();
        }
        #endregion

    }
}
