using System.IO.Ports;

namespace CommTestTool.Common
{
    public class SerialPortHelper
    {
        public static string[] GetAllPorts()
        {
            string[] portList = SerialPort.GetPortNames();
            return portList;
        }
    }
}
