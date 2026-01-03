using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Nesdesign.Models
{
    public partial class Client : ObservableObject
    {
        [ObservableProperty]
        [Column("clientId")]
        private int clientId;
        [ObservableProperty]
        [Column("name")]
        private string name = "";
        [ObservableProperty]
        [Column("NIP")]
        private string _NIP ="";
        [ObservableProperty]
        [Column("description")]
        private string? description = "";
        [ObservableProperty]
        [Column("address")]
        private string? address ="";



    }
}
