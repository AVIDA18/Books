using System.Data.SqlClient;
using System.Data;
using Books_api.AppLogics;
using Books_api.Models;

namespace Books_api.Data
{
    public class LoginClass
    {
        private readonly DatabaseOperationClass _dbOperation;
        public LoginClass(DatabaseOperationClass dbOperation)
        {
            _dbOperation = dbOperation;
        }

        public async Task<ListUserData?> GetUserData(string userName)
        {
            var connectionString = _dbOperation.GetConnectionString();

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new("sp_GetUserDataByUserName", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserName", userName);

            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                ListUserData result = new()
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    UserName = Convert.ToString(reader["UserName"]),
                    Password = Convert.ToString(reader["Password"]),
                    RoleType = Convert.ToInt32(reader["RoleType"]),
                    Status = Convert.ToBoolean(reader["Status"])
                };

                return result;
            }

            return null;
        }


        public async Task<string> ChangePassword(ChangePassword parameters)
        {
            try
            {
                var connectionString = _dbOperation.GetConnectionString();

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                using SqlCommand command = new("sp_ChangePassword", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", parameters.UserId);
                command.Parameters.AddWithValue("@Password", parameters.Password);

                string result = (await command.ExecuteScalarAsync())?.ToString() ?? "";

                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
