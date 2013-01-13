CortexWebApi Readme
===================

This project is a simple self-hosted WebApi server that exposes the Idratek Cortex XML API as a JSON WebApi.  
Such a facility is of use to simple clients (e.g. Android) that do not want to be burdened with XML.

Compilation
-----------

The solution is for Visual Studio 2012 and built using .Net 4.0 (chosen so that it will run on XP machines that do not support .Net 4.5)
Note that Nuget package restore must be enabled : see http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages

Usage
-----

Run the compiled server from a command prompt.  Currently the endpoint is hardcoded as http://localhost:50231/

The use of a REST client (e.g. the Postman Google Chrome add in) is recommended for testing.
The following API endpoints are exposed :

	http://localhost:50231/api/Objects
		GET		Returns a list of Cortex objects
		
	http://localhost:50231/api/Properties/123	
		GET		Returns a list of the properties for Cortex object 123
		
	http://localhost:50231/api/Ports/123
		GET		Returns a list of the ports for Cortex object 123

	http://localhost:50231/api/Ports/123/456
		GET		Returns information about port 456 of Cortex object 123
		PUT		Sets port 456 of Cortex object 123 : an optional 'value' parameter can be specified to pass to the object/port
		
ToDo
----

Support object and port identification by name as well as id (perhaps by accepting a JSON payload instead of url parameters)

Better handling of multiple clients : implement a queue for command responses (consider IObservable<>).  

Deal with the possible race condition whereby a command is issued but a port changes before the command response is sent by Cortex - in this
case the port change message will be returned as the response which is not correct.  Not sure about how much parallelism there is in Cortex...

Support XML as well as JSON for clients that need it

Push notification (e.g. using http://signalr.net/) for port change notification


