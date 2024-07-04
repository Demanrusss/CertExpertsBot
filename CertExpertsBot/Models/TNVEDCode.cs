namespace CertExpertsBot.Models
{
    public class TNVEDCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public virtual ICollection<TechReg> TechRegs { get; set; } = new HashSet<TechReg>();

        public override string ToString()
        {
            string techRegsStr = String.Join("\n", TechRegs);
            if (techRegsStr.Length == 0)
                techRegsStr = "Скорее всего таможня сертификаты требовать не будет";

            return String.Format("{0}\n\nОписание\n{1}\n\nМеры\n{2}", Code, Name, techRegsStr);
        }
    }
}
