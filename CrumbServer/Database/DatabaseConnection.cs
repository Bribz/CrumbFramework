using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrumbServer.Database
{
    public abstract class DatabaseConnection
    {
        public DatabaseConnection()
        {

        }

        protected abstract void Initialize();

        protected abstract bool OpenConnection();

        protected abstract bool CloseConnection();

        public virtual void PollAccount(string accountHash, string passwordHash, Action<CrumbShared.LoginResponse> callback) { }

        public virtual void PollAccountCharacters(ulong accountID, Action<List<CrumbShared.CharacterData>> callback) { }

        public virtual void PollCharacterData(ulong characterID, Action<CrumbShared.CharacterData> callback) { }


    }
}
