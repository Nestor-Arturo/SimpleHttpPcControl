# SimpleHttpPcControl
A Simple HTTP Server for Firing Custom Shell Commands in Windows Machines
# Background
My husband is like an inversed electromagnetic pulse: Wherever he enters all electrodomestics gets turned on, computers also. This little utility help me execute custom commands in any given PC in my house to, ex. shut it down. And I can do this by a simple web page I can see from my phone.
# Requirements
- Windows 10/11
- .Net 6.0
- About 20mb of disk space for executable
# Preparation
You may want this app to run when your computer starts. For this to work with the minimum permissions:
- Create a local user and login at least once. By default regular users can put the PC to sleep or shutdown.
- Even I've said this utility puts a PC to sleep or shutdown, it really can execute any arbitrary shell command (CMD). If used to run any command that requires elevated privileges then add this local user to the appropiate user group.
# Installation
- Create a folder and be sure it has permissions for the local user. 
- Uncompress the release file in this folder.
# Windows Configuration
By default Windows won't let unknown programs to receive connections. To let Windows know it's ok for the app we need to get a few info:
- Computer PC address. If your PC has dynamic assigned IP consider set it with a fixed one. This will make a reliable way to always see your PC from any browser in your home network. This is recommened unless you have some sort of trusted DNS, which almost every home doesn't. Use the ```IPCONFIG``` command to get the IP address (IPv4).
- A Free Port. To get a list of all currently used ports execute the command ```NETSTAT -an```. Usually the port 8080 is free, but be sure is not listed in the command.

To tell Windows to receive communications for this app execute this command as an administrator:
```
netsh http add urlacl url=http://MyPcAddress:MyFreePort/ user=everyone listen=yes
```

The previous command:
- Permanently register the address in the url parameter as an acceptable incomming point of request.
- **MyPcAddress**: This is your PC IP address or name. Prefer the IP address. This IP was found with the command IPCONFIG.
- **MyFreePort**: (number) This port will be of exclusive use for the app. This number was found using the above command NETSTAT.
- **user**: You could leave this with "everyone" or use the name of the local user created (better).
# App Configuration
The file ```data\config.json``` contains the configuration required for the app to work correctly.
- **UrlToListen**: This array contains a list of valid URLs used to communicate with this app. Usually one is enough, ex. ```http://192.168.0.2:8080/```. This must be exactly the URL and PORT used inthe ```netsh``` command.
