using fruixion.bebop.Exceptions;
using fruixion.bebop.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace fruixion.bebop
{
    /// <summary>
    /// Main Drone Controller Interface
    /// </summary>
    public class DroneManager
    {

        #region Parameters

        /// <summary>
        /// Drone IP Address
        /// </summary>
        public IPAddress DroneIp { get; set; }
        /// <summary>
        /// Drone Port
        /// </summary>
        public int DronePort { get; set; }

        #endregion Parameters


        #region Properties
        private int[] seq = new int[256];
        private UdpClient d2c_client;
        private MoveCmd _currentMove;
        #endregion Properties



        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public DroneManager()
        {
            DroneIp = IPAddress.Parse(Defaults.DeaultIpAddress);
            DronePort =Defaults.DeafultTcpPort;
            _currentMove = default(MoveCmd);
        }



        public DroneManager(IPAddress address, int port)
        {
            DroneIp = address;
            DronePort = port;
            _currentMove = default(MoveCmd);
        }


        #endregion Constructors


        #region Public Methods
        /// <summary>
        /// Discover Drones on this network
        /// </summary>
        /// <exception cref="fruixion.bebop.Exceptions.DiscoveryException">Discovery Error thrown when drones not found.</exception>
        /// <exception cref="fruixion.bebop.Exceptions.CommandException">Command exception thrown on command network error.</exception>
        public bool Discover() 
        {
            // create the UDP Connection
            d2c_client = new UdpClient(DroneIp.ToString(), 54321);

            // Make the default TCP connection to the drone for discovery
            TcpClient client = new TcpClient(DroneIp.ToString(), DronePort);
            NetworkStream nStrm = new NetworkStream(client.Client);
            StreamWriter strmWr = new StreamWriter(nStrm);
            StreamReader strmRd = new StreamReader(nStrm);

            string handshake = "{\"controller_type\":\"computer\", \"controller_name\":\"fruixion\", \"d2c_port\":\""+Defaults.DefraultD2CPort+"\", \"arstream2_client_stream_port\":\""+Defaults.DefaultStreamPort+"\", \"arstream2_client_control_port\":\""+Defaults.DeafultControlPort+"\"}";
            strmWr.WriteLine(handshake);
            strmWr.Flush();

            string recMsg = strmRd.ReadLine();
            if(recMsg == null)
            {
                return false;
            }
            else
            {
                generateAllStates();
                generateAllSettings();
                return true;
            }
        }

        /// <summary>
        /// Take Off
        /// </summary>
        public void TakeOff()
        {
            byte[] cmd = new byte[4];

            cmd[0] = Commands.ARCOMMANDS_ID_PROJECT_ARDRONE3;
            cmd[1] = Commands.ARCOMMANDS_ID_ARDRONE3_CLASS_PILOTING;
            cmd[2] = Commands.ARCOMMANDS_ID_ARDRONE3_PILOTING_CMD_TAKEOFF;
            cmd[3] = 0;

            SendCommand(cmd, 4, Commands.ARNETWORKAL_FRAME_TYPE_DATA_WITH_ACK, Commands.BD_NET_CD_ACK_ID);
        }

        /// <summary>
        /// Land
        /// </summary>
        public void Land()
        {
            byte[] cmd = new byte[4];
            cmd[0] = Commands.ARCOMMANDS_ID_PROJECT_ARDRONE3;
            cmd[1] = Commands.ARCOMMANDS_ID_ARDRONE3_CLASS_PILOTING;
            cmd[2] = Commands.ARCOMMANDS_ID_ARDRONE3_PILOTING_CMD_LANDING;
            cmd[3] = 0;

            SendCommand(cmd, 4, Commands.ARNETWORKAL_FRAME_TYPE_DATA_WITH_ACK, Commands.BD_NET_CD_ACK_ID);
        }



        public void Move(int flag, int roll, int pitch, int yaw, int gaz)
        {
            5
        }
        #endregion Public Methods


        #region Private Methods

        /// <summary>
        /// Send the Drone a command packet
        /// </summary>
        /// <remarks>
        /// Command Data Packet:
        /// -Data type (1 byte)
        /// - Target buffer ID (1 byte)
        /// - Sequence number (1 byte)
        /// - Total size of the frame (4 bytes, Little endian) 
        /// - Actual data (N bytes)
        /// 
        /// Actual Data Packet
        /// - Project or Feature ID (1 byte)
        /// - Class ID in the project/feature (1 byte)
        /// - Command ID in the class (2 bytes)
        /// </remarks>
        /// <exception cref="fruixion.bebop.Exceptions.CommandException">Command exception on message send failure</exception>
        private void SendCommand(byte[] cmd, int size, int type = Commands.ARNETWORKAL_FRAME_TYPE_DATA, int id = Commands.BD_NET_CD_NONACK_ID)
        {
            try
            {
                int bufSize = size + 7;
                byte[] buf = new byte[bufSize];

                seq[id]++;
                if (seq[id] > 255)
                    seq[id] = 0;


                buf[0] = (byte)type;
                buf[1] = (byte)id;
                buf[2] = (byte)seq[id];
                buf[3] = (byte)(bufSize & 0xff);
                buf[4] = (byte)((bufSize & 0xff00) >> 8);
                buf[5] = (byte)((bufSize & 0xff0000) >> 16);
                buf[6] = (byte)((bufSize & 0xff000000) >> 24);

                cmd.CopyTo(buf, 7);

                d2c_client.Send(buf, buf.Length);
            }catch(Exception es)
            {
                throw new CommandException(es.Message);
            }
        }



        private void generateAllStates()
        {
            byte[] cmd = new byte[4];
            cmd[0] = Commands.ARCOMMANDS_ID_PROJECT_COMMON;
            cmd[1] = Commands.ARCOMMANDS_ID_COMMON_CLASS_COMMON;
            cmd[2] = (Commands.ARCOMMANDS_ID_COMMON_COMMON_CMD_ALLSTATES & 0xff);
            cmd[3] = (Commands.ARCOMMANDS_ID_COMMON_COMMON_CMD_ALLSTATES & 0xff00 >> 8);

            SendCommand(cmd, 4, Commands.ARNETWORKAL_FRAME_TYPE_DATA_WITH_ACK, Commands.BD_NET_CD_ACK_ID);
        }


        private void generateAllSettings()
        {
            byte[] cmd = new byte[4];

            cmd[0] = Commands.ARCOMMANDS_ID_PROJECT_COMMON;
            cmd[1] = Commands.ARCOMMANDS_ID_COMMON_CLASS_SETTINGS;
            cmd[2] = (0 & 0xff);
            cmd[3] = (0 & 0xff00 >> 8);

            SendCommand(cmd, 4, Commands.ARNETWORKAL_FRAME_TYPE_DATA_WITH_ACK, Commands.BD_NET_CD_ACK_ID);
        }
        #endregion Private Methods
    }
}
