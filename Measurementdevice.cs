using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class Measurementdevice
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Typeid { get; set; }

    public string Serialnumber { get; set; } = null!;

    public DateOnly Releasedate { get; set; }

    public DateOnly Lastverificationdate { get; set; }

    public int? Verificationinterval { get; set; }

    public DateOnly? Nextverificationdate { get; set; }

    public bool? Suitable { get; set; }

    public string? Installationlocation { get; set; }

    public int? Userid { get; set; }

    public string? Responsible {  get; set; }

    public string? Note { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual Devicetype Type { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual ICollection<Verification> Verifications { get; set; } = new List<Verification>();
}
