using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
   public class MessageHelper
    {
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public static byte[] SendMessage(DataStructure data)
        {
            string app_secret = "237183141@qq.com";
            string app_id = "netmq";

            string paramters =$"{data.userStatus},{data.messageType},{data.dateTime.ToString("yyyy-MM-dd HH:mm:ss")},{data.message},{data.fileData}";

            string token = paramters + "|" + GetMd5(app_id + app_secret + paramters);

           return Encoding.UTF8.GetBytes(token);

        }

        /// <summary>
        /// 校验秘钥
        /// </summary>
        /// <param name="tokenString"></param>
        /// <returns></returns>
        public static DataStructure GetMessage(byte[] buffer)
        {
            string token = Encoding.UTF8.GetString(buffer);
            if (!string.IsNullOrEmpty(token) && token.IndexOf(",") > 0)
            {
                string paramters = token.Split("|")[0];


                var paramtersObj = JObject.Parse(paramters);
                if (paramtersObj != null)
                {

                    //校验过期时间
                    var minutes = (DateTime.Now - Convert.ToDateTime(paramtersObj["date_time"].ToString())).TotalMinutes;
                    if (minutes > Convert.ToDouble(paramtersObj["expired"].ToString()))
                        return false;
                    else
                    {
                        // 校验秘钥
                        if (GetToken(paramters) == tokenString)
                            return true;
                        else
                            return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
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
