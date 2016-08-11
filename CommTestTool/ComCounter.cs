using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommTestTool
{
    public class ComCounter
    {
        private static int _recvNum = 0;
        private static int _sendNum = 0;

        public static int RecvNum()
        {
            lock (new object())
            {
                return ++_recvNum;
            }
        }

        public static int SendNum()
        {
            lock (new object())
            {
                return ++_sendNum;
            }
        }

        public static void ClearCounter()
        {
            lock (new object())
            {
                _sendNum = 0;
                _recvNum = 0;
            }
        }
    }
}
