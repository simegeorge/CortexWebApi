using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace CortexWebApiServer
{
    public class PortsController : ApiController
    {
        //---   GET /api/Ports/5
        //---   Gets all ports for the specified Cortex object

        public HttpResponseMessage Get( int object_id )
        {
            string json = CortexInterface.Instance.SendCommandJson( "<CortexAPI><APIRequestInfo><APIListCommand>Ports</APIListCommand><IDNumber>" + object_id + "</IDNumber></APIRequestInfo></CortexAPI>" );

            return new HttpResponseMessage() { Content = new StringContent( json, System.Text.Encoding.UTF8, "application/json" ) };
        }

        //---   GET /api/Ports/5/10
        //---   Gets the specified port for the specified Cortex object

        public HttpResponseMessage Get( int object_id, int port_id )
        {
            string json = CortexInterface.Instance.SendCommandJson( "<CortexAPI><QueryPortValue><IDNumber>" + object_id + "</IDNumber><PortNumber>" + port_id + "</PortNumber></QueryPortValue></CortexAPI>" );

            return new HttpResponseMessage() { Content = new StringContent( json, System.Text.Encoding.UTF8, "application/json" ) };
        }

        //---   PUT /api/Ports/5/10
        //---   Sets the specified port for the specified Cortex object

        public HttpResponseMessage Put( int object_id, int port_id )
        {
            string json = CortexInterface.Instance.SendCommandJson( "<CortexAPI><SetPort><IDNumber>" + object_id + "</IDNumber><PortNumber>" + port_id + "</PortNumber></SetPort></CortexAPI>" );

            return new HttpResponseMessage() { Content = new StringContent( json, System.Text.Encoding.UTF8, "application/json" ) };
        }

        //---   PUT /api/Ports/5/10
        //---   Sets the specified port for the specified Cortex object to the specified value

        public HttpResponseMessage Put( int object_id, int port_id, int value )
        {
            string json = CortexInterface.Instance.SendCommandJson( "<CortexAPI><SetPort><IDNumber>" + object_id + "</IDNumber><PortNumber>" + port_id + "</PortNumber><Value>" + value + "</Value></SetPort></CortexAPI>" );

            return new HttpResponseMessage() { Content = new StringContent( json, System.Text.Encoding.UTF8, "application/json" ) };
        }

        //public HttpResponseMessage Get( string name )
        //{
        //    string json = CortexInterface.Instance.SendCommandJson( "<CortexAPI><APIRequestInfo><APIListCommand>Ports</APIListCommand><FriendlyName>" + name + "</FriendlyName></APIRequestInfo></CortexAPI>" );

        //    return new HttpResponseMessage() { Content = new StringContent( json, System.Text.Encoding.UTF8, "application/json" ) };
        //}
    }
}
