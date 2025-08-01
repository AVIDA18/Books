using Books_api.AppLogics;
using System.Data.SqlClient;
using System.Data;

namespace Books_api.Data
{
    public class AdminClass
    {
        private readonly DatabaseOperationClass _dbOperation;
        public AdminClass(DatabaseOperationClass dbOperation)
        {
            _dbOperation = dbOperation;
        }

        public async Task AssignRoleToUserAsync(int userId, int roleId)
        {
            var connectionString = _dbOperation.GetConnectionString();
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("sp_AssignRoleToUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
            cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId });

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var connectionString = _dbOperation.GetConnectionString();
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("sp_AssignPermissionToRole", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId });
            cmd.Parameters.Add(new SqlParameter("@PermissionId", SqlDbType.Int) { Value = permissionId });

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            var connectionString = _dbOperation.GetConnectionString();
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("sp_UserHasPermission", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
            cmd.Parameters.Add(new SqlParameter("@PermissionName", SqlDbType.NVarChar, 100) { Value = permissionName });

            await conn.OpenAsync();

            var result = await cmd.ExecuteScalarAsync();

            if (result != null && int.TryParse(result.ToString(), out var intResult))
            {
                return intResult == 1;
            }

            return false;
        }
    }
}
