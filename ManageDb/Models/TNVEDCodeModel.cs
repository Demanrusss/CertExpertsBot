using System.ComponentModel.DataAnnotations;

namespace ManageDb.Models;

public class TNVEDCodeModel
{
    public int Id { get; set; }
    [Display(Name = "Код ТН ВЭД")]
    [RegularExpression(@"[0-9]{10}", ErrorMessage = "Код ТН ВЭД должен состоять из 10 цифр")]
    public string Code { get; set; } = null!;
    [Display(Name = "Описание")]
    public string Name { get; set; } = null!;
    [Display(Name = "Технические регламенты")]
    public virtual ICollection<TechRegModel>? TechRegs { get; set; } = new HashSet<TechRegModel>();
}