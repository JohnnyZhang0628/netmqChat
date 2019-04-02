using System;

namespace Utility
{
    public class DataStructure
    {
        public UserStatus UserStatus { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public File File { get; set; }

        public MessageType MessageType { get; set; }

        public DateTime DateTime { get; set; }

    }

    public class File
    {
        public string  Name { get; set; }

        public byte[] Data { get; set; }
    }
         
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// 连接
        /// </summary>
        Connect = 0,
        /// <summary>
        /// 推送消息
        /// </summary>
        Send = 1,
        /// <summary>
        /// 关闭
        /// </summary>
        Close = 2,
    }
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        Message = 0,
        /// <summary>
        /// 文件消息
        /// </summary>
        File = 1,
        /// <summary>
        /// 文本和文件消息
        /// </summary>
        MessageAndFile = 2
    }
}
