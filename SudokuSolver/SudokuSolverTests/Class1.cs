using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using System.Runtime.Serialization;

namespace SudokuSolverTests
{
    [Serializable]
    public class Class1
    {
        public int[,] Ints { get; set; }
        [Test]
        public void IndexOfTest()
        {
            Ints = new int[2, 2] { { 1, 2 }, { 3, 4 } };
            var bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, this);

        }

    }
}
