using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ;
using NetMQ.Sockets;
using Utility;
using System.IO;
using System.Configuration;
using NAudio.Wave;

namespace netmqClient
{
    public partial class FrmClient : Form
    {
        //当前消息类型
        private MessageType messageType;
        //当前上传文件
        private UploadFile uploadFile = new UploadFile();

        private readonly string pubIP = ConfigurationManager.AppSettings["pubIP"];
        private readonly string pubPort = ConfigurationManager.AppSettings["pubPort"];

        private readonly string serverIP = ConfigurationManager.AppSettings["serverIP"];
        private readonly string serverPort = ConfigurationManager.AppSettings["serverPort"];

        private readonly string topic = "all";//ConfigurationManager.AppSettings["topic"];

        delegate void SetTextCallback(string text);

        SubscriberSocket subSocket = new SubscriberSocket();
        RequestSocket client = new RequestSocket();

        NAudioRecorder recorder = new NAudioRecorder();


        public FrmClient()
        {
            InitializeComponent();

            client.Connect($"tcp://{serverIP}:{serverPort}");
            subSocket.Connect($"tcp://{pubIP}:{pubPort}");
            //1、订阅主题
            subSocket.Subscribe(topic);




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

            Task task = new Task(() =>
            {
                SendAndReceiveMsg(data);
            });
            task.Start();


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

            txtMsg.Text = "";

            DataStructure data = new DataStructure
            {
                DateTime = DateTime.Now,
                UserStatus = UserStatus.Send,
                MessageType = messageType,
                File = uploadFile,
                Message = msg,
                UserName = txtUserName.Text.Trim()
            };
            
            Task task = new Task(() =>
            {
                SendAndReceiveMsg(data);
            });
            task.Start();


        }


        void SendAndReceiveMsg(DataStructure data)
        {
            #region 发送消息
            //1.生成消息包
            string message = MessageHelper.SendMessage(data);
            //2、发送
            client.SendFrame(message);
            #endregion

            #region 接受消息
            client.ReceiveFrameString();

            while (true)
            {
                //1、接收主题
                string myTopic = subSocket.ReceiveFrameString();
                //2、接收消息
                data = MessageHelper.GetMessage(subSocket.ReceiveFrameString());


                if (data != null)
                {
                    playNotice();
                    if (data.File != null&&data.File.FileName!=null&&data.File.FileData!=null)
                    {
                        string filePath = Path.GetFullPath("file");
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);

                        using (var fs = new FileStream(filePath + "/" + data.File.FileName, FileMode.OpenOrCreate))
                        {
                            fs.Read(data.File.FileData, 0, data.File.FileData.Length);
                            SetText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {data.UserName} 发送【{data.File.FileName}】文件，已保存在{filePath}");
                        }
                    }
                    if (!string.IsNullOrEmpty(data.Message))
                    {
                        SetText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {data.UserName}：{data.Message}");
                    }

                }

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
                uploadFile.FileName = Path.GetFileName(fileDialog.FileName);
                uploadFile.FileData = File.ReadAllBytes(fileDialog.FileName);
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

        //播放提示消息
        void playNotice()
        {
            var waveOutDevice = new WaveOut();
            var audioFileReader = new AudioFileReader("../../audio/notice.mp3");
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
        }

        private void btnStartSpeak_Click(object sender, EventArgs e)
        {
            string path =Path.GetFullPath("temp");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            recorder.SetFileName($"{path}/{DateTime.Now.ToString("yyyyMMddHHmmss")}.wav");
            recorder.StartRec();
        }

        private void btnStopSpeak_Click(object sender, EventArgs e)
        {
            recorder.StopRec();
            uploadFile.FileName =Path.GetFileName(recorder.fileName);
            uploadFile.FileData = File.ReadAllBytes(recorder.fileName);
        }

       
    }
}
