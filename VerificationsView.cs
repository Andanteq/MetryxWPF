using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetryxWPF
{
    internal class VerificationsView
    {
        public int Id { get; set; }

        public string Organization { get; set; }

        public string Certificatenumber { get; set; }

        public DateOnly Verificationdate { get; set; }

        public DateOnly? Nextverificationdate { get; set; }

        public bool? Suitable { get; set; }

        public string VSearialnumber { get; set; }

        public string VMeasurementdevice {  get; set; }
    }
}
