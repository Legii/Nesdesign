using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nesdesign.Models
{
    [Table("WhoRecords")]
    public partial class Who : ObservableObject
    {
        [ObservableProperty]
        [Key]
        private int? id;
        [ObservableProperty]
        [Column("Name")]
        private string name = "";

    }
}
