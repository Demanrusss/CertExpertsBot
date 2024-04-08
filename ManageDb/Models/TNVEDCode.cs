namespace ManageDb.Models
{
    public class TNVEDCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public virtual ICollection<TechReg> TechRegs { get; set; } = new HashSet<TechReg>();
    }
}