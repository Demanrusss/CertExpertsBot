namespace CertExpertsBot.Models
{
    internal class AppSettings
    {
        public virtual IDictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
    }
}
