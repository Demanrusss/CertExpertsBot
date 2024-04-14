using System.ComponentModel.DataAnnotations;

namespace ManageDb.Models
{
    public class TNVEDCode
    {
        public int Id { get; set; }
        [RegularExpression(@"[0-9]{10}", ErrorMessage = "Код ТН ВЭД должен состоять из 10 цифр")]
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public virtual ICollection<TechReg>? TechRegs { get; set; } = new HashSet<TechReg>();
    }
}