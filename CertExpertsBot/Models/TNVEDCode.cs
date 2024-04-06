using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertExpertsBot.Models
{
    public class TNVEDCode
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public ICollection<TechReg> TechRegs { get; set; } = new HashSet<TechReg>();

        public override string ToString()
        {
            string techRegsStr = String.Join("\n", TechRegs);

            return String.Format("{0}\n{1}\n{2}", Code, Name, techRegsStr);
        }
    }
}
