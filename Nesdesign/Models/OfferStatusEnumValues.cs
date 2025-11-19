using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesdesign.Models
{
    public static class OfferStatusEnumValues
    {

        public class EnumItem<T>
        {
            public T Value { get; set; }
            public string Description { get; set; }
        }
        public static List<EnumItem<OfferStatus>> AllValues { get; } =
                   Enum.GetValues(typeof(OfferStatus))
                       .Cast<OfferStatus>()
                       .Select(v => new EnumItem<OfferStatus>
                       {
                           Value = v,
                           Description = StringHandler.GetEnumString(v)
                       })
                       .ToList();
    }
}
