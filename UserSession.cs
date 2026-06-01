using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetryxWPF
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
    }
    public static class Session
    {
        public static UserSession CurrentUser { get; set; }
    }
}
