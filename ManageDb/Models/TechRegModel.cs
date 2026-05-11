namespace ManageDb.Models;

public class TechRegModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public virtual ICollection<TNVEDCodeModel> TNVEDCodes { get; set; } = new HashSet<TNVEDCodeModel>();
}