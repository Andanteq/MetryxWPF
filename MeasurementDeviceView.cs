using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetryxWPF
{
    public class MeasurementDeviceView
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string TypeName { get; set; }

        public string Serialnumber { get; set; }

        public string SpeciesName { get; set; }

        public DateOnly Lastverificationdate { get; set; }

        public DateOnly? Nextverificationdate { get; set; }

        public VerificationStatus VerificationStatus { get; set; }

        public bool? Suitable { get; set; }
    }
    
    public enum VerificationStatus
    {
        Normal,
        Warning,
        Critical
    }
}
