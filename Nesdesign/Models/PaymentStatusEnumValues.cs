using System;
using System.Collections.Generic;
using System.Linq;

namespace Nesdesign.Models
{
    public static class PaymentStatusEnumValues
    {
        public class EnumItem<T> where T : struct, Enum
        {
            public T? Value { get; set; }
            public string Description { get; set; }
        }

        public static List<EnumItem<PaymentStatus>> AllValues { get; } =
            Enum.GetValues(typeof(PaymentStatus))
                .Cast<PaymentStatus>()
                .Select(v => new EnumItem<PaymentStatus>
                {
                    Value = v,
                    Description = StringHandler.GetEnumString(v)
                })
                .ToList();

        public static List<EnumItem<PaymentStatus>> AllValuesPlusNull { get; } =
            new List<EnumItem<PaymentStatus>> { new EnumItem<PaymentStatus> { Value = null, Description = "" } }
                .Concat(AllValues).ToList();
    }
}
