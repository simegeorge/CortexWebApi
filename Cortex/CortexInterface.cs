using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace CortexWebApiServer
{
    public class CortexInterface : IDisposable
    {
        //---   Members

        private CortexApiWindow   cortex_window_;

        //---   Constructor

        private CortexInterface()
        {
            //---   Start a background thread to run the windows form that will communicate with Cortex

            using ( ManualResetEvent mre = new ManualResetEvent( false ) )
            {
                Thread t = new Thread( () =>
                {
                    cortex_window_ = CortexApiWindow.Instance;
                    cortex_window_.Text = "CortexApiWindow";
                    mre.Set();
                    Application.Run();
                } );

                t.Name = "CortexApi message loop";
                t.IsBackground = true;
                t.Start();

                //---   Wait until the window has been created
                mre.WaitOne();
            }
        }

        //---   Cortex interface

        public string SendCommandXml( string command )
        {
            //---   Accept and return XML

            return CortexApiWindow.Instance.SendCommand( command );
        }

        public string SendCommandJson( string command )
        {
            //---   Accept XML and return JSON

            string  response = CortexApiWindow.Instance.SendCommand( command );

            XmlDocument response_doc = new XmlDocument();

            response_doc.LoadXml( response );

            return JsonConvert.SerializeXmlNode( response_doc );
        }

        //---   Singleton implementation

        private static readonly Lazy< CortexInterface > self_ = new Lazy<CortexInterface>( () => new CortexInterface() );

        public static CortexInterface Instance { get { return self_.Value; } }

        //---   IDisposable implementation

        public void Dispose()
        {            
        }
    }
}
