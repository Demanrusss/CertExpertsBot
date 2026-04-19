namespace CertExpertsBot.Models
{
    internal sealed class AppSettings
    {
        public IDictionary<string, string> ConnectionStrings { get; } = new Dictionary<string, string>();
    }
}
