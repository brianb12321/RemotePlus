# RemotePlus An extendable remote control server.
## What is RemotePlus?
RemotePlus is a remote control server/client. You run the server on a remote computer, and you run a client to execute commands on the server.
### What features comes with RemotePlus?
* Flexable extension system.
* Customizable settings
## Setting up RemotePlus
### Prerequisites
* .Net 4.6.2
* At least one network adapter.
* Perferred but not required, your network adapter is configured with a static IP address.
* Not scared of the command line. You'll be using the command line a lot.
* If there was a problem starting up the server, the following would happen (release mode):
```
Welcome to RemotePlusServer, version: 1.0.0.0


6/5/2017 6:37:48 PM [Server Host][DEBUG]: Starting stop watch.
6/5/2017 6:37:48 PM [Server Host][INFO]: Adding default known types.
6/5/2017 6:37:48 PM [Server Host][DEBUG]: Adding UserAccount to known type list.
6/5/2017 6:37:48 PM [Server Host][INFO]: Loading commands.
6/5/2017 6:37:48 PM [Server Host][INFO]: Loading server settings file.
6/5/2017 6:37:48 PM [Server Host][ERROR]: Unable to load server settings. There was an error deserializing the object of type RemotePlusLibrary.ServerSettings. The 'Credentials' start tag on line 5 position 8 does not match the end tag of 'Credentdials'. Line 8, position 9.
6/5/2017 6:37:48 PM [Server Host][INFO]: Loading variables.
6/5/2017 6:37:48 PM [Server Host][INFO]: Initializing watchers.
6/5/2017 6:37:48 PM [Server Host][INFO]: Loading extensions...
6/5/2017 6:37:48 PM [Server Host][INFO]: The extensions folder does not exist.
6/5/2017 6:37:48 PM [Server Host][INFO]: Checking prerequisites.
6/5/2017 6:37:48 PM [Server Host][WARNING]: The current logged in user is not part of the group "Administrator". This may cause certain operations to fail.
6/5/2017 6:37:48 PM [Server Host][DEBUG]: Stopping stop watch.
6/5/2017 6:37:48 PM [Server Host][ERROR]: Unable to start server. (1 errors, 1 warnings) Elapsed time: 00:00:00.6824178
```
### Setting up server
* Make sure that you meet the prerequisites mentioned above.
* Make sure that your `GlobalSettings.config` file matches the template below
``` xml
<?xml version="1.0" encoding="utf-8"?>
<ServerSettings xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/RemotePlusLibrary">
  <Accounts>
    <anyType i:type="UserAccount">
      <Credentials>
        <Password>password</Password>
        <Username>admin</Username>
      </Credentials>
      <Role>
        <Privilleges>
          <CanAccessConsole>true</CanAccessConsole>
          <CanAccessSettings>true</CanAccessSettings>
          <CanBeep>true</CanBeep>
          <CanPlaySound>true</CanPlaySound>
          <CanPlaySoundLoop>true</CanPlaySoundLoop>
          <CanPlaySoundSync>true</CanPlaySoundSync>
          <CanRunExtension>true</CanRunExtension>
          <CanRunProgram>true</CanRunProgram>
          <CanRunWatcher>true</CanRunWatcher>
          <CanShowMessageBox>true</CanShowMessageBox>
          <CanSpeak>true</CanSpeak>
          <ExtensionRules xmlns:d6p1="http://schemas.microsoft.com/2003/10/Serialization/Arrays" />
        </Privilleges>
        <RoleName>Admin</RoleName>
      </Role>
    </anyType>
  </Accounts>
  <BannedIPs xmlns:d2p1="http://schemas.microsoft.com/2003/10/Serialization/Arrays" i:nil="true" />
  <CleanLogFolder>false</CleanLogFolder>
  <DisableCommandClients>false</DisableCommandClients>
  <LogFileCountThreashold>10</LogFileCountThreashold>
  <LogOnShutdown>true</LogOnShutdown>
  <PortNumber>9000</PortNumber>
</ServerSettings>
```
If you break your `GlobalServerSettings.config` file, delete it and restart RemotePlusServer. You should see the following lines of text from the console:
```
Welcome to RemotePlusServer, version: 1.0.0.0


6/5/2017 6:31:47 PM [Server Host][DEBUG]: Starting stop watch.
6/5/2017 6:31:48 PM [Server Host][INFO]: Adding default known types.
6/5/2017 6:31:48 PM [Server Host][DEBUG]: Adding UserAccount to known type list.
6/5/2017 6:31:48 PM [Server Host][INFO]: Loading commands.
6/5/2017 6:31:48 PM [Server Host][WARNING]: The server settings file does not exist. Creating server settings file.
```
## To do (things that can be fixed or improved)
* Refactor messy code.
* Adding thread sync.
## Latest builds
If you want the latest, nightly builds, please go to http://52.173.240.155:8080/job/RemotePlusBuild/