# ADB Proxy

Allows access to ADB devices on a remote PC without any special installed on the client.

## What does it do?

This SW runs as a server on the same PC as where ADB devices are connected
It exposes each ADB device over TCP.

Clients can connect to the device with

```
adb connect ip:port
```

..after which they can access the device as if it was local.

For the client, this works similar to connecting to a device over wi-fi, however, the device connection between the ADB host and ADB  device is be wired.

[adb] => [proxy] => [adb host] => USB => [adb device]

### How does it really work?

#### ADB "Smartsockets"

The adb host (server) running on the PC listens on port 5037 using the "smartsockets" API.

This protocol is very simple, allowing access to services on the host, or on a device:

```
[hex4][data]
```

The header is a the length of the data, in ASCII hex representation as 4 digits. 
Example: if the payload is 8 bytes, the header is 0008 in ASCII.
The data is often text based. For example, to list the devices on the host, the payload would specify the host:devices service: 

```
[hex4]host:devices
```

#### ADB device protocol

The adbd running on a device communicates with a slightly more complex protocol. 
Commands such as OPEN, OKAY, REDY, CLSE, CNXN are sent where OPEN kind of corresponds to the service-path in "smartsockets".
There's a 24 byte header which contain identifiers allowing for multiplexing of several concurrent streams.

#### ADB client with a local device

1. The ADB client uses "smartsockets" to talk with the ADB host. 
2. The ADB host then communicates with a device over USB (or IP) using the "ADB device protocol".

Example:
```
adb -s ABCD getprop 
```

This will be use the smartsockets protocol to communicate with the ADB host.
1. A TCP connection will be established to localhost:5037
1. Packet 1 sent to the host: "The device service should be opened": 0010host:device:ABCD
1. Packet 2 sent to the host: "The shell service will be opened": shell:getprop
1. The adb host will respond with the output from the shell command, as text.

Internally, the ADB host will communicate over USB with commands such as OPEN, WRITE, CLSE.

#### ADB client with a remote device (over IP)

The ADB client works the same, but the ADB host sends the OPEN,WRITE,CLSE commands over IP instead of USB.

#### Proxy implementation

The proxy exposes an "ADB device protocol" API, translates this to the "smartsockets" protocol, and communicates with the local ADB host.



