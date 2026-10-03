using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Project1.Network
{
    public class GameServer
    {
        private readonly GameEngine _engine;
        private readonly Dictionary<int, TcpClient> _clients = new Dictionary<int, TcpClient>();

        private readonly int delayms = 500;
        private int _nextPlayerId = 1;
        private readonly Project1.MVC.ServerConsoleView _serverView;

        public GameServer(Level level, List<string> dungeonManual)
        {
            _engine = new GameEngine(level, dungeonManual);
            _serverView = new Project1.MVC.ServerConsoleView();
            Journal.Instance.OnLogAdded += _serverView.OnLogReceived;
        }

        public async Task StartAsync(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Journal.Instance.Log(new SystemLog($"Server started on port {port}."));

            _ = Task.Run(() => ServerTickLoop());

            while (true)
            {
                if (_clients.Count < 9)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    int playerId = _nextPlayerId++;
                    _ = Task.Run(() => HandleClientAsync(client, playerId));
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, int playerId)
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
            {
                await writer.WriteLineAsync($"INIT:{playerId}");

                lock (_engine.StateLock)
                {
                    _clients[playerId] = client;
                    _engine.SpawnPlayer(playerId);
                }

                try
                {
                    while (client.Connected)
                    {
                        string json = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(json)) break;

                        var action = JsonSerializer.Deserialize<ClientActionDTO>(json);
                        if (action != null)
                        {
                            lock (_engine.StateLock)
                            {
                                _engine.ProcessAction(playerId, action);
                            }
                            BroadcastState();
                        }
                    }
                }
                catch (Exception) { }
                finally { DisconnectPlayer(playerId); }
            }
        }

        private void ServerTickLoop()
        {
            Random rng = new Random();
            while (true)
            {
                Thread.Sleep(2000);
                lock (_engine.StateLock)
                {
                    _engine.Tick(rng);
                    BroadcastState();
                }
            }
        }

        private void BroadcastState()
        {
            var disconnectedIds = new List<int>();

            foreach (var kvp in _clients)
            {
                int pId = kvp.Key;
                TcpClient client = kvp.Value;

                try
                {
                    GameStateDTO state;
                    lock (_engine.StateLock)
                    {
                        state = _engine.BuildGameStateDTO(pId);
                    }

                    string json = JsonSerializer.Serialize(state) + "\n";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);

                    client.GetStream().Write(buffer, 0, buffer.Length);
                }
                catch
                {
                    disconnectedIds.Add(pId);
                }
            }

            foreach (var id in disconnectedIds) DisconnectPlayer(id);
        }

        private void DisconnectPlayer(int playerId)
        {
            lock (_engine.StateLock)
            {
                _engine.RemovePlayer(playerId);

                if (_clients.TryGetValue(playerId, out TcpClient client))
                {
                    client.Close();
                    _clients.Remove(playerId);
                }
            }
            BroadcastState();
            Journal.Instance.Log(new SystemLog($"Player {playerId} has disconnected."));
        }
    }
}