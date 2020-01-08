using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;

namespace CrumbServer.GameServer
{
    public class WaitingConnectionRequest
    {
        public ulong AccountID;
        public ulong CharacterID;
        public ConnectionRequest ConnectionRequest;

        public WaitingConnectionRequest(ulong accID, ulong charID, ConnectionRequest connectionRequest)
        {
            AccountID = accID;
            CharacterID = charID;
            ConnectionRequest = connectionRequest;
        }
    }
}
