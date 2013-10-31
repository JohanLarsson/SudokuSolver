using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using System.Runtime.Serialization;

namespace SudokuSolverTests
{
    public class Class1
    {
        public int[,] Ints { get; set; }
        [Test]
        public void IndexOfTest()
        {
            Ints = new int[2, 2] { { 1, 2 }, { 3, 4 } };
            var serializer = new DataContractSerializer(this.GetType());
            using (var writer = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(writer))
                {
                    serializer.WriteObject(xmlWriter, this);
                }
                Console.Write(writer);
            }

        }

    }
}
