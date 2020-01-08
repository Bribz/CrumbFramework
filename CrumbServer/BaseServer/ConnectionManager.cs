using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;

namespace CrumbServer
{
    public class ConnectionManager
    {
        public List<ClientConnection> ConnectedPeers;

        public ConnectionManager()
        {
            ConnectedPeers = new List<ClientConnection>();
        }

        public void Add(ClientConnection connection)
        {
            ConnectedPeers.Add(connection);
        }

        public void Remove(NetPeer peer)
        {
            var connection = ConnectedPeers.FirstOrDefault(p => p.Peer.Id == peer.Id);
            if (connection != null)
            {
                ConnectedPeers.Remove(connection);
            }
        }
    }
}
