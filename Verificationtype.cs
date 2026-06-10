using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class Verificationtype
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Verification> Verifications { get; set; } = new List<Verification>();
}
