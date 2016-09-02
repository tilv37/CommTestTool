using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommTestTool.Model;

namespace CommTestTool.Common
{
    public class FormInit
    {
        public List<string> GetBaudRate()
        {          
            return new string[] {"1200","2400","4800","9600","14400","19200","38400","57600","115200"}.ToList();
        }

        private List<string> GetSocketType()
        {
            return new string[] {"Tcp Client","Tcp Server","Udp Client","Udp Server"}.ToList();
        }

        private List<string> GetAllComPort()
        {
            return SerialPortHelper.GetAllPorts().ToList();
        }

        public List<string> GetAllComType()
        {
            List<string> list = GetSocketType();
            List<string> list1 = GetAllComPort();
            list.AddRange(list1);
            return list;
        } 

        public List<string> GetStopFlag()
        {
            return new string[] {"1","1.5","2"}.ToList();
        }

        public List<string> GetDataFlag()
        {
            return new string[] {"8","7","6","5"}.ToList();
        }

        public List<string> GetCheckFlag()
        {
            return new string[] {"None", "Odd", "Even", "Mark", "Space" }.ToList();
        }

    }
}
