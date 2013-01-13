using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace CortexWebApiServer
{
    public class PropertiesController : ApiController
    {
        //---   GET /api/Properties/5
        //---   Gets all properties for the specified Cortex object

        public HttpResponseMessage Get( int object_id )
        {
            string json = CortexInterface.Instance.SendCommandJson( "<CortexAPI><APIRequestInfo><APIListCommand>Properties</APIListCommand><IDNumber>" + object_id + "</IDNumber></APIRequestInfo></CortexAPI>" );

            return new HttpResponseMessage() { Content = new StringContent( json, System.Text.Encoding.UTF8, "application/json" ) };
        }

        //public HttpResponseMessage Get( string name )
        //{
        //    string json = CortexInterface.Instance.SendCommandJson( "<CortexAPI><APIRequestInfo><APIListCommand>Properties</APIListCommand><FriendlyName>" + name + "</FriendlyName></APIRequestInfo></CortexAPI>" );

        //    return new HttpResponseMessage() { Content = new StringContent( json, System.Text.Encoding.UTF8, "application/json" ) };
        //}
    }
}
