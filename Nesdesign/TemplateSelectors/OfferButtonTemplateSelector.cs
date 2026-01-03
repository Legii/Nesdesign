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
    public class OfferButtonTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TemplateA { get; set; }
        public DataTemplate TemplateB { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Offer offer)
            {
   
                if (string.IsNullOrWhiteSpace(offer.OrderNumber))
                    return TemplateA;
                return TemplateB;
            }

            return base.SelectTemplate(item, container);
        }

    }

}


