using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nesdesign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Nesdesign
{
    public class ProjectButtonTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TemplateA { get; set; }
        public DataTemplate TemplateB { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Offer offer)
            {
                

                var cell = container as FrameworkElement;
                var column = cell?.Parent as DataGridCell;
                var tag = (column?.Column as MyTemplateColumn)?.ColumnKey as string;
                // Jeśli orderNumber jest puste → A
                if (string.IsNullOrWhiteSpace(tag == "production"? offer.production : offer.construction))
                    return TemplateA;
                // Jeśli orderNumber ma wartość → B
                return TemplateB;
            }

            return base.SelectTemplate(item, container);
        }

    }

}


