# MLAPI.WebSockets
MLAPI.WebSockets is a wrapper library that can wrap around [websocket-sharp](https://github.com/sta/websocket-sharp) for native platforms or around the browsers JS interface for WebGL. Note that MLAPI.WebSockets is not an actual WebSocket implementation and will have all limitations of the two underlying wrapped implementations.

The implementation is based on [Unitys implementation](https://www.assetstore.unity3d.com/en/#!/content/38367e) and these articles: [Article 1](https://docs.unity3d.com/Manual/webgl-networking.html), [Article2](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html).

Server functionality is only avalible through the native implementation and cannot be ran in the web browser. This is due to security restrictions put in place by modern web browsers.