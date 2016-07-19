
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.IO.Ports;
using TJSYXY.Communication.TCP;
using TJSYXY.Communication.UDP;

namespace CommTestTool.Model
{
    public class LocalSettings
    {
        private Type _type;
        private IPEndPoint _ip;
        private string portName;
        private SerialPort _spPort;
        private string _netTpye;
        
        /// <summary>
        /// 通信类型
        /// </summary>
        public Type CommType
        {
            get { return _type; }
        }

        /// <summary>
        /// 串口对象
        /// </summary>
        public SerialPort SpPort
        {
            get { return _spPort; }
        }

        /// <summary>
        /// IP地址及端口
        /// </summary>
        public IPEndPoint IpAddress
        {
            get { return _ip; }
        }

        public string NetType
        {
            get { return _netTpye; }
        }





        private void SetCommType(string commStr)
        {
            if (commStr.Contains("Tcp"))
            {
                if (commStr.Contains("Client"))
                {
                    _type = typeof (TCPClientManager);
                    _netTpye = "TcpClient";
                }
                else
                {
                    _type = typeof (TCPServerManager);
                    _netTpye = "TcpServer";
                }
            }
            else if (commStr.Contains("Udp"))
            {
                if (commStr.Contains("Client"))
                {
                    _type = typeof(UDPClientManager);
                    _netTpye = "UdpClient";
                }
                else
                {
                    _type = typeof(UDPClientManager);
                    _netTpye = "TcpServer";
                }
            }
            else
            {
                portName = commStr;
                _type = typeof (SerialPort);
                _netTpye = "SerialPort";
            }
        }

        private void SetIp(string ip, string port)
        {
            IPAddress ipAddress;
            int _port;
            if (IPAddress.TryParse(ip, out ipAddress) && int.TryParse(port,out _port))
            {
                _ip = _port < 65535 ? new IPEndPoint(ipAddress, _port) : new IPEndPoint(ipAddress, 20000);
            }
            else
            {
                _ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"),20000);
            }
        }

        private void SetSerialPort(string PortName,string BaudRate,string StopBits,string DataBits,string Parity)
        {
            try
            {
                _spPort = new SerialPort(PortName, int.Parse(BaudRate), (Parity)Enum.Parse(typeof(Parity), Parity, true), int.Parse(DataBits), (StopBits)Enum.Parse(typeof(StopBits), StopBits, true));

            }
            catch (Exception)
            {
                
                _spPort=null;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="comtype">通信类型</param>
        /// <param name="ip">IP</param>
        /// <param name="port">端口号</param>
        /// <param name="comName">串口号</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataFlag">数据位</param>
        /// <param name="checkFlag">校验位</param>
        /// <param name="stopFlag">停止位</param>
        public LocalSettings(string comtype, string ip, string port, string baudRate, string dataFlag,
            string checkFlag, string stopFlag,bool RTS,bool DTR)
        {
            SetCommType(comtype);
            SetIp(ip,port);
            SetSerialPort(portName, baudRate,stopFlag,dataFlag,checkFlag);
            SetSerialRTS(RTS,DTR);
        }

        private void SetSerialRTS(bool RTS, bool DTR)
        {
            if (_spPort != null)
            {
                _spPort.ReadTimeout = 5000;
                _spPort.WriteTimeout = 5000;
                _spPort.ReceivedBytesThreshold = 10;
                _spPort.RtsEnable = RTS;
                _spPort.DtrEnable = DTR;
            }
        }
    }
}
