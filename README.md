# Broadcast
## Unity Versions:

Unity 2017.2.2f1

## Description:

PicoVR Broadcast Platform is developed by Pico Technology, which aims to provide synchronized multi-user experiences. In this solution, multiple VR standalone headsets will work as clients and PC/Pads as server. Server controls all clients all together using the same local network.

Pico Broadcast Platform consists of two parts: Server and Client.Open-sourced codes includes both parts,to watch the project effect,we need:

I.Copy the "pre resource" folder under the broadcast server to the root directory of the device system, if the folder already exists in the directory, merge the folder;
II. Copy broadcast_client.apk to the root directory of the system;
III. copy the application to be broadcast controlled to the root directory; (if the application is not broadcast controlled, ignore this step)

Note:The APK packaged on the client needs to be signed, and the signing account can be provided by contacting Pico technical support.

##

Server Usage:

Scene: Assets->Project->Scene->Server

Client Usage:

Scene: Assets->main






