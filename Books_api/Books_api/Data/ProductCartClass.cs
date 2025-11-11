using Books_api.AppLogics;
using Books_api.Models;
using System.Data;
using System.Data.SqlClient;

namespace Books_api.Data
{
    public class ProductCartClass
    {
        private readonly DatabaseOperationClass _dbOperation;
        public ProductCartClass(DatabaseOperationClass dbOperation)
        {
            _dbOperation = dbOperation;
        }

        public async Task<string> AddProductsToCart(AddToCartParams parameters, int userId)
        {
            try
            {
                var connectionString = _dbOperation.GetConnectionString();

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                using SqlCommand command = new("sp_AddToCart", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ProductId", parameters.ProductId);
                command.Parameters.AddWithValue("@UserId", userId);

                string result = (await command.ExecuteScalarAsync())?.ToString() ?? "";

                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<List<CartDataDetails>> SelectAllCartProducts(int userId)
        {
            var connectionString = _dbOperation.GetConnectionString();
            List<CartDataDetails> cartData = new();

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new("sp_ListCartItems", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    cartData.Add(new CartDataDetails
                    {
                        CartId = Convert.ToInt32(reader["CartId"]),
                        UserId = Convert.ToInt32(reader["UserId"]),
                        UserName = Convert.ToString(reader["UserName"]),
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        ProductCount = Convert.ToInt32(reader["ProductCount"]),
                        ProductName = Convert.ToString(reader["ProductName"]),
                        Image = Convert.ToString(reader["Image"]),
                        Price = Convert.ToDecimal(reader["Price"]),
                        DiscountPrice = reader["DiscountPrice"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["DiscountPrice"]),
                        EntryDate = Convert.ToDateTime(reader["EntryDate"]),
                        Status = Convert.ToBoolean(reader["Status"]),
                        Remarks = Convert.ToString(reader["Remarks"])

                    });
                }
            }
            return cartData;
        }

        public async Task<string> UpdateCartProductCount(UpdateCartProductCount parameters)
        {
            try
            {
                var connectionString = _dbOperation.GetConnectionString();

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                using SqlCommand command = new("sp_UpdateCartProductCount", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@CartId", parameters.CartId);
                command.Parameters.AddWithValue("@Count", parameters.Count);

                string result = (await command.ExecuteScalarAsync())?.ToString() ?? "";

                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> RemoveSingleProductFromCart(UpdateAndDeleteCartProductStatus parameters)
        {
            try
            {
                var connectionString = _dbOperation.GetConnectionString();

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                using SqlCommand command = new("sp_RemoveFromCart", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@CartId", parameters.CartId);

                string result = (await command.ExecuteScalarAsync())?.ToString() ?? "";

                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<string> UpdateCartProductStatus(UpdateAndDeleteCartProductStatus parameters)
        {
            try
            {
                var connectionString = _dbOperation.GetConnectionString();

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                using SqlCommand command = new("sp_UpdateCartItems", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@CartId", parameters.CartId);

                string result = (await command.ExecuteScalarAsync())?.ToString() ?? "";

                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<string> RemoveAllProductFromCart(int userId)
        {
            try
            {
                var connectionString = _dbOperation.GetConnectionString();

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                using SqlCommand command = new("sp_RemoveAllProductFromCart", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserId", userId);

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
