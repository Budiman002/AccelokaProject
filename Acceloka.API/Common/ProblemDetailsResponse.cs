namespace Acceloka.API.Common
{
    public class ProblemDetailsResponse
    {
        public string Type { get; set; } = "https://tools.ietf.org/html/rfc7807";
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }
}