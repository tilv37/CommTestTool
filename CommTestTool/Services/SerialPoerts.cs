using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommTestTool.Services
{
    /// <summary>
    /// TCP消息参数
    /// </summary>
    public class SerialPortMessageReceivedEventArgs : EventArgs
    {

        /// <summary>
        /// 消息数据
        /// </summary>
        public byte[] Data
        {
            set;
            get;
        }

        /// <summary>
        /// 消息接收时间
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 表示处理TCP消息的方法
    /// </summary>
    /// <param name="csID">激发该事件的服务器（或客户端）ID</param>
    /// <param name="args">消息参数</param>
    public delegate void SerialPortMessageReceivedEventHandler(SerialPortMessageReceivedEventArgs args);

    /// <summary>
    /// 表示处理客户端发送的事件
    /// </summary>
    /// <param name="bytes"></param>
    public delegate void SerialPortMessageSendedEventHandler(byte[] bytes);
}
