PropOhGate
==========
Use WebSockets to propagate a collection of data from a parent to a child repository

It's a relatively simple POC to see if I could stream data using WebSockets. I also wanted to play with the idea of only streaming the bear minimum of data, so when a value changed I didn't want to serialise an entire row and send that down the wire. Instead this POC only sends updates to values. It's seems to work quite well.

I haven't put a lot of time into testing it and I haven't handled error scenarios and edge cases etc. There are a lot of improvements that could be made but as POCs go it's job done.

To Run
------
Simply start the HousingData.Server console application and then start the HousingData.Client UI. You'll have to run one through F5 and then right click -> Debug -> Start new instance on the other.

Packages
--------
- RX: I've used RX for streaming any events.
- Fleck: Used for the WebSocket server.
- WebSocket4Net: Used for the WebSocket client.
- MVVM Light: Used in the VERY simple WPF example UI.

The Data
--------
The data is currently being downloaded (when you run the server) from gov.uk. Please let me know if this starts failing because of an invalid uri and I can try and update any configuration details.