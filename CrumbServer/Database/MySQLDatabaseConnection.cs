using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CrumbServer.Database
{
    public class MySQLDatabaseConnection : DatabaseConnection
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        private string m_ConnectionString;

        public MySQLDatabaseConnection()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            server = "localhost";
            database = "test";
            uid = "admin";
            password = "password";
            m_ConnectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password}";

            connection = new MySqlConnection(m_ConnectionString);
        }

        protected override bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch(MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        CrumbShared.Log.LogError($"MySQL Database Failed to connect to server!");
                        break;
                    case 1045:
                        CrumbShared.Log.LogError($"MySQL Database : Invalid username/password combination!");
                        break;
                    default:
                        CrumbShared.Log.LogError($"MySQL Database Error : {ex.Number}");
                        break;
                }
                return false;
            }
        }

        protected override bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                CrumbShared.Log.LogError($"MySQL Database failed to close connection : {ex.Message}");
                return false;
            }
        }
    }
}
