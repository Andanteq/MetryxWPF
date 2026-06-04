using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetryxWPF
{
    internal class VerificationsView
    {
        public string Organization { get; set; }

        public string Certificatenumber { get; set; }

        public DateOnly Verificationdate { get; set; }

        public DateOnly? Nextverificationdate { get; set; }

        public string? Result { get; set; }

        public bool? Suitable { get; set; }
    }
}
