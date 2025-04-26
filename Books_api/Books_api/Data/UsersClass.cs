using Books_api.AppLogics;
using System.Data.SqlClient;
using System.Data;
using Books_api.Models;

namespace Books_api.Data
{
    public class UsersClass
    {
        private readonly DatabaseOperationClass _dbOperation;
        public UsersClass(DatabaseOperationClass dbOperation)
        {
            _dbOperation = dbOperation;
        }

        public async Task<string> RegisterUsers(UsersParameters parameters)
        {
            try
            {
                var connectionString = _dbOperation.GetConnectionString();

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                using SqlCommand command = new("sp_RegisterUser", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", parameters.UserId);
                command.Parameters.AddWithValue("@UserName", parameters.UserName);
                command.Parameters.AddWithValue("@FullName", parameters.FullName);
                command.Parameters.AddWithValue("@Password", parameters.Password);
                command.Parameters.AddWithValue("@RoleType", parameters.RoleType);
                command.Parameters.AddWithValue("@status", parameters.Status);

                string result = (await command.ExecuteScalarAsync())?.ToString() ?? "";

                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> SetUsersRole(UsersRoleSetParameters parameters)
        {
            try
            {
                var connectionString = _dbOperation.GetConnectionString();

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                using SqlCommand command = new("sp_RegisterUser", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", parameters.UserId);
                command.Parameters.AddWithValue("@RoleType", parameters.RoleType);
                command.Parameters.AddWithValue("@status", parameters.Status);

                string result = (await command.ExecuteScalarAsync())?.ToString() ?? "";

                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<List<Users>> SelectAllUsers(ListUsersParameter parameters)
        {
            var connectionString = _dbOperation.GetConnectionString();
            List<Users> userList = new();

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new("sp_GetAllUsers", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@RoleType", parameters.RoleType);
            command.Parameters.AddWithValue("@Status", parameters.Status);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    userList.Add(new Users
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        UserName = Convert.ToString(reader["UserName"]),
                        FullName = Convert.ToString(reader["FullName"]),
                        Password = Convert.ToString(reader["Password"]),
                        RoleType = Convert.ToInt32(reader["RoleType"]),
                        Status = Convert.ToBoolean(reader["Status"]),

                    });
                }
            }
            return userList;
        }
    }
}
