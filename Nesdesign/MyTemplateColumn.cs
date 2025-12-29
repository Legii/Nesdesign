using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Nesdesign
{
    public class MyTemplateColumn : DataGridTemplateColumn
    {
        public string ColumnKey
        {
            get { return (string)GetValue(ColumnKeyProperty); }
            set { SetValue(ColumnKeyProperty, value); }
        }

        public static readonly DependencyProperty ColumnKeyProperty =
            DependencyProperty.Register("ColumnKey", typeof(string), typeof(MyTemplateColumn));
    }

}
