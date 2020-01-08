using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrumbShared;
using CrumbShared.Objects;
using LiteNetLib;

namespace CrumbServer.GameServer
{
    public class GameConnection : ClientConnection
    {
        public ulong CharacterID;
        public int Latency;
        public ushort MostRecentACK;

        public GameConnection(NetPeer peer, ulong accID = 0, ulong charID = 0) : base(peer, accID)
        {
            CharacterID = charID;
            Latency = 0;
            MostRecentACK = 0;
        }
    }
}
