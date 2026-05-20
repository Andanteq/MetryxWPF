using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class Measurementdevice
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long Typeid { get; set; }

    public string Serialnumber { get; set; } = null!;

    public DateOnly Releasedate { get; set; }

    public DateOnly Lastverificationdate { get; set; }

    public int? Verificationinterval { get; set; }

    public DateOnly? Nextverificationdate { get; set; }

    public bool? Unsuitable { get; set; }

    public string? Installationlocation { get; set; }

    public long? Userid { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Devicetype Type { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual ICollection<Verification> Verifications { get; set; } = new List<Verification>();
}
