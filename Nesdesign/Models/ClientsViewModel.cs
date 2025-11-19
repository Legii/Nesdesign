using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesdesign.Models
{
    public  class ClientsViewModel
    {


        public ObservableCollection<Client> Clients { get; set; }
        public ClientsViewModel()
        {
            Clients = new ObservableCollection<Client>();
        }

    }
}
