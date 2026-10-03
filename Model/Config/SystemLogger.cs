using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Project1
{
    public class GameConfig
    {
        public string PlayerName { get; set; } = "Player";
        public string Theme { get; set; } = "Library";
        public string LogPath { get; set; } = "Logs";

        public static GameConfig Load(string path = "config.json")
        {
            if (!File.Exists(path))
                return new GameConfig();

            try
            {
                string json = File.ReadAllText(path);
                GameConfig config = JsonSerializer.Deserialize<GameConfig>(json);

                return config ?? new GameConfig();
            }
            catch (JsonException)
            {
                Console.WriteLine("Error parsing config.json file. Default settings loaded.");
                return new GameConfig();
            }
        }
    }

    public interface ILogger
    {
        void Log(string message);
        List<string> GetAllLogs();
        List<string> GetLatestLogs(int count);
        string GetFilePath();
    }

    public class FileAndMemoryLogger : ILogger
    {
        private readonly List<string> _logs = new List<string>();
        private readonly string _filePath;

        public FileAndMemoryLogger(string playerName, string logDirectory)
        {
            if (!Directory.Exists(logDirectory)) Directory.CreateDirectory(logDirectory);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _filePath = Path.Combine(logDirectory, $"{playerName}_{timestamp}.log");
        }

        public void Log(string message)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            _logs.Add(logEntry);
            File.AppendAllText(_filePath, logEntry + Environment.NewLine);
        }

        public List<string> GetAllLogs() => new List<string>(_logs);

        public List<string> GetLatestLogs(int count)
        {
            int skip = Math.Max(0, _logs.Count - count);
            return _logs.Skip(skip).ToList();
        }

        public string GetFilePath() => _filePath;
    }

    public class Journal
    {
        private static Journal _instance;
        public static Journal Instance => _instance ?? (_instance = new Journal());

        private readonly List<string> _logs = new List<string>();

        public event Action<ILogMessage> OnLogAdded;

        private Journal() { }


        public void Log(ILogMessage logMessage)
        {
            if (logMessage.IsPublicHistory)
            {
                _logs.Add(logMessage.Content);
                if (_logs.Count > 50) _logs.RemoveAt(0);
            }
            OnLogAdded?.Invoke(logMessage);
        }

        public void Log(string oldMessage)
        {
            Log(new GameLog(oldMessage));
        }

        public List<string> GetLatestLogs(int count)
        {
            int start = Math.Max(0, _logs.Count - count);
            return _logs.GetRange(start, _logs.Count - start);
        }
    }
}