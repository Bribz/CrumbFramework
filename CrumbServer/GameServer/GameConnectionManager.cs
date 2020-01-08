using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;

namespace CrumbServer.GameServer
{
    public class GameConnectionManager
    {
        public List<GameConnection> ConnectedClients;
        private Dictionary<int, GameConnection> PeerIDLookup;

        public GameConnectionManager()
        {
            ConnectedClients = new List<GameConnection>();
            PeerIDLookup = new Dictionary<int, GameConnection>();
        }

        public void Add(GameConnection connection)
        {
            ConnectedClients.Add(connection);
            PeerIDLookup.Add(connection.Peer.Id, connection);
        }

        public void Remove(NetPeer peer)
        {
            var connection = ConnectedClients.FirstOrDefault(p => p.Peer.Id == peer.Id);
            if (connection != null)
            {
                ConnectedClients.Remove(connection);
                PeerIDLookup.Remove(connection.Peer.Id);
            }
        }

        public GameConnection GetConnection(NetPeer peer)
        {
            return ConnectedClients.FirstOrDefault(p => p.Peer.Id == peer.Id);
        }

        public void latency_update(NetPeer peer, int latency)
        {
            var connection = ConnectedClients.Find(p => p.Peer.Id == peer.Id);
            if (connection != null)
            {
                connection.Latency = latency;
            }
        }

        public ushort get_recent_ack(NetPeer peer)
        {
            return PeerIDLookup[peer.Id].MostRecentACK;
        }

        public void set_recent_ack(NetPeer peer, ushort ack)
        {
            PeerIDLookup[peer.Id].MostRecentACK = ack;
        }
    }
}
