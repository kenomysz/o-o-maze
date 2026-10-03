using System.Collections.Generic;

namespace Project1.Network
{
    public class GameStateDTO
    {
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public List<CellDTO> Cells { get; set; } = new List<CellDTO>();
        public List<EntityDTO> Entities { get; set; } = new List<EntityDTO>();
        public string GlobalMessage { get; set; } = string.Empty;
        public PlayerUIDTO MyUI { get; set; } = new PlayerUIDTO();
    }

    public class PlayerUIDTO
    {
        public string CurrentMenu { get; set; } = "Exploration";
        public int MenuSelectedIndex { get; set; } = 0;
        public List<string> IntroText { get; set; } = new List<string>();
        public List<string> InventoryItems { get; set; } = new List<string>();

        public List<string> Attributes { get; set; } = new List<string>();
        public int Gold { get; set; }
        public int Coins { get; set; }
        public string LeftHand { get; set; } = "Empty";
        public string RightHand { get; set; } = "Empty";
        public string ItemOnGround { get; set; } = "Empty space.";

        public List<string> PendingAudio { get; set; } = new List<string>();

        public string SystemMessage { get; set; } = string.Empty;

        public bool IsMyTurn { get; set; } = false;
        public string EnemyName { get; set; } = string.Empty;
        public int EnemyHp { get; set; }
        public int EnemyMaxHp { get; set; }
        public int EnemyAtk { get; set; }
        public int EnemyArm { get; set; }
    }

    public class CellDTO
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }
    }

    public class EntityDTO
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Symbol { get; set; }
        public string Name { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public bool IsPlayer { get; set; }
        public string Stance { get; set; }
    }

    public class ClientActionDTO
    {
        public int PlayerId { get; set; }
        public string ActionType { get; set; }
    }
}