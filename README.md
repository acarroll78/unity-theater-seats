# unity-theater-seats

A visual representation of a movie theater seat reservation system that includes a front end written in Unity and a C# backend.

## Supported platforms
win64

## Running the app
1. Start Server via the executable, wait for the console log to print a "Server started on address" message
2. On the same PC as the server, start one or more instances of the client
3. Select a name to "Login" then follow the on-screen instructions to reserve seats

## Notes
The server currently listens on localhost port 3278, ensure this port isn't blocked or otherwise busy.

Most of the sockets/routes don't stay connected for long, but the sockets which are responsible for sending reservation requests/responses stay open.

Websockets were chosen for bi-directional communication. For speed of implementation, applications use the following websocket libraries.

client: NativeWebSocket (endel/nativewebsocket on github) - a native solution that has an underlying JavaScript WebSocket implementation (for WebGL compatibility)

server: websocket-sharp (sta/websocket-sharp on github) - a well-tested (though sadly deprecated) websocket library for .NET

Data is transmitted in JSON format for ease of use/testing. System.Text.Json was included in both the client and server.

A shared class library project exists in the "shared" folder. This contains all of the data types that were leveraged by both the client and server.
The resultant .dll exists in the /lib folder of the server and the /Assets/Plugins folder of the client.

Client UI was implemented using UIToolkit.
