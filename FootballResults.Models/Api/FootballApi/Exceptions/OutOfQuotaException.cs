namespace FootballResults.Models.Api.FootballApi.Exceptions
{
    public class OutOfQuotaException : ApiException
    {
        public OutOfQuotaException() { }

        public OutOfQuotaException(string message) : base(message) { }
    }
}
