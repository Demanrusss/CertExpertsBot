namespace CertExpertsBot.Models
{
    public record TechReg
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public virtual ISet<TNVEDCode> TNVEDCodes { get; init; } = new HashSet<TNVEDCode>();

        public override string ToString()
        {
            return $"{Name}. Требуется {Description}";
        }
    }
}
