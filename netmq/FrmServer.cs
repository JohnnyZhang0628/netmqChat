using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ.Sockets;
using NetMQ;
using Utility;
using System.IO;
using System.Configuration;

namespace netmq
{
    public partial class FrmServer : Form
    {
        private readonly string pubIP = ConfigurationManager.AppSettings["pubIP"];
        private readonly string pubPort = ConfigurationManager.AppSettings["pubPort"];

        private readonly string subIP = ConfigurationManager.AppSettings["subIP"];
        private readonly string subPort = ConfigurationManager.AppSettings["subPort"];
        public FrmServer()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            using (var pubSocket = new PublisherSocket())
            using (var server = new ResponseSocket())
            {
                txtResult.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 服务器启动成功！\r\n");

                //接收消息
                server.Bind($"tcp://{subIP}:{subPort}");
                string topic = server.ReceiveFrameString();
                string messsage = server.ReceiveFrameString();

                #region 处理消息
                var data = MessageHelper.GetMessage(messsage);
                if (data != null)
                {
                    if (data.UserStatus.Equals(UserStatus.Connect))
                    {
                        data.Message = $"欢迎{data.UserName}加入聊天室";
                        data.UserName = "系统消息";
                    }

                    if (data.UserStatus.Equals(UserStatus.Close))
                    {
                        data.Message = $"{data.UserName}退出聊天室";
                        data.UserName = "系统消息";
                    }

                    txtResult.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 您收到{data.UserName} 发送的【{(data.File != null ? data.File.Name : "")}】文件和【{data.Message}】消息\r\n");


                }
                else
                {
                    data = new DataStructure
                    {
                        UserName = "系统消息",
                        Message = "网络异常",
                        DateTime = DateTime.Now

                    };
                }

                #endregion


                // 推送消息
                pubSocket.Bind($"tcp://{pubIP}:{pubPort}");
                pubSocket.SendMoreFrame("all").SendFrame(MessageHelper.SendMessage(data));

            }

            //using (var _server = new ResponseSocket($"@tcp://{ip}:{port}"))
            //{

            //    while (true)
            //    {
            //        var dataBuffer = _server.ReceiveFrameBytes();
            //        var data = MessageHelper.GetMessage(dataBuffer);
            //        if (data != null)
            //        {
            //            if (data.UserStatus.Equals(UserStatus.Connect))
            //            {
            //                data.Message = $"欢迎{data.UserName}加入聊天室。";
            //                data.UserName = "系统消息";
            //            }

            //            if (data.UserStatus.Equals(UserStatus.Close))
            //            {
            //                data.Message = $"{data.UserName}退出聊天室。";
            //                data.UserName = "系统消息";
            //            }

            //            txtResult.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 您收到{data.UserName} 发送的{(data.File != null ? data.File.Name : "")}文件和{data.Message}消息。\r\n");


            //        }
            //        else
            //        {
            //            data = new DataStructure
            //            {
            //                UserName = "系统消息",
            //                Message = "网络异常",
            //                DateTime = DateTime.Now

            //            };
            //        }

            //        _server.SendMoreFrame("all").SendFrame(MessageHelper.SendMessage(data));

            //    }
            //}

        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }
    }
}
