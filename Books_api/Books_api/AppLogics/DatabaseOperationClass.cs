using System.Data.SqlClient;

namespace Books_api.AppLogics
{
    public class DatabaseOperationClass
    {
        private static IConfiguration _configuration;

        public DatabaseOperationClass(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This will create the connection string as per the service name we send.
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            /*Fetch the connection string configuration from appsettings.json as per the environment*/
            builder.ConnectionString = _configuration.GetConnectionString("DefaultConnection");

            builder.InitialCatalog = "Productdb";
            return builder.ToString();
        }
    }
}
