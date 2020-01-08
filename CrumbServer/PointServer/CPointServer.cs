using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CrumbShared;
using LiteNetLib;

namespace CrumbServer
{
    public class CPointServer : CServer
    {
        private readonly IPAddress LoginServerAddress = IPAddress.Parse("127.0.0.1");

        public CPointServer() : base()
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
            if (messageType == UnconnectedMessageType.BasicMessage)
            {
                m_NetManager.SendUnconnectedMessage(Encoding.UTF8.GetBytes(LoginServerAddress.Address.ToString()), remoteEndPoint);
            }
        }

        protected override void receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {

        }
    }
}
