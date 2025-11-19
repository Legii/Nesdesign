using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesdesign.Models
{
    public class Client
    {
        public String NIP { get; set; }
        public String nazwa { get; set; }

        public Client(string NIP, string nazwa)
        {
            this.NIP = NIP;
            this.nazwa = nazwa;
        }
    }
}
