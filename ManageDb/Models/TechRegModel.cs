using System.ComponentModel.DataAnnotations;
namespace ManageDb.Models;

public class TechRegModel
{
    public int Id { get; set; }
    [Display(Name = "Наименование")]
    public string Name { get; set; } = null!;
    [Display(Name = "Описание")]
    public string Description { get; set; } = null!;
    [Display(Name = "Коды ТН ВЭД")]
    public virtual ICollection<TNVEDCodeModel> TNVEDCodes { get; set; } = new HashSet<TNVEDCodeModel>();
}