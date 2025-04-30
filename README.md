# HellFire Hustle

**Play the game here:** [PLAY--](https://prateektomar123.github.io/HellFireHustle_Build/)

## 🔥 Game Overview

HellFire Hustle is a fast-paced endless runner game where you navigate across platforms to avoid falling into the fire below. Switch lanes strategically, time your movements perfectly, and see how far you can go!

## 🎮 How to Play

- **Desktop Controls:**
  - Use **A** or **Left Arrow** to move left
  - Use **D** or **Right Arrow** to move right
  
- **Mobile Controls:**
  - **Swipe left** to move left
  - **Swipe right** to move right

Your character automatically moves forward. Your goal is to stay on the platforms and avoid falling into the fire below.

## 📋 Features

- **Three-lane system** - Navigate between left, center, and right lanes
- **Procedurally generated platforms** - Endless unique gameplay
- **Object pooling** - Optimized performance for smooth gameplay
- **State pattern** - Clean lane management
- **Command pattern** - Decoupled input handling
- **Service locator** - Efficient dependency management
- **Event system** - Scalable component communication
- **Mobile & desktop support** - Play anywhere!

## 🧩 Architecture

The game is built using several software design patterns to maintain clean, modular code:

### Command Pattern

The command pattern is used for handling player input, allowing for easy remapping and extension:

- `ICommand` - Base interface for all commands
- `MoveLeftCommand` & `MoveRightCommand` - Specific movement implementations

### State Pattern

The player's position is managed using a state pattern:

- `LaneState` - Abstract base class
- `LeftLaneState`, `MiddleLaneState`, `RightLaneState` - Concrete states

### Object Pooling

Performance is optimized through object pooling:

- `GenericObjectPool<T>` - Reusable object management
- `PlatformManager` & `FireGroundManager` - Pool consumers

### Service Locator

Dependencies are managed through a service locator:

- `ServiceLocator` - Central service registry
- Various services (InputService, EventSystem, etc.)

### MVC Pattern

Player functionality follows an MVC-like structure:

- `PlayerModel` - Game logic and state
- `PlayerView` - Visual representation
- `PlayerController` - Coordination and input handling

### Event System

Communication between components uses an event-based system:

- `EventSystem` - Publish/subscribe manager
- `GameEventType` enum - Event identifiers

## 📂 Project Structure

```
Assets/Scripts/
├── CommandSystem/
│   ├── ICommand.cs
│   ├── MoveLeftCommand.cs
│   └── MoveRightCommand.cs
├── ObjectPooling/
│   ├── FireGroundManager.cs
│   ├── GenericObjectPool.cs
│   └── PlatformManager.cs
├── OverallGameThings/
│   └── GameConfig.cs
├── Platform/
│   ├── MiddleColliderTrigger.cs
│   └── Platform.cs
├── Player/
│   ├── PlayerController.cs
│   ├── PlayerModel.cs
│   ├── PlayerView.cs
│   └── States/
│       ├── LaneState.cs
│       ├── LeftLaneState.cs
│       ├── MiddleLaneState.cs
│       └── RightLaneState.cs
├── StateSystem/
│   └── Game State Management/
│       └── GameStateManager.cs
├── Universalusage/
│   ├── CameraFollow.cs
│   └── FireGround.cs
└── Utils/
    ├── EventSystem.cs
    ├── GameManager.cs
    ├── InputService.cs
    ├── MonoSingleton.cs
    └── ServiceLocator.cs
```

## 🛠️ Key Components

### Game Manager

The `GameManager` serves as the central component, initializing core systems and maintaining game state:

- Manages global configuration via `GameConfig`
- Initializes and registers core services
- Implements the Singleton pattern for global access

### Platform System

Platforms are dynamically generated and managed:

- `PlatformManager` handles creation and recycling
- `Platform` represents individual platforms
- `MiddleColliderTrigger` detects player reaching platform midpoints

### Player System

The player character is divided into three components:

- `PlayerModel` handles game logic and state
- `PlayerView` manages visual representation
- `PlayerController` coordinates between model and view

### Input System

Player input is abstracted through the `InputService`:

- Handles keyboard and touch input
- Creates appropriate commands based on input
- Supports swipe detection for mobile

### Game State Management

Game states are managed by the `GameStateManager`:

- Handles transitions between menus, gameplay, pause, and game over
- Controls UI visibility based on game state
- Processes game lifecycle events

## ⚙️ Configuration

Game parameters are centralized in the `GameConfig` ScriptableObject:

- Player speed and lane distances
- Platform dimensions and spacing
- Object pool sizes
- Camera positioning

## 🏆 Game Loop

1. Player starts in the middle lane on the initial platform
2. Platforms are generated ahead as player progresses
3. Old platforms are recycled once passed
4. Player must navigate between lanes to stay on platforms
5. Game ends when player falls into the fire

## 🔄 Event System

The game uses an event-driven architecture with these key events:

- `PlayerMoved` - When player changes lanes
- `PlatformMidpointReached` - When player reaches platform middle
- `PlayerHitFireGround` - When player falls
- `GameOver` - When the game ends
- `GameStarted` - When a new game begins

## 🧠 Design Decisions

1. **Object Pooling** - Used to avoid runtime instantiation/destruction for smooth performance
2. **State Pattern** - Simplifies lane management logic
3. **Command Pattern** - Decouples input from actions
4. **Service Locator** - Provides a flexible dependency system
5. **Event System** - Creates loose coupling between components

## 🚀 Future Improvements

- Add power-ups and collectibles
- Implement difficulty progression
- Add more visual variety to platforms
- Include sound effects and background music
- Add high score system

## 🔧 Development Setup

1. Clone the repository
2. Open the project in Unity (recommended version: 2021.3 or newer)
3. Open the main scene
4. Press Play to test in the editor

## 📱 Building

### WebGL (for web deployment)
1. Open Build Settings (File > Build Settings)
2. Select WebGL platform
3. Click Build
4. Host the resulting files on a web server

### Mobile
1. Set up appropriate platform settings
2. Configure player settings for the target platform
3. Build and deploy to device

## 🙏 Credits

Created by: [Prateek Tomar]

---

Feel free to contribute to this project by submitting issues or pull requests!