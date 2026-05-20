using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class Document
{
    public long Id { get; set; }

    public string Filename { get; set; } = null!;

    public string Filepath { get; set; } = null!;

    public long? Verificationid { get; set; }

    public long? Measurementdeviceid { get; set; }

    public virtual Measurementdevice? Measurementdevice { get; set; }

    public virtual Verification? Verification { get; set; }
}
