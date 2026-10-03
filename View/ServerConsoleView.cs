using System;
using System.Collections.Generic;
using System.IO;

namespace Project1.MVC
{
    public class ServerConsoleView
    {
        private readonly Dictionary<string, ConsoleColor> _categoryColors = new Dictionary<string, ConsoleColor>
        {
            { "GAME", ConsoleColor.Cyan },
            { "SYSTEM", ConsoleColor.Yellow },
            { "INPUT", ConsoleColor.DarkMagenta },
            { "ERROR", ConsoleColor.Red }
        };

        private readonly string _logFilePath;

        public ServerConsoleView()
        {
            try { Console.Title = "Project1 - Dedicated Server"; } catch { }

            _logFilePath = Path.Combine(Environment.CurrentDirectory, "server_logs.txt");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[INFO] Logger initialized. Writing to: \n{_logFilePath}\n");
            Console.ResetColor();
        }

        public void OnLogReceived(ILogMessage message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string fileOutput = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{message.Category}] {message.Content}\n";

            try
            {
                if (_categoryColors.TryGetValue(message.Category, out var color))
                    Console.ForegroundColor = color;
                else
                    Console.ForegroundColor = ConsoleColor.White;

                Console.Write($"[{time}] [{message.Category}]   ");
                Console.ResetColor();
                Console.WriteLine(message.Content);
            }
            catch
            {
                Console.WriteLine($"[{time}] [{message.Category}]   {message.Content}");
            }

            try
            {
                File.AppendAllText(_logFilePath, fileOutput);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Could not write to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}