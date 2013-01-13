using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml;

namespace CortexWebApiServer
{
    public class ObjectsController : ApiController
    {
        //---   GET /api/Objects
        //---   Gets all objects exposed by the Cortex API

        public HttpResponseMessage Get()
        {
            string json = CortexInterface.Instance.SendCommandJson( "<CortexAPI><APIRequestInfo><APIListCommand>Objects</APIListCommand></APIRequestInfo></CortexAPI>" );

            return new HttpResponseMessage() { Content = new StringContent( json, System.Text.Encoding.UTF8, "application/json" ) };
        }
    }
}
