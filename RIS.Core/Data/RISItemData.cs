using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS.Core.Data
{
    [Serializable]
    public class RISItemData
    {
#nullable disable
        public string ID;
        public DateTime Date;
        public int Prob;
        public int Impact;
        public string Assign;
        public string Context;
        public string Description="";
        public string RISMiti;
        public string RISTrigger;
        public string RISStatus;
#nullable restore
    }
}
