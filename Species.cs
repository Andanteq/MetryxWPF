using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetryxWPF
{
    public partial class Species
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Devicetype> Devicetypes { get; set; } = new List<Devicetype>();
    }
}
