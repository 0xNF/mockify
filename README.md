# Mockify - A Replica Spotify Server

## Feature Completeness: 50%. Do not attempt to use yet. 

Mockify attempts to be a drop-in replacement for any applications that want to test their compliance, performance, and resilience against the Spotify Web API. Features include:

1) Totally offline
2) Full Spotify API parity (all API's are present)
3) Full Spotify Object Model parity (All returned objects and headers are the same shape and contain the same information and encoding as Spotify)
4) No worrying about Spotify's rate limits 
5) Ability to control server side errors

Mockify allows you to create users and OAuth Applications, and also models Spotify's OAuth2 log in process, allowing all 3 authentication flows.

Each Spotify endpoint is present and returns the same type of data the live web API would return. Paging objects, album objects, playlist objects, everything behaves as expected. 

Most importantly, Mockify allows you to issue server-side errors, such as 502, 503, timeouts, and rate limiting, allowing you to test your application's resilience without trying to compel that behavior from Spotify itself.

Mockify is a .NET Core 2.0 application, and launches a webserver at https://localhost:44345.

Presently, subdomains are not supported. <br>
For https://accounts.spotify.com/authorize/ requests, the Mockify counterpart is https://localhost:44345/authorize/<br>
For https://api.spotify.com/v1/ requests, the Mockify counterpart is https://localhost:44345/api/v1/

Otherwise the url scheme is the same. For instance, To request an album, instead of hitting https://api.spotify.com/v1/albums/{id}, you hit https://localhost:44345/api/v1/albums/{id}


## Implementation Todo List
Although this is currently in working order as a starting point for anyone attempting to set up an OpenIdConnectServer OAuth2 Server, it's ability to serve as a Spotify integration test suite is not yet implemented. What follows is what is left to do before it can be used fully:

* Collect dummy Spotify data for each endpoint
* Create unit tests
* api.localhost and accounts.localhost subdomains
* More thorough administrative control panels

## Building
Mockify depends on .NET Core 2.0

```
$ git pull https://github.com/0xnf/mockify.git
$ cd mockify
$ dotnet build
$ dotnet ef database update
```

If you make changes to the Database Context (`MockifyDbContext.cs`), then be sure to delete the migration snapshot (something like `2018030218752_n3.cs`), the `MockifyDbContextModelSnapshot.cs` files under the `Mockify\Mockify\Data` folder, and the `Mockify.db` sqlite file. SQLite does not presently support a number of EF Core migration options, meaning that any migrations must be generated from scratch, thus necessitating the deletion of all the above files.

