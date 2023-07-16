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
- **Computer PC address**. If your PC has dynamic assigned IP consider set it with a fixed one (prefered but not required). This will make a reliable way to always see your PC from any browser in your home network. This is recommened unless you have some sort of trusted DNS, which almost every home doesn't. Use the ```IPCONFIG``` command to get the IP address (IPv4).
- **A Free Port**. To get a list of all currently used ports execute the command ```NETSTAT -an```. Usually the port 8080 is free, but be sure is not listed in the command.

To tell Windows to receive communications for this app execute this command as an administrator:
```
netsh http add urlacl url=http://MyPcAddress:MyFreePort/ user=everyone listen=yes
```

Also, we need to tell the firewall to allow communication through the port we selected (8080 in this example):
```
netsh firewall add portopening TCP 8080 SimpleHttpPcControl enable ALL
```

The previous command:
- Permanently register the address in the url parameter as an acceptable incomming point of request.
- **MyPcAddress**: This is your PC IP address or name. Prefer the IP address. This IP was found with the command IPCONFIG.
- **MyFreePort**: (number) This port will be of exclusive use for the app. This number was found using the above command NETSTAT.
- **user**: You could leave this with "everyone" or use the name of the local user created (better).

In case you need to remove that configuration:
```netsh http delete urlacl url=http://MyPcAddress:MyFreePort/```

# App Configuration
The file ```data\config.json``` contains the configuration required for the app to work correctly.
- **UrlToListen**: This array contains a list of valid URLs used to communicate with this app. Usually one is enough, ex. ```http://192.168.0.2:8080/```. This must be **exactly** the URL and PORT used in the ```netsh``` command.
- **Actions**: Array with the possible actions presented to the user.
  - **Name**: the name of the action. In this first version this string cannot be changed.
  - **Title**: This is the title for the button that triggers this action.
  - **Enabled**: false (default). When true, this action is presented to the user. Hidden if false or ommited.
  - **ShellCommand**: The command executed for this action. Prefer full path to the executable. Environment variables are supported.
  - **ShellCommandArguments**: (optional) The parameters for the shell command.
# Verification
To test if everything is ok:
- Execute the file ```SimpleHttpPcControl.exe```.
- A command window opens and shows the text ```SimpleHttpPcControl is Listening...```.
- Open a browser in any device connected to the same network as your PC.
- Navigate to ```http://MyPcAddress:MyFreePort```.
- A page like this should be presented (this is an example address):

![image](https://github.com/Nestor-Arturo/SimpleHttpPcControl/assets/107557991/50afbb2a-97a0-4ffc-8357-db3b6281f4b9)

- Pressing the **Sleep** button puts the PC to sleep.
- Pressing the **Shutdown** button shutsdown the PC.
# Final Windows Configuration
If you want this to run on the PC start.
- Open the Task Scheduller.
- Add a task to run ```SimpleHttpPcControl.exe``` on system start.
- Recommended: Set this task to run user the local user created.
# Troubleshoting:
- For easy troubleshothing open a command prompt in the folder where you copied this app. In this command prompt execute ```SimpleHttpPcControl.exe``` instead of double click it. This way any error can be seen because the window does not close itself.
- The URLs used for this app must be real. You can get these using the ```IPCONFIG``` command and/or using the PC name (executing the command ```HOSTNAME```).
- All the URLs used must be real IPs/names for the PC, and all must be configured using the ```netsh``` command.
- For a first check you should use/register the localhost IP address (127.0.0.1) and call the app from the same PC (http://127.0.0.1:8080).
