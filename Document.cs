using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class Document
{
    public int Id { get; set; }

    public string Filename { get; set; } = null!;

    public string Filepath { get; set; } = null!;

    public int? Verificationid { get; set; }

    public int? Measurementdeviceid { get; set; }

    public virtual Measurementdevice? Measurementdevice { get; set; }

    public virtual Verification? Verification { get; set; }
}
