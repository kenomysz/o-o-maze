using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public interface ILogMessage
    {
        string Content { get; }
        string Category { get; }
        bool IsPublicHistory { get; }
    }

    public class GameLog : ILogMessage
    {
        public string Content { get; }
        public string Category => "GAME";
        public bool IsPublicHistory => true;
        public GameLog(string content) => Content = content;
    }

    public class SystemLog : ILogMessage
    {
        public string Content { get; }
        public string Category => "SYSTEM";
        public bool IsPublicHistory => false;
        public SystemLog(string content) => Content = content;
    }

    public class InputLog : ILogMessage
    {
        public string Content { get; }
        public string Category => "INPUT";
        public bool IsPublicHistory => false;
        public InputLog(string content) => Content = content;
    }
}