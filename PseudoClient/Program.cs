using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrumbClient;

namespace PseudoClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = $"PseudoClient_{CrumbShared.Version.Stub}";
            CrumbShared.Log.ChangeLogLevel(CrumbShared.LogLevel.DEBUG_VERBOSE);

            CClient client = new CClient();
            client.Connect();

        }
    }
}
