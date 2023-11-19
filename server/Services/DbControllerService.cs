using Npgsql;
using NpgsqlTypes;
using server.Models;
using CipherLibs.Ciphers;

namespace server.Services
{
    public class DbControllerService : IDisposable
    {
        private readonly NpgsqlDataSource _dataSource;

        private readonly NpgsqlCommand _insertCommand;
        private readonly NpgsqlCommand _deleteCommand;
        private readonly NpgsqlCommand _existsCommand;
        private readonly NpgsqlCommand _infoCommand;
        private readonly NpgsqlCommand _allUsersCommand;

        public DbControllerService(IConfiguration config)
        {
            var _connectionString = config.GetConnectionString("Db");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new DriveNotFoundException("Database is not configured.");
            }

            _dataSource = NpgsqlDataSource.Create(_connectionString);
            _insertCommand = _dataSource.CreateCommand("INSERT INTO users (Id, Name, Algorithm, Key) VALUES (@connId, @name, @cipher, @key)");
            _insertCommand.Parameters.Add(new NpgsqlParameter("@connId", NpgsqlDbType.Text));
            _insertCommand.Parameters.Add(new NpgsqlParameter("@name", NpgsqlDbType.Text));
            _insertCommand.Parameters.Add(new NpgsqlParameter("@cipher", NpgsqlDbType.Integer));
            _insertCommand.Parameters.Add(new NpgsqlParameter("@key", NpgsqlDbType.Text));

            _deleteCommand = _dataSource.CreateCommand("DELETE FROM users WHERE Id = @connId");
            _deleteCommand.Parameters.Add(new NpgsqlParameter("@connId", NpgsqlDbType.Text));

            _existsCommand = _dataSource.CreateCommand("SELECT EXISTS(SELECT 1 FROM users WHERE Id = @connId)");
            _existsCommand.Parameters.Add(new NpgsqlParameter("@connId", NpgsqlDbType.Text));

            _infoCommand = _dataSource.CreateCommand("SELECT Name, Algorithm, Key FROM users WHERE Id = @connId");
            _infoCommand.Parameters.Add(new NpgsqlParameter("@connId", NpgsqlDbType.Text));

            _allUsersCommand = _dataSource.CreateCommand("SELECT Id FROM users");
        }

        public async Task<bool> CreateUser(string connectionId, string name, int algorithm)
        {
            _insertCommand.Parameters["@connId"].Value = connectionId;
            _insertCommand.Parameters["@name"].Value = name;
            _insertCommand.Parameters["@key"].Value = CreateKey(algorithm);
            _insertCommand.Parameters["@cipher"].Value = algorithm;

            using (var cmd = _insertCommand)
            {
                await cmd.ExecuteNonQueryAsync();
            }
            return true;
        }

        public async Task<bool> RemoveUser(string connectionId)
        {
            _deleteCommand.Parameters["@connId"].Value = connectionId;

            using (var cmd = _deleteCommand)
            {
                await cmd.ExecuteNonQueryAsync();
            }
            return true;
        }

        public async Task<bool> CheckIfUserExists(string connectionId)
        {
            _existsCommand.Parameters["@connId"].Value = connectionId;

            using (var cmd = _existsCommand)
            {
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToBoolean(result);
            }
        }

        public ChatUser? GetUserInfo(string connectionId)
        {
            _infoCommand.Parameters["connId"].Value = connectionId;

            using (NpgsqlDataReader reader = _infoCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    var name = reader.GetString(0);
                    var algorithm = reader.GetInt32(1);
                    var key = Convert.FromBase64String(reader.GetString(2));
                    return new ChatUser(connectionId, name, algorithm, key);
                }
                else
                {
                    return null;
                }
            }
        }

        public List<ChatUser> GetCurrentUsers()
        {
            var list = new List<ChatUser>();

            using (NpgsqlDataReader reader = _allUsersCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetString(0).Trim();
                    list.Add(GetUserInfo(id));
                }
            }

            return list;
        }

        public List<ChatUser> GetCurrentUsersExceptCaller(string connId)
        {
            var list = new List<ChatUser>();

            using (NpgsqlDataReader reader = _allUsersCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetString(0).Trim();
                    if (!id.Equals(connId))
                    {
                        list.Add(GetUserInfo(id));
                    }
                }
            }

            return list;
        }

        private string? CreateKey(int algorithm)
        {
            // TODO: Get Algorithm Info from Redis Cache

            int _value = new Random().Next(1000, 1500);
            string? _key = null;

            if (algorithm == 3)
            {
                _key = Convert.ToBase64String(new XorCipher(_value).Key);
            }
            else if (algorithm == 4)
            {
                _key = Convert.ToBase64String(new Rc4Cipher(_value).Key);
            }
            else
            {
                throw new InvalidDataException("Invalid Algorithm Format");
            }

            return _key;
        }
        public void Dispose()
        {
            _insertCommand.Dispose();
            _deleteCommand.Dispose();
            _existsCommand.Dispose();
            _infoCommand.Dispose();
            _dataSource.Dispose();
        }
    }
}
