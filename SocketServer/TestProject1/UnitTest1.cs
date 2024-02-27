namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            PQBook.book book = new PQBook.book();
            List<PQBook.book> lisbook=PQBook.Program.MuluBook();
            Console.WriteLine("1231234" + lisbook[0].name + "::" + lisbook[0].url);
        }
    }
}