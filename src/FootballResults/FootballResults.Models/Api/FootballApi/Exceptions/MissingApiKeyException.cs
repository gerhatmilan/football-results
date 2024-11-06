namespace FootballResults.Models.Api.FootballApi.Exceptions
{
    public class MissingApiKeyException : Exception
    {
        public MissingApiKeyException() { }

        public MissingApiKeyException(string message) : base(message) { }
    }
}
