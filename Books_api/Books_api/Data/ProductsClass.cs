using Books_api.AppLogics;
using Books_api.Models;
using System.Data.SqlClient;
using System.Data;

namespace Books_api.Data
{
    public class ProductsClass
    {
        private readonly DatabaseOperationClass _dbOperation;
        public ProductsClass(DatabaseOperationClass dbOperation)
        {
            _dbOperation = dbOperation;
        }

        public async Task<string> SaveEditProducts(ProductParameters parameters)
        {
            try
            {
                var connectionString = _dbOperation.GetConnectionString();

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                using SqlCommand command = new("sp_SaveEditProducts", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ProductId", parameters.ProductId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ParentProductId", parameters.ParentProductId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProductName", parameters.ProductName);
                command.Parameters.Add(
                    new SqlParameter("@Image", SqlDbType.VarBinary)
                    {
                        Value = parameters.Image ?? (object)DBNull.Value
                    });
                command.Parameters.AddWithValue("@Description", (object?)parameters.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Price", parameters.Price ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OnDiscount", parameters.OnDiscount);
                command.Parameters.AddWithValue("@DiscountPrice", parameters.DiscountPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsParentProduct", parameters.IsParentProduct);
                command.Parameters.AddWithValue("@Status", parameters.Status);


                string result = (await command.ExecuteScalarAsync())?.ToString() ?? "";

                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<List<ProductCategories>> SelectAllProductCategories(ListProductCategoriesParameters parameters)
        {
            var connectionString = _dbOperation.GetConnectionString();
            List<ProductCategories> productCategoriesList = new();

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new("sp_SelectAllProductCategories", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Status", parameters.Status);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    productCategoriesList.Add(new ProductCategories
                    {
                        ParentProductId = Convert.ToInt32(reader["ParentProductId"]),
                        ParentProductName = Convert.ToString(reader["ParentProductName"]),
                        Status = Convert.ToBoolean(reader["Status"])

                    });
                }
            }
            return productCategoriesList;
        }

        public async Task<List<Products>> SelectAllProducts(ListProductParameters parameters)
        {
            var connectionString = _dbOperation.GetConnectionString();
            List<Products> productsList = new();

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new("sp_SelectAllProductsByProductCategory", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ParentProductId", parameters.ParentProductId);
            command.Parameters.AddWithValue("@Status", parameters.Status);
            command.Parameters.AddWithValue("@PageNumber", parameters.PageNumber);
            command.Parameters.AddWithValue("@PageSize", parameters.PageSize);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    productsList.Add(new Products
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        ParentProductId = reader["ParentProductId"] != DBNull.Value ? Convert.ToInt32(reader["ParentProductId"]) : (int?)null,
                        ParentProductName = reader["ParentProductName"] != DBNull.Value ? Convert.ToString(reader["ParentProductName"]) : null,
                        ProductName = Convert.ToString(reader["ProductName"]),
                        Image = reader["Image"] != DBNull.Value ? (byte[])reader["Image"] : null,
                        Description = reader["Description"] != DBNull.Value ? Convert.ToString(reader["Description"]) : null,
                        Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : (decimal?)null,
                        OnDiscount = reader["OnDiscount"] != DBNull.Value && Convert.ToBoolean(reader["OnDiscount"]),
                        DiscountPrice = reader["DiscountPrice"] != DBNull.Value ? Convert.ToDecimal(reader["DiscountPrice"]) : (decimal?)null,
                        IsParentProduct = reader["IsParentProduct"] != DBNull.Value && Convert.ToBoolean(reader["IsParentProduct"]),
                        Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"])
                    });
                }
            }
            return productsList;
        }


        public async Task<List<Products>> ListBySearchAndSort(SearchAndSortProductParameters parameters)
        {
            var connectionString = _dbOperation.GetConnectionString();
            List<Products> productsList = new();

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            using SqlCommand command = new("sp_SearchAndSortProducts", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SearchTerm", parameters.SearchTerm);
            command.Parameters.AddWithValue("@ParentProductId", parameters.ParentProductId);
            command.Parameters.AddWithValue("@Status", parameters.Status);
            command.Parameters.AddWithValue("@SortBy", parameters.SortBy);
            command.Parameters.AddWithValue("@SortOrder", parameters.SortOrder);
            command.Parameters.AddWithValue("@PageNumber", parameters.PageNumber);
            command.Parameters.AddWithValue("@PageSize", parameters.PageSize);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    productsList.Add(new Products
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        ParentProductId = reader["ParentProductId"] != DBNull.Value ? Convert.ToInt32(reader["ParentProductId"]) : (int?)null,
                        ParentProductName = reader["ParentProductName"] != DBNull.Value ? Convert.ToString(reader["ParentProductName"]) : null,
                        ProductName = Convert.ToString(reader["ProductName"]),
                        Image = reader["Image"] != DBNull.Value ? (byte[])reader["Image"] : null,
                        Description = reader["Description"] != DBNull.Value ? Convert.ToString(reader["Description"]) : null,
                        Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : (decimal?)null,
                        OnDiscount = reader["OnDiscount"] != DBNull.Value && Convert.ToBoolean(reader["OnDiscount"]),
                        DiscountPrice = reader["DiscountPrice"] != DBNull.Value ? Convert.ToDecimal(reader["DiscountPrice"]) : (decimal?)null,
                        IsParentProduct = reader["IsParentProduct"] != DBNull.Value && Convert.ToBoolean(reader["IsParentProduct"]),
                        Status = reader["Status"] != DBNull.Value && Convert.ToBoolean(reader["Status"])
                    });
                }
            }
            return productsList;
        }


    }
}
