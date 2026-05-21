using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetryxWPF
{
    class MeasurementDeviceView
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string TypeName { get; set; }

        public string Serialnumber { get; set; }

        public int? Verificationinterval { get; set; }

        public DateOnly Lastverificationdate { get; set; }

        public DateOnly? Nextverificationdate { get; set; }
    }
}
