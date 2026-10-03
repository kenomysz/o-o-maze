# O-O Maze

A multiplayer terminal-based dungeon crawler game built with C# and .NET Framework. Explore procedurally generated mazes, battle enemies, collect loot, and compete with other players in real-time combat and exploration.

## Features

- **Multiplayer Gameplay**: Play cooperatively or competitively with up to 9 players simultaneously using TCP networking
- **Procedural Dungeon Generation**: Each game features a unique 40×20 dungeon created with builder pattern and thematic generation
- **Turn-Based Combat**: Strategic real-time combat system with multiple attack stances (Normal, Stealth, Magic)
- **Inventory & Equipment**: Collect items, manage equipment, and customize your character with dual hand slots
- **Sound Propagation System**: Realistic audio cues let players hear nearby actions and enemy movements
- **Themed Dungeons**: Multiple visual themes (Library, Vault) with unique atmospheres
- **Dynamic NPC AI**: Enemies with AI-driven behavior that responds to player actions
- **Rich Console UI**: Three-column terminal interface showing the map, player stats, and inventory

## Running the Game

### Prerequisites

- .NET Framework 4.7.2 or later

### Single-Player Mode (Server + Client)
Start a server and connect a client to it:

**Terminal 1 - Start the server:**
```bash
dotnet run --server 5555
```

**Terminal 2 - Connect as client:**
```bash
dotnet run --client 127.0.0.1:5555
```
*(Or connect from another machine by replacing `127.0.0.1` with the server's IP address.)*

### Interactive Mode
If run without arguments, the game prompts you to choose:
```bash
dotnet run
```
You'll be asked: `Start as (S)erver or (C)lient?`
- Press **S** to start a server on localhost:5555
- Press **C** to connect to localhost:5555

## Gameplay Controls

### In the world:
- **W/A/S/D**: Move your character
- **E**: Pick up items on the ground
- **I**: Open inventory
- **1/2/3**: Change your combat stance

### In inventory:
- **W/S**: Navigate items
- **E**: Equip the selected item
- **Q**: Drop the selected item
- **1/2**: Unequip left/right hand
- **ESC**: Close inventory

### In combat:
- **1**: Normal Attack
- **2**: Stealth Attack
- **3**: Magic Attack
- **ESC**: Attempt to flee from combat

## Project Structure

```text
Project1/
├── Program.cs                     # Entry point; handles CLI args and server/client startup
├── config.json                    # Configuration file (player name, theme, log path)
├── Project1.csproj                # Build configuration
├── Project1.sln                   # Solution file
│
├── Model/
│   ├── Config/                    # Configuration and logging
│   ├── Dungeon/                   # Dungeon generation (builder pattern, terrain)
│   ├── Entities/                  # Game objects (Player, Enemy, Character, Entity)
│   ├── Items/                     # Items, equipment, currency
│   ├── Combat/                    # Combat logic and battle management
│   ├── Interaction/               # Faction system, noise observation
│   ├── Graphics/                  # Rendering interfaces
│   ├── Themes/                    # Dungeon theme definitions
│   └── DTO/                       # Data transfer objects for networking
│
├── Controller/
│   ├── Engine/                    # GameEngine: core game state and player management
│   ├── Server/                    # GameServer: TCP networking, client handling
│   ├── CombatController.cs        # Combat action processing
│   ├── PlayerSessionController.cs # Per-player session and state management
│   ├── WorldInteractionHandler.cs # World interaction chain of responsibility
│   ├── InputHandler.cs            # Keyboard input processing
│   ├── InventoryController.cs     # Inventory operations
│   └── ControlsConfig.cs          # Control key bindings
│
└── View/
    ├── ConsoleView.cs             # Client-side rendering (three-column layout)
    └── ServerConsoleView.cs       # Server-side log display
```

## How It Works

### Game Architecture
The project follows the Model-View-Controller (MVC) pattern with Server-Client networking:
- **Model**: Game state (dungeon, players, enemies, items) stored in `GameEngine`
- **Controller**: `GameServer` handles networking; `GameEngine` processes player actions
- **View**: `ConsoleView` renders the game state on the client

### Networking Flow
1. Server creates a `DungeonFacade`, generates the dungeon, and starts listening on the specified port.
2. Clients connect via TCP; each client is assigned a player ID.
3. Clients send `ClientActionDTO` objects (player actions) as JSON.
4. Server broadcasts `GameStateDTO` (full game state) to all clients every game tick (2 seconds).
5. Clients render the state in real-time.

### Key Systems
- **Dungeon Generation**: `DungeonFacade` uses the Builder Pattern with `IDungeonBuilder` implementations (e.g., `InstructionBuilder`) to construct levels.
- **Sound System**: `BroadcastSound` in `Level.cs` uses a breadth-first search to propagate sounds through walkable cells; enemies and players detect sounds within range.
- **Combat**: `CombatController` and `Battle` manage turn order and attack resolution.
- **Inventory**: Items can be equipped, dropped, or used; equipment modifies player attributes.

