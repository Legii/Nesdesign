using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nesdesign.Models
{
    [Table("Contractors")]
    public partial class Who : ObservableObject
    {
        [ObservableProperty]
        [Column("Id")]
        private int id;
        [ObservableProperty]
        [Column("Name")]
        private string name = "";

        

        public override string ToString()
        {
            return id + " " + name; 
        }
    
    }

}
