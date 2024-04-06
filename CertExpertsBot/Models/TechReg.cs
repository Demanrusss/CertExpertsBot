using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertExpertsBot.Models
{
    public class TechReg
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public override string ToString()
        {
            return String.Format("{0}. Требуется {1}", Name, Description);
        }
    }
}
