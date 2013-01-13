using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.SelfHost;
using log4net; 

namespace CortexWebApiServer
{
    //---   Self hosting WebAPI code taken from sample at
    //---   http://code.msdn.microsoft.com/ASPNET-Web-API-Self-Host-30abca12
    //---   and
    //---   http://www.hanselman.com/blog/TinyHappyFeatures2ASPNETWebAPIInVisualStudio2012.aspx

    class Program
    {
        static readonly Uri _baseAddress = new Uri( "http://localhost:50231/" ); 

        static CortexInterface  cortex_;
        static ILog log_;

        static void Main( string[] args )
        {
            //---   Initialise log4net

            log4net.Config.XmlConfigurator.Configure();

            log_ = log4net.LogManager.GetLogger( typeof( Program ) );

            //---   Create the interface to Cortex

            cortex_ = CortexInterface.Instance;

            //---   Self host the WebAPI

            HttpSelfHostServer server = null;

            try
            {
                //---   Set up server configuration

                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration( _baseAddress );

                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{object_id}/{port_id}",
                    defaults: new { object_id = RouteParameter.Optional, port_id = RouteParameter.Optional }
                );

                //---   Create the server

                server = new HttpSelfHostServer( config );

                //---   Start listening

                server.OpenAsync().Wait();
                log_.Info( "Listening on " + _baseAddress + " Hit ENTER to exit..." );
                Console.ReadLine();
            }
            catch ( Exception e )
            {
                log_.Error( "Could not start server: " + e.GetBaseException().Message );
                log_.Error( "Hit ENTER to exit..." );
                Console.ReadLine();
            }
            finally
            {
                if ( server != null )
                {
                    //---   Stop listening 
                    server.CloseAsync().Wait();
                }
            }
        }
    } 
}
