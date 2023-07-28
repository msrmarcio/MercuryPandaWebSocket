using Mercury.PandaSerial;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SerialComm serialComm = new SerialComm();
            serialComm.OpenCloseCom("COM5");
        }
    }
}