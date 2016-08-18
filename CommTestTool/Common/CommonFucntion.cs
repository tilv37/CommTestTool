using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace CommTestTool.Common
{
    public class CommonFucntion
    {

        private const string PortReleaseGuid = "8875BD8E-4D5B-11DE-B2F4-691756D89593";
        public static byte[] StringToBytes(string str)
        {
            str = str.Replace(" ", "");
            int length = str.Length;
            str = (length%2) == 0 ? str:str.Substring(0, (length/2)*2);

            byte[] returnBytes = new byte[str.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            return returnBytes;
        }


        /// <summary>
        /// 将16进制报文输出为字符串
        /// </summary>
        /// <param name="src"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static String bytesToHexString(byte[] src, int length)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (src == null || length <= 0)
            {
                return null;
            }
            for (int i = 0; i < length; i++)
            {
                int v = src[i] & 0xFF;
                string hv = string.Format("{0:X000}", v);
                if (hv.Length < 2)
                {
                    stringBuilder.Append(0);
                }
                if (i == src.Length)
                {
                    stringBuilder.Append(hv);
                }
                else
                {
                    stringBuilder.Append(hv + " ");
                }
            }
            return stringBuilder.ToString();
        }

        public static int FindNextAvailablePort()
        {
            int port = 10000;
            bool isAvailabe = true;
            var mutex=new Mutex(false,string.Concat("Global/",PortReleaseGuid));
            mutex.WaitOne();
            try
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] endPoints = ipGlobalProperties.GetActiveUdpListeners();
                do
                {
                    if (!isAvailabe)
                    {
                        port ++;
                        isAvailabe = true;
                    }

                    foreach (IPEndPoint ipEndPoint in endPoints)
                    {
                        if (ipEndPoint.Port != port)
                            continue;
                        isAvailabe = false;
                        break;
                    }

                } while (!isAvailabe && port < IPEndPoint.MaxPort);

                return port;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
