using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageDb.Models
{
    public class TechReg
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public virtual ICollection<TNVEDCode> TNVEDCodes { get; set; } = new HashSet<TNVEDCode>();
    }
}
