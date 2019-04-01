using System;

namespace Utility
{
    public class DataStructure
    {
        public UserStatus userStatus { get; set; }

        public string message { get; set; }

        /// <summary>
        /// 文件格式一定要将文件流转化为base64
        /// </summary>
        public string fileData { get; set; }

        public MessageType messageType { get; set; }

        public DateTime dateTime { get; set; }

    }
    public enum UserStatus
    {
        Connect = 0,
        Send = 1,
        Close = 2,
    }
    public enum MessageType
    {
        Message = 0,
        File = 1,
        MessageAndFile = 2
    }
}
