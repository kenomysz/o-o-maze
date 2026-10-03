using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project1.Network;

namespace Project1.MVC
{
    public class ConsoleView
    {
        private const int Col2Offset = 2;
        private const int Col2Width = 35;
        private const int Col3Offset = 2;
        private const int Col3Width = 60;

        private readonly StringBuilder _frameBuffer = new StringBuilder();
        private int _lastFrameHeight = 0;

        public ConsoleView()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
        }

        public void Render(GameStateDTO state, int localPlayerId)
        {
            if (state.MyUI.CurrentMenu == "Intro")
            {
                RenderIntro(state.MyUI.IntroText);
                return;
            }

            if (state.MyUI.CurrentMenu == "Death")
            {
                RenderDeathScreen();
                return;
            }

            List<string> col2Lines = BuildMiddleColumn(state, localPlayerId);
            List<string> col3Lines = BuildRightColumn(state);

            int currentHeight = Math.Max(state.MapHeight, Math.Max(col2Lines.Count, col3Lines.Count));
            int windowWidth = Console.WindowWidth > 0 ? Console.WindowWidth : 120;
            int windowHeight = Console.WindowHeight > 0 ? Console.WindowHeight : 40;

            int maxDrawY = Math.Min(currentHeight, windowHeight - 1);
            int col2StartX = state.MapWidth + Col2Offset;
            int col3StartX = col2StartX + Col2Width + Col3Offset;

            for (int y = 0; y < maxDrawY; y++)
            {
                _frameBuffer.Clear();
                AppendMapRow(_frameBuffer, state, y);
                string mapStr = _frameBuffer.ToString().PadRight(col2StartX);
                if (mapStr.Length > col2StartX) mapStr = mapStr.Substring(0, col2StartX);

                Console.SetCursorPosition(0, y);
                Console.Write(mapStr);

                if (col2StartX < windowWidth)
                {
                    _frameBuffer.Clear();
                    AppendTextColumnRow(_frameBuffer, col2Lines, y, Col2Width);
                    string col2Str = _frameBuffer.ToString().PadRight(Col2Width + Col3Offset);

                    int maxLen2 = windowWidth - col2StartX - 1;
                    if (maxLen2 > 0)
                    {
                        if (col2Str.Length > maxLen2) col2Str = col2Str.Substring(0, maxLen2);
                        Console.SetCursorPosition(col2StartX, y);
                        Console.Write(col2Str);
                    }
                }

                if (col3StartX < windowWidth)
                {
                    _frameBuffer.Clear();
                    AppendTextColumnRow(_frameBuffer, col3Lines, y, Col3Width);
                    string col3Str = _frameBuffer.ToString().PadRight(Col3Width);

                    int maxLen3 = windowWidth - col3StartX - 1;
                    if (maxLen3 > 0)
                    {
                        if (col3Str.Length > maxLen3) col3Str = col3Str.Substring(0, maxLen3);
                        Console.SetCursorPosition(col3StartX, y);
                        Console.Write(col3Str);
                    }
                }
            }

            if (_lastFrameHeight > maxDrawY)
            {
                string emptyLine = new string(' ', windowWidth - 1);
                for (int y = maxDrawY; y < Math.Min(_lastFrameHeight, windowHeight - 1); y++)
                {
                    Console.SetCursorPosition(0, y);
                    Console.Write(emptyLine);
                }
            }

            _lastFrameHeight = maxDrawY;
            Console.SetCursorPosition(0, 0);
        }

        private void AppendMapRow(StringBuilder sb, GameStateDTO state, int y)
        {
            if (y >= state.MapHeight)
            {
                sb.Append(new string(' ', state.MapWidth));
                return;
            }
            char[] row = new char[state.MapWidth];
            foreach (var cell in state.Cells.Where(c => c.Y == y)) row[cell.X] = cell.Symbol;
            foreach (var entity in state.Entities.Where(e => e.Y == y)) row[entity.X] = entity.Symbol;
            sb.Append(new string(row));
        }

        private void AppendTextColumnRow(StringBuilder sb, List<string> lines, int y, int width)
        {
            if (y < lines.Count)
            {
                string line = lines[y] ?? "";
                if (line.Length > width) sb.Append(line.Substring(0, width));
                else sb.Append(line.PadRight(width));
            }
            else sb.Append(new string(' ', width));
        }

        private List<string> BuildMiddleColumn(GameStateDTO state, int localPlayerId)
        {
            List<string> ui = new List<string>();
            var localPlayer = state.Entities.FirstOrDefault(e => e.IsPlayer && e.Symbol.ToString() == localPlayerId.ToString());

            ui.Add($"--- PLAYER STATUS ---");
            if (localPlayer != null)
            {
                ui.Add($"HP: {localPlayer.Hp}/{localPlayer.MaxHp}");
                string stance = string.IsNullOrEmpty(localPlayer.Stance) ? "Normal" : localPlayer.Stance;
                ui.Add($"Stance: {stance}");
            }
            else ui.Add("Player dead or disconnected.");

            foreach (var stat in state.MyUI.Attributes) ui.Add(stat);
            ui.Add($"Gold: {state.MyUI.Gold} | Coins: {state.MyUI.Coins}");
            ui.Add("");

            ui.Add("--- EQUIPMENT ---");
            ui.Add($"L: {state.MyUI.LeftHand}");
            ui.Add($"R: {state.MyUI.RightHand}");
            ui.Add("");

            ui.Add("--- ON GROUND ---");
            ui.Add(state.MyUI.ItemOnGround);
            ui.Add("");

            var otherPlayers = state.Entities.Where(e => e.IsPlayer && e.Symbol.ToString() != localPlayerId.ToString()).ToList();
            if (otherPlayers.Count > 0)
            {
                ui.Add("--- OTHER PLAYERS ---");
                foreach (var p in otherPlayers) ui.Add($"[{p.Symbol}] {p.Name}: {p.Hp}/{p.MaxHp}");
                ui.Add("");
            }

            if (state.MyUI.CurrentMenu == "Combat")
            {
                ui.Add(">>> COMBAT MODE <<<");
                if (state.MyUI.IsMyTurn)
                {
                    ui.Add("--- IT IS YOUR TURN! ---");
                }
                else
                {
                    ui.Add("--- WAITING FOR OTHER TURNS... ---");
                }
                ui.Add($"Fighting: {state.MyUI.EnemyName}");
                ui.Add($"Enemy HP: {state.MyUI.EnemyHp}/{state.MyUI.EnemyMaxHp}");
                ui.Add($"ATK: {state.MyUI.EnemyAtk} | ARM: {state.MyUI.EnemyArm}");
                ui.Add("");
                ui.Add("--- COMBAT CONTROLS ---");
                ui.Add("[1] Normal Attack");
                ui.Add("[2] Stealth Attack");
                ui.Add("[3] Magic Attack");
                ui.Add("[ESC] Flee");
            }
            else if (state.MyUI.CurrentMenu == "Inventory")
            {
                ui.Add("--- INVENTORY CONTROLS ---");
                ui.Add("[W/S] Navigate");
                ui.Add("[E] Equip Item");
                ui.Add("[Q] Drop Item");
                ui.Add("[1/2] Unequip L/R Hand");
                ui.Add("[I/ESC] Close Inventory");
            }
            else
            {
                ui.Add("--- CONTROLS ---");
                ui.Add("[W/A/S/D] Move");
                ui.Add("[E] Pick Up Item");
                ui.Add("[I] Open Inventory");
                ui.Add("[1/2/3] Change Stance");
            }

            if (state.MyUI.PendingAudio != null && state.MyUI.PendingAudio.Count > 0)
            {
                ui.Add("");
                ui.Add("--- SOUNDS OVERHEARD ---");
                foreach (var sound in state.MyUI.PendingAudio)
                {
                    ui.Add($"~ {sound}");
                }
            }
            return ui;
        }

        private List<string> BuildRightColumn(GameStateDTO state)
        {
            List<string> ui = new List<string>();
            bool isInvActive = state.MyUI.CurrentMenu == "Inventory";

            ui.Add(isInvActive ? ">>> INVENTORY (ACTIVE) <<<" : "--- INVENTORY ---");
            ui.Add("");

            if (state.MyUI.InventoryItems == null || state.MyUI.InventoryItems.Count == 0)
            {
                ui.Add("(Empty)");
            }
            else
            {
                for (int i = 0; i < state.MyUI.InventoryItems.Count; i++)
                {
                    string prefix = (isInvActive && i == state.MyUI.MenuSelectedIndex) ? " > " : "   ";
                    string itemName = state.MyUI.InventoryItems[i];

                    if (itemName.Length > Col3Width - 6) itemName = itemName.Substring(0, Col3Width - 6) + "..";
                    ui.Add($"{prefix}{i + 1}. {itemName}");
                }
            }

            if (!string.IsNullOrEmpty(state.GlobalMessage))
            {
                ui.Add("");
                ui.Add("--- GLOBAL MESSAGE ---");
                ui.Add($"! {state.GlobalMessage} !");
            }

            if (!string.IsNullOrEmpty(state.MyUI.SystemMessage))
            {
                ui.Add("");
                ui.Add("--- SYSTEM MESSAGE ---");
                ui.Add($"> {state.MyUI.SystemMessage}");
            }

            return ui;
        }

        private void RenderIntro(List<string> instructions)
        {
            Console.Clear();
            Console.ResetColor();

            int windowWidth = Console.WindowWidth > 0 ? Console.WindowWidth : 120;
            Console.WriteLine("\n\n");

            if (instructions != null)
            {
                foreach (var line in instructions)
                {
                    if (line.StartsWith("---"))
                    {
                        Console.WriteLine();
                        PrintCentered(line, windowWidth);
                    }
                    else PrintCentered(line, windowWidth);
                }
            }

            Console.WriteLine("\n\n");
            PrintCentered("Press [ENTER] or [SPACE] to start your adventure", windowWidth);
            Console.WriteLine("\n");
        }

        private void PrintCentered(string text, int width)
        {
            if (string.IsNullOrEmpty(text)) { Console.WriteLine(); return; }
            int spaces = Math.Max(0, (width / 2) - (text.Length / 2));
            Console.WriteLine(new string(' ', spaces) + text);
        }

        private void RenderDeathScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            int windowWidth = Console.WindowWidth > 0 ? Console.WindowWidth : 120;

            Console.WriteLine("\n\n\n\n\n\n");
            PrintCentered("========================================", windowWidth);
            PrintCentered("             Y O U   D I E D            ", windowWidth);
            PrintCentered("========================================", windowWidth);
            Console.WriteLine("\n");

            Console.ForegroundColor = ConsoleColor.Gray;
            PrintCentered("Your physical form has vanished from the realm.", windowWidth);
            PrintCentered("You have been disconnected from the material world.", windowWidth);
            Console.WriteLine("\n\n");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            PrintCentered("You can safely close this window to exit.", windowWidth);
            Console.ResetColor();
        }
    }
}