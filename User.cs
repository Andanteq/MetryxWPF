using System;
using System.Collections.Generic;

namespace MetryxWPF;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string? Phonenumber { get; set; }

    public int Roleid { get; set; }

    public string Lastname { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string? Middlename { get; set; }
    public string GetFullName()
    {
        return Lastname + " " + Firstname + " " + Middlename;
    }

    public virtual ICollection<Measurementdevice> Measurementdevices { get; set; } = new List<Measurementdevice>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role Role { get; set; } = null!;
}
