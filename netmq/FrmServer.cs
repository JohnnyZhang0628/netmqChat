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

namespace netmq
{
    public partial class FrmServer : Form
    {

        public FrmServer()
        {
            InitializeComponent();
            txtServerIP.Text = "127.0.0.1";
            txtPort.Text = "5556";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string ip = txtServerIP.Text.Trim();
            if (string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("请输入服务器IP地址", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string port = txtPort.Text.Trim();

            if (string.IsNullOrEmpty(port))
            {
                MessageBox.Show("请输入服务器端口号", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (var _server = new ResponseSocket($"@tcp://{ip}:{port}"))
            {
                txtResult.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 服务器启动成功！\r\n");
                while (true)
                {
                    string msg = _server.ReceiveFrameString();
                    _server.SendFrame($"来自服务端消息：{msg}");
                    txtResult.AppendText($"{msg}\r\n");
                }


            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }
    }
}
