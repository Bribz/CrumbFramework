using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrumbServer;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = $"GameServer_{CrumbShared.Version.Stub}";
            CrumbShared.Log.ChangeLogLevel(CrumbShared.LogLevel.DEBUG_VERBOSE); 

            CGameServer server = new CGameServer();
            server.Start();
        }
    }
}
