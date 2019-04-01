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

namespace netmqClient
{
    public partial class FrmClient : Form
    {
     
        public FrmClient()
        {
            InitializeComponent();
            txtServerIP.Text = "127.0.0.1";
            txtPort.Text = "5556";
        }

        private void btnConnect_Click(object sender, EventArgs e)
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
            if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
            {
                MessageBox.Show("请输入昵称", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (var client = new RequestSocket($">tcp://{ip}:{port}"))
            {
                client.SendFrame($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {txtUserName.Text} 加入聊天室。\r\n");
                txtResult.AppendText(client.ReceiveFrameString());
                
            }


        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtMsg.Text.Trim();
            using (var client = new RequestSocket($">tcp://{txtServerIP.Text}:{txtPort.Text}"))
            {
                client.SendFrame(msg);
                txtResult.AppendText(client.ReceiveFrameString());

            }
            

        }
    }
}
