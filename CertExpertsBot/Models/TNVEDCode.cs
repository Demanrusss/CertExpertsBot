namespace CertExpertsBot.Models
{
    public record TNVEDCode
    {
        public int Id { get; init; }
        public required string Code { get; init; }
        public required string Name { get; init; }
        public virtual ISet<TechReg> TechRegs { get; init; } = new HashSet<TechReg>();

        public override string ToString()
        {
            var techRegsStr = String.Join("\n", TechRegs);
            if (techRegsStr.Length == 0)
                techRegsStr = "Скорее всего таможня сертификаты требовать не будет";

            return $"{Code}\n\nОписание\n{Name}\n\nМеры\n{techRegsStr}";
        }
    }
}
