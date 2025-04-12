namespace Books_api.Models
{
    public class Status
    {
        private Status(string message) { Message = message; }

        public string Message { get; set; }

        public static Status Success { get { return new Status("OK"); } }
        public static Status InvalidUser { get { return new Status("Invalid user"); } }
        public static Status InvalidAmount { get { return new Status("Invalid amount"); } }
        public static Status InvalidTransactionId { get { return new Status("Invalid transaction id"); } }
        public static Status InvalidPaymentGatewayName { get { return new Status("Invalid payment gateway name"); } }
        public static Status InvalidOldPassword { get { return new Status("Invalid old password"); } }
        public static Status InvalidParameters { get { return new Status("Invalid parameters"); } }
        public static Status DataNotFound { get { return new Status("Requested data not found"); } }
        public static Status PasswordNotStrong { get { return new Status("Password not strong enough"); } }
        public static Status InvalidFileTypes { get { return new Status("Invalid file"); } }
    }

}
