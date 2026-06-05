using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class Verification
{
    public int Id { get; set; }

    public string Organization {  get; set; }

    public string Certificatenumber { get; set; }

    public DateOnly Verificationdate { get; set; }

    public DateOnly? Nextverificationdate { get; set; }

    public bool? Suitable { get; set; }

    public int Measurementdeviceid { get; set; }

    public int Verificationtypeid { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual Measurementdevice Measurementdevice { get; set; } = null!;

    public virtual Verificationtype Verificationtype { get; set; } = null!;
}
