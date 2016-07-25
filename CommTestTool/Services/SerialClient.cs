using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace CommTestTool.Services
{
    public class SerialClient
    {
        private SerialPort _sp;


        public event SerialPortMessageReceivedEventHandler SerialDataReceivedEvent;
        public event SerialPortMessageSendedEventHandler SerialPortMessageSendedEvent;

        public SerialClient(SerialPort sp)
        {
            _sp = sp;
        }


        /// <summary>
        /// 客户端连接状态
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// 打开指定串口
        /// </summary>
        /// <param name="sp"></param>
        public void Connect()
        {
            if (_sp != null)
            {
                _sp.Open();
                _sp.DataReceived += new SerialDataReceivedEventHandler(OnReceived);
                Connected = true;
            }
            else if (_sp.IsOpen)
            {

            }
        }

        private void OnReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //接收前等待，是缓冲区充分读取本包报文
            System.Threading.Thread.Sleep(200);
            if (this._sp.BytesToRead <= 0)
                return;


            byte[] bytes = new byte[_sp.BytesToRead];
            for (int j = 0; j < bytes.Length; j++)
            {
                _sp.Read(bytes, j, 1);
                //字节接收间间隔，依然是给串口读取一个缓冲时间
                System.Threading.Thread.Sleep(10);
            }
            //_sp.Read(bytes, 0, _sp.BytesToRead);
            if (SerialDataReceivedEvent != null)
            {
                SerialPortMessageReceivedEventArgs args =new SerialPortMessageReceivedEventArgs();
                args.Time=DateTime.Now;
                args.Data = bytes;
                SerialDataReceivedEvent.BeginInvoke(args, null, null);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            if (Connected)
            {
                if (_sp != null)
                {
                    _sp.Close();
                    _sp = null;
                }
                Connected = false;
            }
        }

        public void Send(byte[] bytes)
        {
            _sp.Write(bytes,0,bytes.Length);
            if (SerialPortMessageSendedEvent != null)
            {
                SerialPortMessageSendedEvent.BeginInvoke(bytes,null,null);
            }
        }
    }
}
