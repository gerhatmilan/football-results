namespace FootballResults.Models.Api.FootballApi.Exceptions
{
    public class MissingApiKeyException : ApiException
    {
        public MissingApiKeyException() { }

        public MissingApiKeyException(string message) : base(message) { }
    }
}
