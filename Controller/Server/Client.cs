using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Project1.MVC;

namespace Project1.Network
{
    public class GameClient
    {
        private readonly ConsoleView _view = new ConsoleView();
        private StreamWriter _writer;
        private int _localPlayerId = 0;
        private readonly CommandMap _commands = new CommandMap();

        public GameClient()
        {
            _commands.Register(ConsoleKey.Enter, () => SendAction("Action_Start"));
            _commands.Register(ConsoleKey.Spacebar, () => SendAction("Action_Start"));

            _commands.Register(ControlsConfig.MoveUp, () => SendAction("MoveUp"));
            _commands.Register(ControlsConfig.MoveDown, () => SendAction("MoveDown"));
            _commands.Register(ControlsConfig.MoveLeft, () => SendAction("MoveLeft"));
            _commands.Register(ControlsConfig.MoveRight, () => SendAction("MoveRight"));

            _commands.Register(ControlsConfig.PickUp, () => SendAction("Action_E"));
            _commands.Register(ControlsConfig.Inventory, () => SendAction("Action_I"));

            _commands.Register(ConsoleKey.Q, () => SendAction("Action_Q"));
            _commands.Register(ConsoleKey.Escape, () => SendAction("Action_ESC"));
            _commands.Register(ConsoleKey.D1, () => SendAction("Action_1"));
            _commands.Register(ConsoleKey.D2, () => SendAction("Action_2"));
            _commands.Register(ConsoleKey.D3, () => SendAction("Action_3"));
        }

        public async Task ConnectAsync(string ip, int port)
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(ip, port);
            Console.Clear();

            NetworkStream stream = client.GetStream();
            _writer = new StreamWriter(stream) { AutoFlush = true };
            StreamReader reader = new StreamReader(stream);

            string initMsg = await reader.ReadLineAsync();
            if (initMsg != null && initMsg.StartsWith("INIT:"))
                _localPlayerId = int.Parse(initMsg.Split(':')[1]);

            _ = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        string json = await reader.ReadLineAsync();
                        if (json != null)
                        {
                            var state = JsonSerializer.Deserialize<GameStateDTO>(json);
                            if (state != null) _view.Render(state, _localPlayerId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine($"\nDisconnected from server.");
                    Console.WriteLine($"Reason: {ex.Message}");
                    Console.WriteLine("\nPress any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            });

            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                string result = _commands.HandleInput(keyInfo); 
                if (!string.IsNullOrEmpty(result))
                {
                    SendAction($"LogError_{result}"); 
                }
            }
        }

        private string SendAction(string actionType)
        {
            var action = new ClientActionDTO { PlayerId = _localPlayerId, ActionType = actionType };
            _writer.WriteLineAsync(JsonSerializer.Serialize(action));
            return string.Empty;
        }
    }
}