# HellFire Hustle

**HellFire Hustle** is a 3D endless runner game built in Unity, where the player navigates a character through a dynamically generated path of platforms, avoiding a deadly fire ground below. The game features lane-based movement, continuous forward motion, and a scrolling environment, with a focus on clean architecture and performance optimization.

## Table of Contents
- [Features](#features)
- [Project Status](#project-status)
- [Architecture](#architecture)
- [Setup Instructions](#setup-instructions)
- [Scene Setup](#scene-setup)
- [Scripts Overview](#scripts-overview)
- [Development Phases](#development-phases)
- [Testing](#testing)
- [Future Work](#future-work)
- [Contributing](#contributing)

## Features
- **Endless Runner Gameplay**: The player moves forward automatically, switching between Left, Middle, and Right lanes.
- **Lane-Based Movement**: Constrained lane switching (Middle ↔ Left/Right, no consecutive side-lane moves) using the State pattern.
- **Dynamic Platform Spawning**: Platforms spawn based on the player’s lane, with randomized placement and Object Pooling for performance.
- **Deadly Fire Ground**: Contact with the fire ground triggers game over.
- **Scrolling Camera**: Follows the player for a seamless experience.
- **Input Handling**: Supports keyboard (A/D keys) and touch (swipe) inputs using Unity’s legacy input system.
- **Clean Architecture**: Uses Service Locator, Singleton, Observer, Command, State, and MVC patterns for modularity.

## Project Status
The project is in **Phase 3** (Environment and Platform Spawning) of development, with core mechanics implemented. Future phases will add game loop, polish, and additional features like animations and obstacles.

## Architecture
The project employs several design patterns for maintainability and scalability:
- **Service Locator**: Manages global services (`GameManager`, `InputService`, `EventSystem`).
- **Singleton**: Ensures a single `GameManager` instance.
- **Observer**: Handles events (e.g., “PlayerMoved”) via `EventSystem`.
- **Command**: Encapsulates input actions (`MoveLeftCommand`, `MoveRightCommand`).
- **State**: Manages lane states (`MiddleLaneState`, `LeftLaneState`, `RightLaneState`).
- **MVC**: Separates player logic (`PlayerModel`, `PlayerView`, `PlayerController`).
- **Object Pooling**: Optimizes platform spawning/recycling.

## Setup Instructions
### Prerequisites
- **Unity**: Version 2022.3 LTS or later.
- **Git**: For version control.
- **IDE**: Visual Studio or Rider for C# scripting.

### Installation
1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   ```
2. **Open in Unity**:
   - Open Unity Hub, click **Add**, and select the cloned project folder.
   - Ensure the Unity version matches 2022.3 LTS or later.
3. **Configure Input System**:
   - Go to **Edit > Project Settings > Player**.
   - Set **Active Input Handling** to **Input Manager (Old)** to use the legacy input system.
4. **Set Script Execution Order**:
   - Go to **Edit > Project Settings > Script Execution Order**.
   - Set:
     - `GameManager`: `-100`
     - `PlayerController`: `0`
     - `PlatformManager`: `10`
   - Click **Apply**.

## Scene Setup
The main scene (`MainScene`) includes:
- **GameManager**: Empty GameObject with `GameManager.cs`.
- **Player**: Cube with `PlayerView.cs`, `PlayerController.cs`, and “Player” tag.
- **PlatformManager**: Empty GameObject with `PlatformManager.cs`, linked to a platform prefab.
- **FireGround**: Large plane (scale: `(10, 1, 1000)`, position: `(0, -0.5, 0)`) with `FireGround.cs` and a red material.
- **Main Camera**: Positioned at `(0, 2, -5)` with `CameraFollow.cs`, linked to the Player.

### Platform Prefab
- Create a Cube named “Platform” (scale: `(2, 0.5, 10)`).
- Add `Platform.cs`.
- Save as a prefab in `Assets/Prefabs`.
- Assign to `PlatformManager`’s `Platform Prefab` field in the Inspector.

## Scripts Overview
Scripts are organized in `Assets/Scripts` with subfolders:
- **Core**:
  - `ServiceLocator.cs`: Manages services.
  - `GameManager.cs`: Initializes game state and services.
  - `EventSystem.cs`: Publishes/subscribes to events.
- **Input**:
  - `ICommand.cs`, `MoveLeftCommand.cs`, `MoveRightCommand.cs`: Handle input actions.
  - `InputService.cs`: Processes keyboard (A/D) and touch (swipe) inputs.
- **Player**:
  - `PlayerModel.cs`: Stores player data (lane state).
  - `PlayerView.cs`: Updates player visuals.
  - `PlayerController.cs`: Manages movement and input.
  - `LaneState.cs`, `MiddleLaneState.cs`, `LeftLaneState.cs`, `RightLaneState.cs`: State pattern for lane management.
- **Platform**:
  - `FireGround.cs`: Triggers game over on collision.
  - `Platform.cs`: Defines platform properties.
  - `ObjectPool.cs`: Manages platform pooling.
  - `PlatformManager.cs`: Spawns/recycles platforms.
- **Camera**:
  - `CameraFollow.cs`: Follows the player.

## Development Phases
### Phase 1: Infrastructure
- Implemented Service Locator, Singleton, Observer, Command, and MVC patterns.
- Set up legacy input system (A/D keys, swipe).
- Created core scripts (`GameManager`, `InputService`, `PlayerController`, etc.).

### Phase 2: Player Movement
- Added continuous forward movement (5 units/second).
- Implemented lane-based movement with State pattern.
- Ensured lane constraints (Middle ↔ Left/Right).

### Phase 3: Environment and Platform Spawning
- Added deadly fire ground (`FireGround.cs`).
- Implemented platform spawning with Object Pooling (`PlatformManager.cs`, `ObjectPool.cs`).
- Set up lane-based spawning rules:
  - **Middle**: Next platform in Left, Middle (continuous), or Right (halfway, 5 units).
  - **Left/Right**: Next platform in same lane (continuous) or Middle (earlier, 5 units).
- Added scrolling camera (`CameraFollow.cs`).

## Testing
1. **Player Movement**:
   - Press A/D or swipe to switch lanes.
   - Verify constraints: Middle ↔ Left/Right, no Left ↔ Right.
   - Confirm continuous forward movement.
2. **Platform Spawning**:
   - In Middle lane, check random spawning (Left: X=-2, Z=+5; Middle: X=0, Z=+10; Right: X=2, Z=+5).
   - In Left/Right lanes, verify continuous or earlier Middle spawning.
   - Ensure platforms are recycled (no new instantiations).
3. **Fire Ground**:
   - Move player to `y = -1` to trigger “Game Over” log.
4. **Camera**:
   - Confirm smooth Z-axis following.
5. **Console Logs**:
   - Check for service registration, lane changes, and platform spawning.
   - Ensure no errors (e.g., “Service not found”).

## Future Work
- **Phase 4: Game Loop and Polish**:
  - Implement start/restart mechanics.
  - Add score system and UI.
  - Include animations and visual effects (e.g., fire particles).
- **Phase 5: Obstacles and Power-Ups**:
  - Add obstacles on platforms.
  - Introduce power-ups (e.g., speed boost).
- **Phase 6: Final Polish**:
  - Optimize performance.
  - Add audio and menus.

## Contributing
- Fork the repository and create a feature branch.
- Follow Unity’s C# coding conventions.
- Test changes thoroughly in `MainScene`.
- Submit pull requests with clear descriptions.