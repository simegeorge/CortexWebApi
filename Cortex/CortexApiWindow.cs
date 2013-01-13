using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CortexWebApiServer
{
    //---   This is the window that Cortex talks to
    //---   The code is pretty much lifted from the Cortex API C# sample

    public partial class CortexApiWindow : Form
    {
        //---   DLL imports

        [DllImport( "user32.dll" )]
        public static extern UInt32 RegisterWindowMessage( string lpString );

        [DllImport( "user32.dll" )]
        public static extern IntPtr SendMessage( IntPtr windowHandle, uint Msg, IntPtr wParam, IntPtr lParam );

        //---   Windows message values

        private uint WM_CortexControlAPIAttach;
        private uint WM_CortexControlAPIDiscover;
        private uint WM_COPYDATA = 0x004a;

        //---   Window handles

        private IntPtr HWND_BROADCAST = new IntPtr( -1 );
        private IntPtr cortex_handle_;

        //---   Whether we have opened communications with Cortex

        private bool cortex_active_ = false;

        //---   Data structure for sending data to/from Cortex

        private struct CopyDataStruct
        {
            public string ID;
            public int Length;
            public string Data;
        }

        //---   Logging

        private ILog log_;

        //---   Constructor

        private CortexApiWindow()
        {
            InitializeComponent();

            log_ = log4net.LogManager.GetLogger( typeof( CortexApiWindow ) );

            //---   Get Cortex Windows message IDs

            WM_CortexControlAPIAttach = RegisterWindowMessage( "CortexAPIAttach" );
            WM_CortexControlAPIDiscover = RegisterWindowMessage( "CortexAPIDiscover" );

            //---   Ping Cortex API

            log_.Debug( "> WM_CortexAPIDiscover" );

            SendMessage( HWND_BROADCAST, WM_CortexControlAPIDiscover, this.Handle, IntPtr.Zero );
        }

        //---   Singleton implementation

        private static readonly Lazy< CortexApiWindow > self_ = new Lazy<CortexApiWindow>( () => new CortexApiWindow() );

        public static CortexApiWindow Instance { get { return self_.Value; } }

        //---   SendCommand

        private string  cortex_response_ = "";
        private bool    awaiting_cortex_response_ = false;

        private static ManualResetEvent cortex_event_ = new ManualResetEvent( false );

        private bool SendCommandToCortex( string command )
        {
            if ( ! cortex_active_ )
                return false;

            log_.Debug( "> " + command );

            CopyDataStruct aCDS = new CopyDataStruct();
            aCDS.ID = "1";
            aCDS.Data = command;
            aCDS.Length = aCDS.Data.Length + 1;

            IntPtr lParam = Marshal.AllocCoTaskMem( Marshal.SizeOf( aCDS ) );

            Marshal.StructureToPtr( aCDS, lParam, false );

            IntPtr  aRetVal = SendMessage( cortex_handle_, WM_COPYDATA, this.Handle, lParam );

            Marshal.FreeCoTaskMem( lParam );

            return aRetVal != IntPtr.Zero;
        }

        public string SendCommand( string command )
        {
            //lock ( cortex_response_lock_ )
            {
                awaiting_cortex_response_ = true;

                bool    ok = SendCommandToCortex( command );

                //if ( ! ok )
                //    return "";

                cortex_event_.WaitOne();

                return cortex_response_;
            }
        }

        //---   WndProc

        protected override void WndProc( ref Message m )
        {
            //---   See what the message was

            UInt32 aMsg = (UInt32)m.Msg;

            if ( aMsg == WM_CortexControlAPIAttach )
            {
                if ( (UInt32)m.LParam == 1 )
                {
                    if ( (System.IntPtr)m.WParam != this.Handle )
                    {
                        log_.Debug( "< WM_CortexAPIAttach" );
                        cortex_handle_ = m.WParam;
                        cortex_active_ = true;
                    }
                }
                m.Result = new IntPtr( 1 );
                return;
            }
            else
            if ( aMsg == WM_CortexControlAPIDiscover )
            {
                if ( (UInt32)m.LParam == 1 )
                {
                    if ( (System.IntPtr)m.WParam != this.Handle )
                    {
                        cortex_handle_ = (System.IntPtr)m.WParam;
                        cortex_active_ = true;
                        log_.Debug( "< WM_CortexAPIDiscover" );
                        log_.Debug( "> WM_CortexAPIAttach" );
                        SendMessage( HWND_BROADCAST, WM_CortexControlAPIAttach, this.Handle, IntPtr.Zero );
                    }
                }
                m.Result = new IntPtr( 1 );
                return;
            }
            else
            if ( aMsg == WM_COPYDATA )
            {
                if ( m.WParam == cortex_handle_ )
                {
                    CopyDataStruct aCDS = (CopyDataStruct)m.GetLParam( typeof( CopyDataStruct ) );
                    string received_message = aCDS.Data;
                    HandleReceivedCortexMessage( received_message );
                    m.Result = new IntPtr( 1 );
                    return;
                }
                else
                {
                    base.WndProc( ref m );
                }
            }
            else
            {
                base.WndProc( ref m );
            }
        }

        //---   Handle messages received from Cortex

        private void HandleReceivedCortexMessage( string message )
        {
            //---   Filter out NetworkRunningState and Pulse messages

            if ( message.StartsWith( "Pulse=" ) ||                      // API v1
                 message.StartsWith( "NetworkRunningState=" ) ||
                 message.Contains( "<APIAction>Pulse" ) ||              // API v2
                 message.Contains( "<NetworkRunningState>" ) )
            {
                return;
            }

            log_.Debug( "< " + message );

            //lock ( cortex_response_lock_ )
            {
                if ( awaiting_cortex_response_ )
                {
                    cortex_response_ = message;
                    awaiting_cortex_response_ = false;

                    cortex_event_.Set();
                }
            }
        }
    }
}
