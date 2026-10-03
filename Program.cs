using System;
using System.IO;
using System.Threading.Tasks;
using Project1.Network;

namespace Project1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool isServer = false;
            string ip = "127.0.0.1";
            int port = 5555;

            if (args.Length > 0)
            {
                if (args[0] == "--server") { isServer = true; if (args.Length > 1) int.TryParse(args[1], out port); }
                else if (args[0] == "--client")
                {
                    if (args.Length > 1)
                    {
                        var parts = args[1].Split(':');
                        ip = parts[0];
                        if (parts.Length > 1) int.TryParse(parts[1], out port);
                    }
                }
            }
            else
            {
                Console.WriteLine("Start as (S)erver or (C)lient?");
                isServer = (Console.ReadKey(true).Key == ConsoleKey.S);
            }

            try
            {
                if (isServer)
                {
                    Console.Clear();
                    Console.WriteLine("DEBUG: 1/3 - Booting up server...");

                    var dungeonFacade = new DungeonFacade();
                    DungeonPackage dungeon = dungeonFacade.CreateFullDungeon(40, 20, new LibraryTheme());

                    Console.WriteLine("DEBUG: 2/3 - Dungeon generated!");
                    Console.WriteLine("DEBUG: 3/3 - Starting network listener...");

                    GameServer server = new GameServer(dungeon.Level, dungeon.Manual);
                    await server.StartAsync(port);
                }
                else
                {
                    Console.Clear();
                    GameClient client = new GameClient();
                    await client.ConnectAsync(ip, port);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[CRITICAL ERROR] The application crashed!");
                Console.WriteLine(ex.ToString()); 
                Console.ResetColor();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }
}