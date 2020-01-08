using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrumbShared;

namespace CrumbServer.Database
{
    public class LocalDatabaseConnection : DatabaseConnection
    {
        public ulong CurrentAccountID = 0;
        public ulong CurrentCharacterID = 0;

        public LocalDatabaseConnection()
        {
            Initialize();
        }
        
        protected override void Initialize()
        {

        }

        protected override bool OpenConnection()
        {
            return true;
        }

        protected override bool CloseConnection()
        {
            return true;
        }

        public override void PollAccount(string accountHash, string passwordHash, Action<LoginResponse> callback)
        {
            if (OpenConnection() == true)
            {
                var LoginResponse = new LoginResponse();
                LoginResponse.AccountID = CurrentAccountID++;
                LoginResponse.Response = true;
                callback.Invoke(LoginResponse);

                CloseConnection();
            }
        }

        public override void PollAccountCharacters(ulong accountID, Action<List<CharacterData>> callback)
        {

        }

        public override void PollCharacterData(ulong characterID, Action<CharacterData> callback)
        {
            if (OpenConnection() == true)
            {
                CharacterData data = new CharacterData();
                data.Name = "TestName";

                data.CharacterID = characterID++;

                callback.Invoke(data);

                CloseConnection();
            }
        }
    }
}
