namespace API.Extensions.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public ApiResponse(int statusCode, string? errorMessage = null)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage ?? GetMatchingMessageByStatusCode(statusCode);
        }

        private string GetMatchingMessageByStatusCode(int statusCode) => statusCode switch
        {
            400 => "It seems you made a bad request, try again!",
            401 => "You are not authorized to access this resource.",
            404 => "We couldn't find this particular resource.",
            500 => "It seems there was a problem with the server, try again.",
            _ => null
        };
    }
}
