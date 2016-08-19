using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CommTestTool
{
    public class ComCounter
    {
        private static int _recvNum = 0;
        private static int _sendNum = 0;
        private static int _timerTotal = 0;//定时器执行的次数
        private static bool _timerStatus = false;//false时候，不管次数是多少，依然继续执行定时器
        public static Mutex mutex = new Mutex();
        private static object monitor = new object();
        private static bool flag = true;
        private static  ManualResetEvent mre=new ManualResetEvent(false);

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

        public static void SetTimerTotal(int count)
        {
            lock (new object())
            {
                _timerTotal = count;
                if (count > 0)
                {
                    _timerStatus = true;
                }
            }
        }

        public static bool isTimerNeedDone()
        {
            lock (monitor)
            {
                if (!_timerStatus)
                {
                    return false;
                }
                else
                {
                    TimerTotalDesc();
                    Console.WriteLine(Thread.CurrentThread.Name + "完成访问变量");
                    return _timerTotal <= 0 ? true : false;
                }

            }
        }

        private static void TimerTotalDesc()
        {
            lock (monitor)
            {
                --_timerTotal;
            }

        }
    }
}
