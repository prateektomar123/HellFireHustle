Unity 3D Endless Runner Game
Overview
This is a 3D endless runner game built in Unity, featuring a player navigating an infinite series of platforms while avoiding a deadly fire ground below. The game uses a modular architecture with design patterns (Service Locator, Singleton, Observer, Command, State, MVC) and implements core mechanics like lane-based movement, dynamic platform spawning, and a scrolling camera. The project is structured in phases, with Phases 1–3 completed, covering infrastructure, player movement, and environment spawning.
Features

Player Movement: Continuous forward movement with lane-based lateral movement (Left, Middle, Right) using the legacy Unity input system (A/D keys or swipe).
Lane Management: State pattern enforces lane constraints (Middle ↔ Left/Right, no consecutive side-lane moves).
Dynamic Environment: Platforms spawn based on the player’s lane:
Middle: Randomly Left, Middle (continuous), or Right (halfway).
Left/Right: Continuous in same lane or Middle (earlier).


Object Pooling: Efficient platform recycling to optimize performance.
Fire Ground: Deadly surface below platforms triggers game over on contact.
Scrolling Camera: Follows the player’s forward movement.
Modular Architecture: Uses Service Locator, Singleton, Observer, Command, State, and MVC patterns for scalability.

Project Structure
Assets/
├── Prefabs/
│   └── Platform.prefab         # Platform prefab for spawning
├── Scenes/
│   └── MainScene.unity        # Main game scene
├── Scripts/
│   ├── GameManager.cs         # Singleton for game state and service registration
│   ├── ServiceLocator.cs      # Service Locator pattern for global service access
│   ├── EventSystem.cs         # Observer pattern for event handling
│   ├── ICommand.cs            # Command pattern interface
│   ├── MoveLeftCommand.cs     # Command for left movement
│   ├── MoveRightCommand.cs    # Command for right movement
│   ├── InputService.cs        # Handles legacy input (keyboard/touch)
│   ├── PlayerModel.cs         # MVC Model for player data
│   ├── PlayerView.cs          # MVC View for player visuals
│   ├── PlayerController.cs    # MVC Controller for player logic
│   ├── LaneState.cs           # Abstract state for lane management
│   ├── MiddleLaneState.cs     # Middle lane state
│   ├── LeftLaneState.cs       # Left lane state
│   ├── RightLaneState.cs      # Right lane state
│   ├── FireGround.cs          # Deadly fire ground logic
│   ├── Platform.cs            # Platform properties (length, lane position)
│   ├── ObjectPool.cs          # Object Pool for platform recycling
│   ├── PlatformManager.cs     # Manages platform spawning/recycling
│   └── CameraFollow.cs        # Camera follows player

Setup Instructions
Prerequisites

Unity Version: Unity 2022.3 LTS or later.
Platform: Windows, macOS, or Linux (for development); supports PC and mobile (touch input).
Git: For version control.

Installation

Clone the Repository:git clone <repository-url>


Open in Unity:
Open Unity Hub, click Add, and select the project folder.
Ensure the Unity version matches 2022.3 LTS or later.


Configure Scene:
Open Assets/Scenes/MainScene.unity.
Verify the scene hierarchy:MainScene
├── GameManager (GameObject)
│   └── GameManager.cs
├── Player (GameObject, Tag: "Player")
│   ├── PlayerView.cs
│   └── PlayerController.cs
├── PlatformManager (GameObject)
│   └── PlatformManager.cs
├── FireGround (GameObject)
│   └── FireGround.cs
├── Main Camera (GameObject)
│   └── CameraFollow.cs


Player: Assign the “Player” tag in the Inspector.
PlatformManager: Assign Assets/Prefabs/Platform.prefab to the Platform Prefab field.
Main Camera: Assign the Player GameObject to the Player field in CameraFollow.
FireGround: Ensure it’s a large plane (e.g., scale: (10, 1, 1000), position: (0, -0.5, 0)).


Set Script Execution Order:
Go to Edit > Project Settings > Script Execution Order.
Set:
GameManager: -100
PlayerController: 0
PlatformManager: 10


Click Apply.


Test the Scene:
Press Play in Unity.
Use A/D keys or swipe left/right (in Device Simulator or on a touch device) to switch lanes.
Verify:
Player moves forward continuously.
Platforms spawn dynamically (Left, Middle, Right from Middle; continuous or earlier from Left/Right).
Falling to the fire ground logs “Game Over”.
Camera follows the player smoothly.





Gameplay Mechanics

Controls:
Keyboard: Press A (move left), D (move right).
Touch: Swipe left/right to change lanes.


Lanes: Three lanes (Left: X = -2, Middle: X = 0, Right: X = 2).
From Middle, move to Left or Right.
From Left/Right, return to Middle or stay (no consecutive side-lane moves).


Platforms:
Size: 2 units wide, 0.5 units high, 10 units long.
Spawn when the player reaches the halfway point (Z = platform Z + 5).
Middle lane: Randomly Left, Middle (continuous, Z = previous Z + 10), or Right (halfway, Z = previous Z + 5).
Left/Right lanes: Continuous in same lane or Middle (earlier, Z = previous Z + 5).


Fire Ground: Contact with the ground below platforms triggers game over.
Camera: Follows the player’s Z-position with a fixed offset (e.g., (0, 2, -5)).

Development Phases
Phase 1: Infrastructure

Implemented Service Locator, Singleton (GameManager), Observer (EventSystem), Command (MoveLeftCommand, MoveRightCommand), and MVC (PlayerModel, PlayerView, PlayerController).
Set up legacy input system (InputService) for keyboard (A/D) and touch (swipe).
Scripts: GameManager.cs, ServiceLocator.cs, EventSystem.cs, ICommand.cs, MoveLeftCommand.cs, MoveRightCommand.cs, InputService.cs, PlayerModel.cs, PlayerView.cs, PlayerController.cs.

Phase 2: Player Movement

Added continuous forward movement (5 units/second).
Implemented State pattern for lane management (LaneState, MiddleLaneState, LeftLaneState, RightLaneState).
Enforced lane constraints and integrated with input system.
Scripts: Updated PlayerModel.cs, PlayerController.cs; added LaneState.cs, MiddleLaneState.cs, LeftLaneState.cs, RightLaneState.cs.

Phase 3: Environment and Platform Spawning

Created deadly fire ground (FireGround.cs).
Implemented platform spawning with Object Pooling (Platform.cs, ObjectPool.cs, PlatformManager.cs).
Added lane-based spawning logic and recycling.
Set up scrolling camera (CameraFollow.cs).
Scripts: FireGround.cs, Platform.cs, ObjectPool.cs, PlatformManager.cs, CameraFollow.cs.

Phase 4: Planned (Game Loop and Polish)

Add game loop (start, scoring, restart).
Implement animations and visual effects (e.g., fire particles, player movement).
Enhance UI and audio.
Refine gameplay balance (speed, platform spacing).

Known Issues

Animations: PlayerView.PlayMoveAnimation is a placeholder; full animations will be added in Phase 4.
Lane Initialization: PlatformManager uses a temporary MiddleLaneState(null) for initial lane state; will sync with PlayerModel in Phase 4.
Visuals: Platforms and fire ground use basic materials; visual polish planned for Phase 4.

Debugging Tips

Spawning Issues:
Check Console for Middle lane choice (should vary: 0, 1, 2) and Spawned platform at X logs in PlatformManager.
Verify PlayerMoved events in PlayerController (logs in Publish calls).


Service Errors:
Ensure script execution order is set correctly.
Check GameManager.Awake logs for service registration.


Performance:
Monitor Hierarchy for Object Pooling (no new platform instantiations).
Adjust ObjectPool initial size (10) if needed.



Contributing

Fork the repository and create a branch for your feature or bug fix.
Follow the existing architecture (Service Locator, MVC, etc.).
Test changes in MainScene and commit with clear messages.
Submit a pull request with a description of changes.

License
This project is unlicensed and intended for personal or educational use. Contact the repository owner for commercial use.
Contact
For issues or questions, open an issue on the repository or contact the project maintainer.
