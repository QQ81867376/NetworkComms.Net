// This file has been autogenerated from parsing an Objective-C header file added in Xcode.

using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using NetworkCommsDotNet;

namespace ExamplesChat.iOS
{
	public partial class Settings : UIViewController
    {
        #region Private Fields
        /// <summary>
        /// The IP address of the master (server)
        /// </summary>
        static string _masterIPValue = "";

        /// <summary>
        /// The port of the master (server)
        /// </summary>
        static int _masterPortValue = 10000;

        /// <summary>
        /// The local name used when sending messages
        /// </summary>
        static string _localNameValue = "iphone";

        /// <summary>
        /// The type of connection currently being used for sending and receiving
        /// </summary>
        static ConnectionType _connectionTypeValue = ConnectionType.TCP;

        /// <summary>
        /// Boolean for recording if the local device is acting as a server
        /// </summary>
        static bool _localServerEnabled = false;
        #endregion

        #region Get & Set
        public static string MasterIPValue { get { return _masterIPValue; } private set { _masterIPValue = value; } }
        public static int MasterPortValue { get { return _masterPortValue; } private set { _masterPortValue = value; } }
        public static string LocalNameValue { get { return _localNameValue; } private set { _localNameValue = value; } }
        public static ConnectionType ConnectionTypeValue { get { return _connectionTypeValue; } private set { _connectionTypeValue = value; } }
        #endregion

        public Settings (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Remove the keyboard on a tap gesture
            var tap = new UITapGestureRecognizer();
            tap.AddTarget(() =>
            {
                this.View.EndEditing(true);
            });
            this.View.AddGestureRecognizer(tap);

            //Update the settings based on previous values
            LocalServerEnabled.SetState(_localServerEnabled, false);
            MasterIP.Text = MasterIPValue;
            MasterPort.Text = MasterPortValue.ToString();
            LocalName.Text = LocalNameValue;

            //Set the correct segment on the connection mode toggle
            ConnectionMode.SelectedSegment = (ConnectionTypeValue == ConnectionType.TCP ? 0 : 1);
        }

        #region Event Handlers
        /// <summary>
        /// Update the settings when the user goes back to the main interface
        /// </summary>
        /// <returns></returns>
        public override bool ResignFirstResponder()
        {
            //Parse the name, ip and port information
            LocalNameValue = LocalName.Text.Trim();
            MasterIPValue = MasterIP.Text.Trim();

            int port;
            int.TryParse(MasterPort.Text, out port);
            MasterPortValue = port;

            //Store if we are running a local server
            _localServerEnabled = LocalServerEnabled.On;

            //If connection mode has been changed we will update ConnectionTypeValue 
            bool connectionTypeChanged = false;
            if (ConnectionMode.SelectedSegment == 0 && ConnectionTypeValue == ConnectionType.UDP)
            {
                connectionTypeChanged = true;
                ConnectionTypeValue = ConnectionType.TCP;
            }
            else if (ConnectionMode.SelectedSegment == 1 && ConnectionTypeValue == ConnectionType.TCP)
            {
                connectionTypeChanged = true;
                ConnectionTypeValue = ConnectionType.UDP;
            }

            //If the local server is enabled and we have changed the connection type
            //The ending logic "|| (!TCPConnection.Listening() && !UDPConnection.Listening())" is to catch the first
            //enable of local server mode if the ConnectionTypeValue has not been updated
            if (_localServerEnabled && (connectionTypeChanged || (!TCPConnection.Listening() && !UDPConnection.Listening())))
            {
                //If we were previously listening we first shutdown comms.
                if (TCPConnection.Listening() || UDPConnection.Listening())
                {
                    ChatWindow.AppendLineToChatBox("Connection mode has been updated. Any existing connections have been closed.");
                    NetworkComms.Shutdown();
                }

                //Enable local server mode using the selected connection type
                ChatWindow.InitialiseComms(true, ConnectionTypeValue);
            }
            else if (!_localServerEnabled && (TCPConnection.Listening() || UDPConnection.Listening()))
            {
                //If the local server mode has been disabled but we are still listening we need to stop accepting incoming connections
                NetworkComms.Shutdown();
                ChatWindow.AppendLineToChatBox("Local server mode disabled. Any existing connections have been closed.");
            }

            return base.ResignFirstResponder();
        }
        #endregion
    }
}