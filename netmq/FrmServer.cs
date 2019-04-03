using System;
using System.Windows.Forms;
using NetMQ.Sockets;
using NetMQ;
using Utility;
using System.Configuration;
using System.Threading.Tasks;

namespace netmq
{
    public partial class FrmServer : Form
    {
        delegate void SetTextCallback(string text);
        private readonly string pubIP = ConfigurationManager.AppSettings["pubIP"];
        private readonly string pubPort = ConfigurationManager.AppSettings["pubPort"];

        private readonly string serverIP = ConfigurationManager.AppSettings["serverIP"];
        private readonly string serverPort = ConfigurationManager.AppSettings["serverPort"];
        public FrmServer()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            Task task = new Task(BeginServer);
            task.Start();


            //using (var server = new ResponseSocket($"@tcp://*:5556"))
            //{
            //    txtResult.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 服务器启动成功！\r\n");
            //    while (true)
            //    {
            //        //接收消息
            //        string messsage = server.ReceiveFrameString();
            //        var data = MessageHelper.GetMessage(messsage);
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

            //            txtResult.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {data.UserName} 【{(data.File != null ? data.File.Name : "")}】【{data.Message}】\r\n");


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

            //        server.SendFrame(MessageHelper.SendMessage(data));

            //    }
            //}

        }

        void BeginServer()
        {
            using (var pubSocket = new PublisherSocket())
            using (var server = new ResponseSocket())
            {
                SetText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 服务器启动成功！");

                //绑定订阅端口
                pubSocket.Bind($"tcp://{pubIP}:{pubPort}");
                //绑定服务端口
                server.Bind($"tcp://{serverIP}:{serverPort}");

                while (true)
                {
                    //接收消息
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
                        string file_name = "";
                        if (data.File != null && !string.IsNullOrEmpty(data.File.FileName))
                            file_name = "上传了【" + data.File.FileName + "】文件";
                        string msg = "";
                        if (!string.IsNullOrEmpty(data.Message))
                            msg = "发送了【" + data.Message + "】消息";

                        SetText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {data.UserName}：{file_name} {msg}");


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

                    pubSocket.SendMoreFrame("all").SendFrame(MessageHelper.SendMessage(data));
                    server.SendFrameEmpty();
                }
            }
        }

        private void SetText(string text)
        {
            if (txtResult.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                txtResult.AppendText(text + "\r\n");
            }
        }
    }
}
