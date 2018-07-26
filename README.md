# RemotePlus An extendable remote control server.
## What is RemotePlus?
RemotePlus is a remote control server/client. You run the server on a remote computer, and you run a client to execute commands on the server.
### What features comes with RemotePlus?
* Flexable extension system.
* Customizable settings
* The ability to write your own client.
* Uploading and downloading files.
* Flexible scripting engine using IronPython
## Setting up RemotePlus
### Prerequisites
* .Net 4.6.2
* At least one network adapter.
* Recommended but not required, your network adapter is configured with a static IP address.
* Not scared of the command line. You'll be using the command line a lot.
* If there was a problem starting up the server, the following would happen (debug mode):
```
Welcome to RemotePlusServer, version: 1.0.0.0


Starting server core to setup and initialize services.
7/26/2018 4:23:27 PM [37:AddServices - Debug]: Building endpoint URL.
7/26/2018 4:23:27 PM [37:AddServices - Debug]: URL built net.tcp://brianVM:9000/Remote
7/26/2018 4:23:27 PM [37:AddServices - Debug]: Creating server.
7/26/2018 4:23:27 PM [37:AddServices - Debug]: Publishing server events.
7/26/2018 4:23:27 PM [37:AddServices - Debug]: Binding configurations:

MaxBufferPoolSize: 9223372036854775807
MaxBufferSize: 2147483647
MaxReceivedMessageSize: 2147483647

7/26/2018 4:23:27 PM [41:AddServices - Debug]: Attaching server events.
7/26/2018 4:23:27 PM [53:AddServices - Info]: Adding file transfer service.
7/26/2018 4:23:27 PM [64:InitializeKnownTypes - Info]: Adding default known types.
7/26/2018 4:23:27 PM [66:InitializeKnownTypes - Debug]: Adding UserAccount to known type list.
7/26/2018 4:23:27 PM [229:LoadServerConfig - Info]: Loading server roles file.
7/26/2018 4:23:27 PM [263:LoadServerConfig - Info]: Loading server settings file.
7/26/2018 4:23:27 PM [271:LoadServerConfig - Error]: Unable to load server settings. System.Runtime.Serialization.SerializationException: Error in line 8 position 18. Expecting state 'Element'.. Encountered 'Text'  with name '', namespace ''.
   at System.Runtime.Serialization.XmlObjectSerializerReadContext.HandleMemberNotFound(XmlReaderDelegator xmlReader, ExtensionDataObject extensionData, Int32 memberIndex)
   at System.Runtime.Serialization.XmlObjectSerializerReadContext.GetMemberIndex(XmlReaderDelegator xmlReader, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces, Int32 memberIndex, ExtensionDataObject extensionData)
   at ReadDiscoverySettingsFromXml(XmlReaderDelegator , XmlObjectSerializerReadContext , XmlDictionaryString[] , XmlDictionaryString[] )
   at System.Runtime.Serialization.ClassDataContract.ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
   at System.Runtime.Serialization.XmlObjectSerializerReadContext.ReadDataContractValue(DataContract dataContract, XmlReaderDelegator reader)
   at System.Runtime.Serialization.XmlObjectSerializerReadContext.InternalDeserialize(XmlReaderDelegator reader, String name, String ns, Type declaredType, DataContract& dataContract)
   at System.Runtime.Serialization.XmlObjectSerializerReadContext.InternalDeserialize(XmlReaderDelegator xmlReader, Int32 id, RuntimeTypeHandle declaredTypeHandle, String name, String ns)
   at ReadServerSettingsFromXml(XmlReaderDelegator , XmlObjectSerializerReadContext , XmlDictionaryString[] , XmlDictionaryString[] )
   at System.Runtime.Serialization.ClassDataContract.ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
   at System.Runtime.Serialization.XmlObjectSerializerReadContext.ReadDataContractValue(DataContract dataContract, XmlReaderDelegator reader)
   at System.Runtime.Serialization.XmlObjectSerializerReadContext.InternalDeserialize(XmlReaderDelegator reader, String name, String ns, Type declaredType, DataContract& dataContract)
   at System.Runtime.Serialization.XmlObjectSerializerReadContext.InternalDeserialize(XmlReaderDelegator xmlReader, Type declaredType, DataContract dataContract, String name, String ns)
   at System.Runtime.Serialization.DataContractSerializer.InternalReadObject(XmlReaderDelegator xmlReader, Boolean verifyObjectName, DataContractResolver dataContractResolver)
   at System.Runtime.Serialization.XmlObjectSerializer.ReadObjectHandleExceptions(XmlReaderDelegator reader, Boolean verifyObjectName, DataContractResolver dataContractResolver)
   at System.Runtime.Serialization.DataContractSerializer.ReadObject(XmlReader reader)
   at RemotePlusLibrary.Configuration.StandordDataAccess.ConfigurationHelper.LoadConfig[TConfigModel](String file) in C:\Source\Repos\RemotePlus\src\RemotePlusLibrary.Configuration\StandordDataAccess\ConfigurationHelper.cs:line 36
   at RemotePlusServer.Core.ServerCore.ServerBuilderExtensions.<>c.<LoadServerConfig>b__9_0() in C:\Source\Repos\RemotePlus\src\RemotePlusServer.Core\ServerCore\ServerBuilderExtensions.cs:line 266
7/26/2018 4:23:27 PM [79:Scripting Engine - Info]: Initializing functions and variables.
7/26/2018 4:23:27 PM [110:InitializeScriptingEngine - Info]: Starting scripting engine.
7/26/2018 4:23:28 PM [112:Scripting Engine - Info]: Engine started. IronPython version 2.7.8.0
7/26/2018 4:23:28 PM [113:Scripting Engine - Debug]: Redirecting STDOUT to duplex channel.
7/26/2018 4:23:28 PM [116:InitializeScriptingEngine - Info]: Finished starting scripting engine.
7/26/2018 4:23:28 PM [167:LoadExtensionLibraries - Info]: Loading extensions...
7/26/2018 4:23:28 PM [199:LoadExtensionLibraries - Info]: 0 extension libraries loaded.
7/26/2018 4:23:28 PM [130:InitializeVariables - Info]: Loading variables.
7/26/2018 4:23:28 PM [84:InitializeServer - Info]: Loading Commands.
7/26/2018 4:23:28 PM [101:CheckPrerequisites - Info]: Checking prerequisites.
7/26/2018 4:23:28 PM [53:CheckSettings - Info]: NOTE: Logging is enabled for this application.
7/26/2018 4:23:28 PM [106:CheckPrerequisites - Debug]: Stopping stop watch.
7/26/2018 4:23:28 PM [111:CheckPrerequisites - Error]: Unable to start server. (1 errors) Elapsed time: 00:00:00.6926099
```
### Setting up server
* Make sure that you meet the prerequisites mentioned above.
* Make sure that your `GlobalServerSettings.config` file matches the template below
``` xml
<?xml version="1.0" encoding="utf-8"?>
<ServerSettings xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/RemotePlusLibrary.Configuration.ServerSettings">
  <BannedIPs xmlns:d2p1="http://schemas.microsoft.com/2003/10/Serialization/Arrays" i:nil="true" />
  <DiscoverySettings>
    <ConnectToBuiltInProxyServer>true</ConnectToBuiltInProxyServer>
    <Connection>
      <ProxyServerURL>net.tcp://localhost:9001/Proxy</ProxyServerURL>
    </Connection>
    <DiscoveryBehavior>None</DiscoveryBehavior>
    <Setup>
      <DiscoveryPort>9001</DiscoveryPort>
      <ProxyClientEndpointName>ProxyClient</ProxyClientEndpointName>
      <ProxyEndpointName>Proxy</ProxyEndpointName>
    </Setup>
  </DiscoverySettings>
  <EnableMetadataExchange>false</EnableMetadataExchange>
  <LoggingSettings xmlns:d2p1="http://schemas.datacontract.org/2004/07/RemotePlusLibrary.Configuration">
    <d2p1:CleanLogFolder>false</d2p1:CleanLogFolder>
    <d2p1:DateDelimiter>45</d2p1:DateDelimiter>
    <d2p1:LogFileCountThreashold>10</d2p1:LogFileCountThreashold>
    <d2p1:LogFolder>ServerLogs</d2p1:LogFolder>
    <d2p1:LogOnShutdown>true</d2p1:LogOnShutdown>
    <d2p1:TimeDelimiter>45</d2p1:TimeDelimiter>
  </LoggingSettings>
  <PortNumber>9000</PortNumber>
</ServerSettings>
```
If you break your `GlobalServerSettings.config` file, delete it and restart RemotePlusServer. You should see the following lines of text from the console:
```
Welcome to RemotePlusServer, version: 1.0.0.0


Starting server core to setup and initialize services.
7/26/2018 4:25:34 PM [37:AddServices - Debug]: Building endpoint URL.
7/26/2018 4:25:34 PM [37:AddServices - Debug]: URL built net.tcp://brianVM:9000/Remote
7/26/2018 4:25:34 PM [37:AddServices - Debug]: Creating server.
7/26/2018 4:25:34 PM [37:AddServices - Debug]: Publishing server events.
7/26/2018 4:25:34 PM [37:AddServices - Debug]: Binding configurations:

MaxBufferPoolSize: 9223372036854775807
MaxBufferSize: 2147483647
MaxReceivedMessageSize: 2147483647

7/26/2018 4:25:34 PM [41:AddServices - Debug]: Attaching server events.
7/26/2018 4:25:34 PM [53:AddServices - Info]: Adding file transfer service.
7/26/2018 4:25:34 PM [64:InitializeKnownTypes - Info]: Adding default known types.
7/26/2018 4:25:34 PM [66:InitializeKnownTypes - Debug]: Adding UserAccount to known type list.
7/26/2018 4:25:34 PM [229:LoadServerConfig - Info]: Loading server roles file.
7/26/2018 4:25:34 PM [258:LoadServerConfig - Warning]: The server settings file does not exist. Creating server settings file.
7/26/2018 4:25:34 PM [79:Scripting Engine - Info]: Initializing functions and variables.
7/26/2018 4:25:34 PM [110:InitializeScriptingEngine - Info]: Starting scripting engine.
7/26/2018 4:25:34 PM [112:Scripting Engine - Info]: Engine started. IronPython version 2.7.8.0
7/26/2018 4:25:34 PM [113:Scripting Engine - Debug]: Redirecting STDOUT to duplex channel.
7/26/2018 4:25:34 PM [116:InitializeScriptingEngine - Info]: Finished starting scripting engine.
7/26/2018 4:25:34 PM [167:LoadExtensionLibraries - Info]: Loading extensions...
7/26/2018 4:25:34 PM [199:LoadExtensionLibraries - Info]: 0 extension libraries loaded.
7/26/2018 4:25:34 PM [130:InitializeVariables - Info]: Loading variables.
7/26/2018 4:25:34 PM [84:InitializeServer - Info]: Loading Commands.
7/26/2018 4:25:34 PM [101:CheckPrerequisites - Info]: Checking prerequisites.
7/26/2018 4:25:34 PM [53:CheckSettings - Info]: NOTE: Logging is enabled for this application.
7/26/2018 4:25:34 PM [106:CheckPrerequisites - Debug]: Stopping stop watch.
7/26/2018 4:25:34 PM [121:CheckPrerequisites - Warning]: The server can start, but with warnings. (1 warnings) Elapsed time: 00:00:00.6140086
```
## Commands
So, you want to know the commands that are pre-loaded on the server? Type `help` in the client console for a list of commands.
## I want to build my own extension
Please refer to the documentation for more details on how to write a server extension.
## So, I'm a noob. How do I get started?
Please refer to the quick start guide on how to setup and configure RemotePlus for your environment.
## What operations are exposed?
Please look at the `IRemote.cs` for your answer. If you want a more user friendly version, go to the documentation for your answer.
## troubleshooting (the short version)
### RemotePlusServer will not start
* Make sure that all prerequisites have been met.
* There may have been a problem (exception) that occured. Please refer to the list of exceptions in the troubleshooting guide for more support.
### Certain folders are missing
* Make sure that the `Configurations\Server` folder exists.
* Make sure that the `ServerLogs` folder exists.
* Make sure that the 'policyObjects' folder exists.
* Make sure that the 'Users' folder exists.
### Server Core Is Missing
If you receive the follwing error while starting up RemotePlusServer:
```
elcome to RemotePlusServer, version: 1.0.0.0


Starting server core to setup and initialize services.
FATAL ERROR: A server core is not present. Cannot start server.
```
Make sure that either 'DefaultServerCore.dll' is in the same directory as the server or make sure that the server core you downloaded is valid.
## To do (things that can be fixed or improved)
* Refactor messy code.
* Adding thread sync.
## Latest builds
If you want the latest, nightly builds, please go to http://52.173.240.155:8080/job/RemotePlusBuild/