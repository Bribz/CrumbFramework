using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CrumbServer.Database;
using CrumbShared;
using LiteNetLib;

namespace CrumbServer
{
    public class CLoginServer : CServer
    {
        private DatabaseConnection m_DatabaseConnection;

        public CLoginServer()
        {
            m_DatabaseConnection = new LocalDatabaseConnection();
        }

        public override void Start()
        {

        }

        public override void Stop()
        {

        }

        protected override void network_error(IPEndPoint endPoint, SocketError socketError)
        {

        }

        protected override void log_msg(LogLevel level, string msg)
        {

        }

        protected override void latency_update(NetPeer peer, int latency)
        {

        }

        protected override void peer_disconnected_event(NetPeer peer, DisconnectInfo disconnectInfo)
        {

        }

        protected override void peer_connected_event(NetPeer peer)
        {

        }

        protected override void connection_request_event(ConnectionRequest request)
        {

        }

        protected override void receive_unconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {

        }

        protected override void receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketType type = (PacketType) reader.GetByte();
            switch (type)
            {
                case PacketType.LoginRequest:

                    break;
                case PacketType.CharacterSelection:

                    break;
                default:
                    break;
            }
        }
    }
}
