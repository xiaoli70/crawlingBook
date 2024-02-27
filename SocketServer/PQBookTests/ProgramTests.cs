using Microsoft.VisualStudio.TestTools.UnitTesting;
using PQBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PQBook.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        [DataRow(45,35)]
        [DataRow(325,35265)]
        [DataRow(2363626,436346)]
        public void MuluBookTest(int a,int b)
        {
            Assert.AreEqual(a+b, Program.addintsum(a,b));
        }
    }
}