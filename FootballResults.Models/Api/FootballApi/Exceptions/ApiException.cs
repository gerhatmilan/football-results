namespace FootballResults.Models.Api.FootballApi.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException() { }

        public ApiException(string message) : base(message) { }
    }
}
