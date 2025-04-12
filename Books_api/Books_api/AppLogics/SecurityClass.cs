namespace Books_api.AppLogics
{
    public class SecurityClass
    {
        public static string GetBcryptHash(string str)
        {
            return BCrypt.Net.BCrypt.HashPassword(str);
        }

        public static bool VerifyBcryptHash(string str, string hashed)
        {
            return BCrypt.Net.BCrypt.Verify(str, hashed);
        }
    }
}
