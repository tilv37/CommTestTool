using System;
using System.Collections.Generic;
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
            if (this._sp.BytesToRead <= 0)
                return;


            byte[] bytes = new byte[_sp.BytesToRead];
            _sp.Read(bytes, 0, _sp.BytesToRead);
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
