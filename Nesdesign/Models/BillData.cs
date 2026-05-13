using System;
using System.Collections.Generic;
using System.Text;

namespace Nesdesign.Models
{
    public class BillData
    {
        public string City { get; set; } = "Kraków";
        public string Date { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string Person { get; set; }
        public string BillNr { get; set; }
        public string OrderNumber { get; set; }
        public List<Part>Parts { get; set; } = new List<Part>();


        public BillData(string companyName, string companyAddress, string name, string billNr, string orderNumber, List<Part> parts)
        {
            Date = DateTime.Today.ToString("dd.MM.yyyy");
            CompanyName = companyName;
            CompanyAddress = companyAddress;
            Person = name;
            BillNr = billNr;
            OrderNumber = orderNumber;
            Parts = parts;
            Console.WriteLine(parts.Count);
        }
    }
}
