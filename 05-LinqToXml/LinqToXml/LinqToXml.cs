using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;


namespace LinqToXml
{
    public static class LinqToXml
    {
        /// <summary>
        /// Creates hierarchical data grouped by category
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation (refer to CreateHierarchySourceFile.xml in Resources)</param>
        /// <returns>Xml representation (refer to CreateHierarchyResultFile.xml in Resources)</returns>
        public static string CreateHierarchy(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            var output = new XElement("Root", doc.Elements("Data").GroupBy(x => x.Element("Category").Value)
            .Select(t => new XElement("Group", new XAttribute("ID", t.Key),
            t.Select(x => new XElement("Data", x.Element("Quantity"), x.Element("Price"))))));
            return output.ToString();
        }

        /// <summary>
        /// Get list of orders numbers (where shipping state is NY) from xml representation
        /// </summary>
        /// <param name="xmlRepresentation">Orders xml representation (refer to PurchaseOrdersSourceFile.xml in Resources)</param>
        /// <returns>Concatenated orders numbers</returns>
        /// <example>
        /// 99301,99189,99110
        /// </example>
        public static string GetPurchaseOrders(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            XNamespace NS = doc.GetNamespaceOfPrefix("aw");

            var res = doc.Elements(NS + "PurchaseOrder").Where(t => t.Element(NS + "Address").Element(NS + "State").Value == "NY").
                Select(t => t.Attribute(NS + "PurchaseOrderNumber").Value);

            string outPut = string.Empty;
            foreach (var v in res)
            {
                outPut = outPut != string.Empty ? $"{outPut},{v}" : v;
            }

            return outPut;
        }

        /// <summary>
        /// Reads csv representation and creates appropriate xml representation
        /// </summary>
        /// <param name="customers">Csv customers representation (refer to XmlFromCsvSourceFile.csv in Resources)</param>
        /// <returns>Xml customers representation (refer to XmlFromCsvResultFile.xml in Resources)</returns>
        public static string ReadCustomersFromCsv(string customers)
        {
            List<string> lst = customers.Split(',', '\n').ToList();

            XElement doc = new XElement("Root");

            for (int i = 0; i < lst.Count; i += 10)
            {
                string[] array = new string[10];
                lst.CopyTo(i, array, 0, 10);
                doc.Add(CreateCustomerNode(array));
            }

            return doc.ToString();
        }
        private static XElement[] CreateCustomerNode(string[] array)
        {
            return new XElement[]
            {
                new XElement("Customer", new XAttribute("CustomerID", array[0]),
                   new XElement("CompanyName", array[1]),
                   new XElement("ContactName", array[2]),
                   new XElement("ContactTitle", array[3]),
                   new XElement("Phone", array[4]),
                   new XElement("FullAddress",
                      new XElement("Address", array[5]),
                      new XElement("City", array[6]),
                      new XElement("Region", array[7]),
                      new XElement("PostalCode", array[8]),
                      new XElement("Country", array[9])))
            };
        }
        /// <summary>
        /// Gets recursive concatenation of elements
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation of document with Sentence, Word and Punctuation elements. (refer to ConcatenationStringSource.xml in Resources)</param>
        /// <returns>Concatenation of all this element values.</returns>
        public static string GetConcatenationString(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            return doc.Elements().Select(t => t.Value).Aggregate((result, b) => result + b);
        }

        /// <summary>
        /// Replaces all "customer" elements with "contact" elements with the same childs
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with customers (refer to ReplaceCustomersWithContactsSource.xml in Resources)</param>
        /// <returns>Xml representation with contacts (refer to ReplaceCustomersWithContactsResult.xml in Resources)</returns>
        public static string ReplaceAllCustomersWithContacts(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);

            doc.ReplaceAll(doc.Elements().Select(t => new XElement("contact", t.Elements())));
            return doc.ToString();
        }

        /// <summary>
        /// Finds all ids for channels with 2 or more subscribers and mark the "DELETE" comment
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with channels (refer to FindAllChannelsIdsSource.xml in Resources)</param>
        /// <returns>Sequence of channels ids</returns>
        public static IEnumerable<int> FindChannelsIds(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            IEnumerable<int> result = doc.Elements()
              .Where(t => t.Elements("subscriber").Count() > 1 &&
               t.Nodes().Any(x => x.NodeType == XmlNodeType.Comment && ((XComment)x).Value == "DELETE"))
              .Select(t => Convert.ToInt32(t.Attribute("id").Value));
            return result;
        }

        /// <summary>
        /// Sort customers in docement by Country and City
        /// </summary>
        /// <param name="xmlRepresentation">Customers xml representation (refer to GeneralCustomersSourceFile.xml in Resources)</param>
        /// <returns>Sorted customers representation (refer to GeneralCustomersResultFile.xml in Resources)</returns>
        public static string SortCustomers(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            doc.ReplaceNodes(doc.Elements()
                .OrderBy(u => u.Element("FullAddress").Element("Country").ToString())
                .ThenBy(u => u.Element("FullAddress").Element("City").ToString()));
            return doc.ToString();
        }

        /// <summary>
        /// Gets XElement flatten string representation to save memory
        /// </summary>
        /// <param name="xmlRepresentation">XElement object</param>
        /// <returns>Flatten string representation</returns>
        /// <example>
        ///     <root><element>something</element></root>
        /// </example>
        public static string GetFlattenString(XElement xmlRepresentation)
        {
            return xmlRepresentation.Value;
        }

        /// <summary>
        /// Gets total value of orders by calculating products value
        /// </summary>
        /// <param name="xmlRepresentation">Orders and products xml representation (refer to GeneralOrdersFileSource.xml in Resources)</param>
        /// <returns>Total purchase value</returns>
        public static int GetOrdersValue(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            var result = doc.Elements("Orders").Elements("Order").Elements("product").GroupBy(t => t.Value)
                .Select(gr => new { ID = gr.Key, Count = gr.Count() })
                .Sum(x =>
                Convert.ToInt32(doc.Elements("products").Elements().First(t => t.Attribute("Id").Value == x.ID).Attribute("Value").Value)
                * x.Count);
            return result;
        }
    }
}
