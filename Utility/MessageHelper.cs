using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utility
{
    public class MessageHelper
    {
        /// <summary>
        /// 生成消息包
        /// </summary>
        /// <param name="data">数据格式</param>
        /// <returns></returns>
        public static string SendMessage(DataStructure data)
        {
            string app_secret = "237183141@qq.com";
            string app_id = "netmq";

            string paramters = JsonConvert.SerializeObject(data);

            string message = paramters + "|" + GetMd5(app_id + app_secret + paramters);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

        }

        /// <summary>
        /// 校验消息包
        /// </summary>
        /// <param name="token">消息包</param>
        /// <returns></returns>
        public static DataStructure GetMessage(string message)
        {
           string token = Encoding.UTF8.GetString(Convert.FromBase64String(message));
            if (!string.IsNullOrEmpty(token))
            {
                string[] msg = token.Split('|');
                // 校验消息完整性
                if (msg.Length == 2)
                {
                    DataStructure data = JsonConvert.DeserializeObject<DataStructure>(msg[0]);
                    // 校验时间
                    if (data.DateTime < DateTime.Now)
                    {
                        //校验数据包的是否被修改过
                        if (message.Equals(SendMessage(data)))
                            return data;
                    }


                }
            }
            return null;
        }

        /// <summary>
        /// md5 32位加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMd5(string str)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();

        }
    }
}
