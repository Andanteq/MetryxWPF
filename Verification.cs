using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class Verification
{
    public long Id { get; set; }

    public DateOnly Verificationdate { get; set; }

    public DateOnly? Nextverificationdate { get; set; }

    public string? Result { get; set; }

    public bool? Unsuitable { get; set; }

    public long Measurementdeviceid { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual Measurementdevice Measurementdevice { get; set; } = null!;
}
