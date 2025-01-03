
using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Linq;

namespace AODb.Common
{
    internal class PRKController : IDbController
    {
        private string _rdbPath;
        private SQLiteConnection _connection;
        
        private Dictionary<int, Dictionary<int, ulong>> _records;
        
        public PRKController(string rdbPath)
        {
            _rdbPath = rdbPath;
            string connectionString = $"Data Source={_rdbPath};Version=3";
            _connection = new SQLiteConnection(connectionString);

            try
            {
                _connection.Open();
                LoadRecordTypes();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void LoadRecordTypes()
        {
            _records = new Dictionary<int, Dictionary<int, ulong>>();
            List<string> tableNames = GetTableNames();

            foreach (string name in tableNames)
            {
                int tableType = int.Parse(name.Split('_').Last());
                
                using var cmd = new SQLiteCommand(_connection);
                cmd.CommandText = $"SELECT id FROM {name}";
                using SQLiteDataReader reader = cmd.ExecuteReader();
                
                Dictionary<int, ulong> keys = new Dictionary<int, ulong>();

                while (reader.Read())
                {
                    keys.Add(reader.GetInt32(0), 0);
                }
                
                _records.Add(tableType, keys);
            }
        }

        private List<string> GetTableNames()
        {
            List<string> tableNames = new List<string>();
            
            using var command = new SQLiteCommand(_connection);
            command.CommandText = "SELECT * FROM sqlite_master WHERE type='table'";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tableNames.Add(reader.GetString(1));
            }
            return tableNames;
        }
        
        public byte[] Get(int type, int instance)
        {
            string tableName = $"rdb_{type}";
            byte[] data = new byte[0];
            string sql = $"SELECT version, data FROM {tableName} WHERE id = {instance}";
            using (SQLiteCommand command = new SQLiteCommand(sql, _connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data = data.Concat(BitConverter.GetBytes((uint)type)).ToArray();
                        data = data.Concat(BitConverter.GetBytes((uint)instance)).ToArray();
                        data = data.Concat(BitConverter.GetBytes((uint)1)).ToArray();
                        data = data.Concat((byte[])reader["data"]).ToArray();
                    }
                }
            }
            return data;
        }
        
        public void Put(uint type, uint record, byte[] data)
        {
            // string sql = $"INSERT INTO data (type, instance, data) VALUES ({type}, {record}, @data)";
            // using (SQLiteCommand command = new SQLiteCommand(sql, _connection))
            // {
            //     command.Parameters.Add(new SQLiteParameter("@data", data));
            //     command.ExecuteNonQuery();
            // }
            throw new NotImplementedException();
        }

        public Dictionary<int, Dictionary<int, ulong>> GetRecords()
        {
            return _records;
        }

        public void Dispose()
        {
            
        }
    }
}
