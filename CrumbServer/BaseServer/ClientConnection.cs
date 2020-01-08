using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;

namespace CrumbServer
{
    public class ClientConnection
    {
        public NetPeer Peer;
        public ulong AccountID;

        public ClientConnection(NetPeer peer, ulong accID = 0)
        {
            Peer = peer;
            AccountID = accID;
            //CharacterID = charID;
        }

        public void send_packet(byte[] packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            Peer.Send(packet, method);
        }
    }
}
