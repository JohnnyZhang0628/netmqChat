using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ;
using NetMQ.Sockets;
using Utility;
using System.IO;
using System.Configuration;

namespace netmqClient
{
    public partial class FrmClient : Form
    {
        //当前消息类型
        private MessageType messageType;
        //当前上传文件
        private Utility.File uploadFile;

        private readonly string pubIP = ConfigurationManager.AppSettings["pubIP"];
        private readonly string pubPort = ConfigurationManager.AppSettings["pubPort"];

        private readonly string subIP = ConfigurationManager.AppSettings["subIP"];
        private readonly string subPort = ConfigurationManager.AppSettings["subPort"];

        RequestSocket client = new RequestSocket();
        SubscriberSocket subSocket = new SubscriberSocket();

        public FrmClient()
        {
            InitializeComponent();

            client.Connect($"tcp://{subIP}:{subPort}");
            subSocket.Connect($"tcp://{pubIP}:{pubPort}");
            //1、订阅主题
            subSocket.Subscribe("all");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
            {
                MessageBox.Show("请输入昵称", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            DataStructure data = new DataStructure
            {
                DateTime = DateTime.Now,
                UserStatus = UserStatus.Connect,
                UserName = txtUserName.Text.Trim()
            };

            SendAndReceiveMsg(data);

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMsg.Text.Trim();
            if (messageType.Equals(MessageType.File))
            {
                if (!string.IsNullOrEmpty(msg))
                    messageType = MessageType.MessageAndFile;
            }
            else
            {
                if (string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show("消息不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            DataStructure data = new DataStructure
            {
                DateTime = DateTime.Now,
                UserStatus = UserStatus.Send,
                MessageType = messageType,
                File = uploadFile,
                Message = msg,
                UserName = txtUserName.Text.Trim()
            };

            SendAndReceiveMsg(data);

        }


        void SendAndReceiveMsg(DataStructure data)
        {
            #region 发送消息
            //1.生成消息包
            string message = MessageHelper.SendMessage(data);
            //2、发送
            // pubSocket.SendMoreFrame("all").SendFrame(dataBuffer);
            client.SendMoreFrame("all").SendFrame(message);
            #endregion

            #region 接受消息
            //1、接收主题
            string topic = subSocket.ReceiveFrameString();
            //2、接收消息
            data = MessageHelper.GetMessage(subSocket.ReceiveFrameString());
            if (data != null)
            {
                if (data.File != null)
                {
                    using (var fs = new FileStream(data.File.Name, FileMode.OpenOrCreate))
                    {
                        fs.Read(data.File.Data, 0, data.File.Data.Length);
                        txtResult.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {data.UserName} 发送【{data.File.Name}】文件，已保存在{Path.GetFullPath(data.File.Name)} \r\n");
                    }
                }

                txtResult.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {data.UserName} {data.Message} \r\n");
            }
            #endregion
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "选择文件";
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                messageType = MessageType.File;
                uploadFile.Name = fileDialog.FileName;
                uploadFile.Data = Encoding.UTF8.GetBytes(Path.GetFullPath(fileDialog.FileName));
            }
        }
    }
}
