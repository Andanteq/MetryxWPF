using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class Notification
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public DateOnly Createdat { get; set; }

    public bool? Isread { get; set; }

    public int Userid { get; set; }

    public int? Measurementdeviceid { get; set; }

    public virtual Measurementdevice? Measurementdevice { get; set; }

    public virtual User User { get; set; } = null!;
}
