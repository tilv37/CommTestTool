﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommTestTool.Common;
using CommTestTool.Model;
using CommTestTool.Services;
using TJSYXY.Communication;
using TJSYXY.Communication.TCP;
using TJSYXY.Communication.UDP;

namespace CommTestTool
{
    public partial class Form1 : Form
    {
        private delegate void SendMessageDelegate(Msg msg,byte[] bytes,DateTime dateTime);
        private static SendMessageDelegate SendMessageHandler;

        private delegate void ShowMessageDelegate(Color color,byte[] bytes, DateTime dateTime);
        private static ShowMessageDelegate ShowMessageHandler;

        private TCPClientManager _tcpClientManager=null;
        private TCPServerManager _tcpServerManager = null;
        private UDPClientManager _udpClientManager = null;
        private SerialClient _serialClient = null;
        private LocalSettings _localSettings;
        private static bool initialFlag = false;

        private static int _richText2Rows = 0;
        public Form1()
        {
            InitializeComponent();
            CommboxBind();
            RadioButtonInit();
            DisableSendButton();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!initialFlag)
            {
                FormToSettings();
                if (initialFlag)
                {
                    button1.Text = "关闭端口";
                }
                cbCom.Enabled = false;
            }
            else
            {
                ClosePort();
                if (!initialFlag)
                {
                    button1.Text = "打开端口";
                    EnableIpUI();
                    EnableSerialUI();
                }
            }
        }

        #region 初始化UI
        private void CommboxBind()
        {
            FormInit fmInit=new FormInit();
            cbCom.DataSource = fmInit.GetAllComType();
            cbBaudrate.DataSource = fmInit.GetBaudRate();
            cbCheckFlag.DataSource = fmInit.GetCheckFlag();
            cbDataFlag.DataSource = fmInit.GetDataFlag();
            cbStopFlag.DataSource = fmInit.GetStopFlag();
            ShowMessageHandler += ShowDebug;
        }

        private void RadioButtonInit()
        {
            rbDisplayHex.Checked = true;
            rbInputHex.Checked = true;
        }
        #endregion

        #region 界面事件

        private void cbCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = (sender as ComboBox).SelectedIndex;
            if (selectedIndex > 3)
            {
                EnableSerialUI();
                DisableIpUI();
            }
            else
            {
                DisableSerialUI();
                EnableIpUI();
            }

            if ((sender as ComboBox).Text.Contains("Server"))
            {
                ckbOnlyRecv.Checked = true;
            }
            else
            {
                ckbOnlyRecv.Checked = false;
            }
        }

        private void DisableIpUI()
        {
            txtIp.Enabled = false;
            txtPort.Enabled = false;
        }

        private void EnableIpUI()
        {
            cbCom.Enabled = true;
            txtIp.Enabled = true;
            txtPort.Enabled = true;
        }

        private void DisableSerialUI()
        {
            cbCom.Enabled = false;
            cbCheckFlag.Enabled = false;
            cbBaudrate.Enabled = false;
            cbDataFlag.Enabled = false;
            cbStopFlag.Enabled = false;
            chkRTS.Enabled = false;
            chkDTR.Enabled = false;
        }

        private void EnableSerialUI()
        {
            cbCom.Enabled = true;
            cbCheckFlag.Enabled = true;
            cbBaudrate.Enabled = true;
            cbDataFlag.Enabled = true;
            cbStopFlag.Enabled = true;
            chkRTS.Enabled = true;
            chkDTR.Enabled = true;
        }

        private void EnableSendButton()
        {
            button3.Enabled = true;
        }

        private void DisableSendButton()
        {
            button3.Enabled = false;
        }
        #endregion


        /// <summary>
        /// 加载界面配置并打开端口
        /// </summary>
        private void FormToSettings()
        {

           _localSettings =new LocalSettings(cbCom.Text.Trim(),txtIp.Text.Trim(),txtPort.Text.Trim(),cbBaudrate.Text.Trim(),cbDataFlag.Text.Trim(),cbCheckFlag.Text.Trim(),cbStopFlag.Text.Trim(),chkRTS.Checked,chkDTR.Checked);
            Type obj = _localSettings.CommType;

            try
            {
                if (obj == typeof(TCPClientManager))
                {
                    if (_tcpClientManager == null)
                    {
                        _tcpClientManager = new TCPClientManager("tcpClient");
                        _tcpClientManager.TCPMessageSended += new TCPMessageSendedEventHandler(DiplayMessageSend);
                        _tcpClientManager.TCPMessageReceived += new TCPMessageReceivedEventHandler(manager_TCPMessageReceived);
                    }
                    _tcpClientManager.Connect(_localSettings.IpAddress);
                    DisableIpUI();

                    EnableSendButton();
                    initialFlag = true;

                }
                else if (obj == typeof(TCPServerManager))
                {
                    if (_tcpServerManager == null)
                    {
                        _tcpServerManager = new TCPServerManager("tcpServer");
                        _tcpServerManager.TCPMessageReceived += new TCPMessageReceivedEventHandler(manager_TCPMessageReceived);
                    }
                    _tcpServerManager.Start(_localSettings.IpAddress.Port); //启动服务器
                    DisableIpUI();
                    initialFlag = true;
                }
                else if (obj == typeof(UDPClientManager))
                {
                    if (_udpClientManager == null)
                    {
                        _udpClientManager = new UDPClientManager("udpClient");
                        _udpClientManager.UDPMessageSended += new UDPMessageSendedEventHandler(DiplayMessageSend);
                        _udpClientManager.UDPMessageReceived += new UDPMessageReceivedEventHandler(manager_UDPMessageReceived);
                    }
                    if (_localSettings.NetType.Contains("Server"))
                    {
                        _udpClientManager.Start(_localSettings.IpAddress);

                    }
                    else
                    {
                        _udpClientManager.Start(CommonFucntion.FindNextAvailablePort());
                        EnableSendButton();
                    }

                    DisableIpUI();
                    initialFlag = true;
                }

                else if (obj == typeof(SerialPort))
                {
                    _serialClient=new SerialClient(_localSettings.SpPort);
                    try
                    {
                        _serialClient.Connect();
                        EnableSendButton();
                        DisableSerialUI();
                    }
                    catch (Exception ex)
                    {
                        
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    
                    _serialClient.SerialPortMessageSendedEvent+=new SerialPortMessageSendedEventHandler(DiplayMessageSend);
                    _serialClient.SerialDataReceivedEvent+=new SerialPortMessageReceivedEventHandler(manager_serialPortMessageReceived);
                    initialFlag = true;
                }
                else
                {
                    MessageBox.Show("你选择的通信方式不支持！");
                    return;
                }
//                ShowMessageHandler += ShowDebug;
            }
            catch (Exception exception)
            {

                MessageBox.Show(exception.Message);
            }

        }

        private void ClosePort()
        {
            if (_tcpClientManager!=null)
            {
                _tcpClientManager.DisConnect();
//                _tcpClientManager = null;
                initialFlag = false;
            }

            if (_tcpServerManager != null)
            {
                _tcpServerManager.Stop();
//                _tcpServerManager = null;
                initialFlag = false;
            }
            if (_udpClientManager != null)
            {
                _udpClientManager.Stop();
//                _udpClientManager = null;
                initialFlag = false;
            }
            if (_serialClient != null)
            {
                _serialClient.DisConnect();
//                _serialClient = null;
                initialFlag = false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutForm af=new AboutForm();
            af.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearRichText();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string tempData = richTextBox1.Text.Trim();
            byte[] bytes;
            if (rbDisplayAsc.Checked)
            {
                bytes = Encoding.ASCII.GetBytes(tempData);
            }
            else
            {
                bytes = CommonFucntion.StringToBytes(tempData);
            }
            

            if (bytes.Count() != 0)
            {
                if (_localSettings.NetType.Contains("Tcp"))
                {
                    _tcpClientManager.Send(Msg.Zmsg1, bytes);

                }
                else if(_localSettings.NetType.Contains("Udp"))
                {
                    _udpClientManager.SendTo(Msg.Default, bytes, _localSettings.IpAddress.Address.ToString(), _localSettings.IpAddress.Port);

                }
                else
                {
                    _serialClient.Send(bytes);
                }
            }
        }

        private void DiplayMessageSend(byte[] bytes)
        {
            this.Invoke(ShowMessageHandler, Color.LimeGreen, bytes, DateTime.Now);
        }

        private void ShowDebug(Color color, byte[] bytes,DateTime dateTime)
        {
            StringBuilder sb = new StringBuilder();
            string tempData = CommonFucntion.bytesToHexString(bytes, bytes.Length);
            string nowDate = string.Format("[{0}]", dateTime.ToString("yyyy-MM-dd HH:mm:ss:fff"));

            if (color == Color.Blue)
            {
                LogHelper.ShowInfo(nowDate+" Recv:"+tempData);
            }
            else if (color == Color.LimeGreen)
            {
                LogHelper.ShowInfo(nowDate + " Send:" + tempData);
            }


            if (ckbShowTime.Checked)
            {
                richTextBox2.SelectionStart = richTextBox2.TextLength;
                richTextBox2.SelectionLength = 0;
                richTextBox2.SelectionColor =Color.DarkGray;
                richTextBox2.AppendText(nowDate);
                richTextBox2.SelectionColor = richTextBox2.ForeColor;
            }
            sb.Append(" "+tempData);
            sb.Append(string.Format("({0} bytes)", bytes.Count()));
            if (ckbNewLine.Checked)
            {
                sb.Append("\r");
            }

            richTextBox2.SelectionStart = richTextBox2.TextLength;
            richTextBox2.SelectionLength = 0;
            richTextBox2.SelectionColor = color;
            richTextBox2.AppendText(sb.ToString());
            richTextBox2.SelectionColor = richTextBox2.ForeColor;
            _richText2Rows++;

//            this.richTextBox2.AppendText(sb.ToString());
        }

        private void ClearRichText()
        {
            richTextBox2.Clear();
            richTextBox2.ClearUndo();
            _richText2Rows = 0;
        }


        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            richTextBox2.SelectionStart = richTextBox2.Text.Length;
            richTextBox2.ScrollToCaret();
            if (_richText2Rows > 1000)
            {
                ClearRichText();
            }
        }

        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="csID"></param>
        /// <param name="args"></param>
        private  void manager_TCPMessageReceived(string csID, TCPMessageReceivedEventArgs args)
        {
            DateTime tm=args.Time;
            byte[] bytes = args.Data;
            this.Invoke(ShowMessageHandler, Color.Blue, bytes, tm);
//            this.Invoke((Action)delegate ()
//            {
//                    richTextBox2.AppendText(args.Time.ToLongTimeString() + " " + args.End.RemoteIP + ":" + args.End.RemotePort + " 发送文本:\n"
//                        + Encoding.Unicode.GetString(args.Data) + "\n");
//            });
        }

        private void manager_UDPMessageReceived(string csID, UDPMessageReceivedEventArgs args)
        {
            DateTime tm = args.Time;
            byte[] bytes = args.Data;
            this.Invoke(ShowMessageHandler, Color.Blue, bytes, tm);
        }

        private void manager_serialPortMessageReceived(SerialPortMessageReceivedEventArgs args)
        {
            DateTime tm = args.Time;
            byte[] bytes = args.Data;
            this.Invoke(ShowMessageHandler, Color.Blue, bytes, tm);
        }
    }
}