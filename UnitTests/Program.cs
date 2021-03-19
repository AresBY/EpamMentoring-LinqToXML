using LinqToXml.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqToXmlTests t = new LinqToXmlTests();
            //t.CreateHierarchyTest();
            t.GetPurchaseOrdersTest();
            // t.ReadCustomersFromCsvTest();
            t.GetConcatenationStringTest();
            // t.ReplaceAllCustomersWithContactsTest();
            t.FindChannelsIdsTest();
            //t.SortCustomersTest();
            t.GetFlattenStringTest();
           
        }
    }
}
