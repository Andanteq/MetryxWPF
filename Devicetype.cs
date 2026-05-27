using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class Devicetype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Measurementdevice> Measurementdevices { get; set; } = new List<Measurementdevice>();
}
