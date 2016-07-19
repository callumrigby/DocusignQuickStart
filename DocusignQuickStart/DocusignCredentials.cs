using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocusignQuickStart
{
    public class DocusignCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string IntegratorKey { get; set; }

        public DocusignCredentials()
        {
        }

        public DocusignCredentials(string username, string password, string integratorKey)
        {
            this.Username = username;
            this.Password = password;
            this.IntegratorKey = integratorKey;
        }
    }
}
