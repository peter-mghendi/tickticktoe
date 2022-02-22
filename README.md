# tickticktoe
Real-time tic-tac-toe on Blazor Webassembly over ASP.NET 6 SignalR/websockets. OAuth2/OpenID Connect via Duende IdentityServer.

## Solution structure

The root of this repository contains a [Visual Studio solution (.sln)](https://github.com/sixpeteunder/tickticktoe/blob/master/tickticktoe.sln) that has 5 projects:


### [TickTickToe.Cli](https://github.com/sixpeteunder/tickticktoe/tree/master/src/TickTickToe.Cli)

.NET console application containing the command line client for TickTickToe (still work in progress).


### [TickTickToe.Core](https://github.com/sixpeteunder/tickticktoe/tree/master/src/TickTickToe.Core)

.NET class library containing shared code (domain models, mostly).


### [TickTickToe.Web.Client](https://github.com/sixpeteunder/tickticktoe/tree/master/src/TickTickToe.Web.Client)

Blazor Webassembly application, web client for TickTickToe (feature complete).


### [TickTickToe.Web.Shared](https://github.com/sixpeteunder/tickticktoe/tree/master/src/TickTickToe.Web.Shared)

Remnant from project setup. I keep it around because I think I might need it someday. Contains code shared betewen server and web client.


### [TickTickToe.Web.Server](https://github.com/sixpeteunder/tickticktoe/tree/master/src/TickTickToe.Web.Server)

All of the things:
- ASP.NET 6 web server.
- OAuth2/OIDC identity server.
- All of the database and Redis access.
- Pre-rendering (SSR) for the web client.
- SignalR hubs for the realtime functionality.
- [Planned] REST APIs for some planned features.


## Planned

### TickTickToe.App

.NET MAUI native application.
