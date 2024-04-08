namespace CertExpertsBot.Models
{
    public class TechReg
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public virtual ICollection<TNVEDCode> TNVEDCodes { get; set; } = new HashSet<TNVEDCode>();

        public override string ToString()
        {
            return String.Format("{0}. Требуется {1}", Name, Description);
        }
    }
}
