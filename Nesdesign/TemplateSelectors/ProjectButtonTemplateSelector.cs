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

namespace Nesdesign.TemplateSelectors
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

                if (string.IsNullOrWhiteSpace(tag == "production"? offer.Production : offer.Construction))
                    return TemplateA;
        
                return TemplateB;
            }

            return base.SelectTemplate(item, container);
        }

    }

}


