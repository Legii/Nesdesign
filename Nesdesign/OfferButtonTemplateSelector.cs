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
    public class OfferButtonTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TemplateA { get; set; }
        public DataTemplate TemplateB { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Offer offer)
            {
                // Jeśli orderNumber jest puste → A
                if (string.IsNullOrWhiteSpace(offer.orderNumber))
                    return TemplateA;

                // Jeśli orderNumber ma wartość → B
                return TemplateB;
            }

            return base.SelectTemplate(item, container);
        }

    }

}


