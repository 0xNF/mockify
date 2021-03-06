# Mockify - A Replica Spotify Server

## Feature Completeness: 50%. Do not attempt to use yet. 

Mockify attempts to be a drop-in replacement for any applications that want to test their compliance, performance, and resilience against the Spotify Web API. Features include:

1) Self hosted and totally offline<sup><a href="#offline-caveat">1</a></sup>
2) Full Spotify API parity (as of March 14 2018)<sup><a href="#parity-caveat">2</a></sup>
3) Full Spotify Object Model parity
4) No worrying about Spotify rate limits while testing
5) Ability to control server side errors
6) Ability to test against token revoke and rate limiting

Mockify allows you to create users and OAuth Applications, and also models Spotify's OAuth2 log in process, allowing all 3 supported authentication flows.

Each Spotify endpoint is present and returns the same type of data the live web API would return. Paging objects, album objects, playlist objects, everything behaves as expected. 

Most importantly, Mockify allows you to issue server-side errors, such as 502, 503, timeouts, and rate limiting, allowing you to test your application's resilience without trying to compel that behavior from Spotify itself.

Mockify is a .NET Core 2.0 application, and launches a webserver at https://localhost:44345.
Mockify requires you to accept a temporary https certificate. If you do not wish to accept this certificate, then you cannot currently use Mockify. A non-https version is not currently planned.

Presently, subdomains are not supported. <br>
For https://accounts.spotify.com/authorize/ requests, the Mockify counterpart is https://localhost:44345/authorize/<br>
For https://api.spotify.com/v1/ requests, the Mockify counterpart is https://localhost:44345/api/v1/

Otherwise the url scheme is the same. For instance, To request an album, instead of hitting https://api.spotify.com/v1/albums/{id}, you hit https://localhost:44345/api/v1/albums/{id}


## Implementation Todo List
Although this is currently in working order as a starting point for anyone attempting to set up an OpenIdConnectServer OAuth2 Server, its ability to serve as a Spotify integration test suite is not yet implemented. What follows is what is left to do before it can be used fully:

* Collect dummy Spotify data for each endpoint
* Create unit tests
* api.localhost and accounts.localhost subdomains
* More thorough administrative control panels
* Rate limiting is not active
* Server Response Modes are not active
* CLI methods for adjusting rate limits and response modes

## Building
Mockify depends on .NET Core 2.0.  
Please see Microsoft's download and installation instructions [here](https://www.microsoft.com/net/download) before proceeding.

```
$ git pull https://github.com/0xnf/mockify.git
$ cd mockify
$ dotnet build
$ dotnet ef database update
```

### Development Notes

If you make changes to the Database Context (`MockifyDbContext.cs`), please be sure to delete the following files:  
* `Mockify\Mockify\Data\2018030218752_n3.cs` (Delete whatever matches the pattern. the actual file name is a generated timestamp and will differ for you upon each build)
* `Mockify\Mockify\Data\MockifyDbContextModelSnapshot.cs`
* `Mockify\Mockify\Mockify.db`

SQLite does not presently support a number of EF Core migration options, meaning that any migrations must be generated from scratch, thus necessitating the deletion of all the above files, including the Database itself. 


### Footnotes
#### offline caveat
If you decide to use the Facebook authentication flow for a user, you will need to allow Mockify access to the external internet. 
#### parity caveat
Endpoints regarding web-playback, connected devices, and random seeding for playlists are not in the scope of this project