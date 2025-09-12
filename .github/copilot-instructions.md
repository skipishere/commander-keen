# Copilot Instructions for Commander Keen

## Agent responses:
- Don't needlessly confirm everything after every response, responses like "You're right", "Understood" or similar are not necessary.
- When suggesting code changes, always provide a brief explanation of why the change is needed or beneficial, if there is a choice to be made ask which option is preferred don't just assume one.
- Stay on target and making unnecessary changes to files or code. If there are improvements that can be made, suggest them but don't make changes that aren't necessary.
- Ensure any changes that are made, especially to the build process are correctly updated in all relevant documentation files.
- If it is Saturday the first response of the day should contain one Star Wars quote.

## Project Overview
This is a Godot 4.4+ C# recreation of Commander Keen Episode 1. The project uses a state machine architecture for character behaviors and a centralized signal system for component communication.

## Architecture & Key Patterns

### State Machine Pattern
The project extensively uses state machines for character behaviors:
- **Player states**: `player/` directory contains `State` subclasses (`AirState`, `GroundState`, `PogoState`, etc.)
- **Enemy states**: Each creature has its own base state (`YorpBaseState`, `VorticonBaseState`, `GargBaseState`)
- **Interface**: All states implement `IState<T>` with `StateType`, `CanMove`, `Character`, and lifecycle methods
- **Example**: Look at `player/GroundState.cs` for typical state implementation patterns

### Signal-Based Communication
Global communication uses `SignalManager` singleton:
- Autoloaded as `/root/SignalManager` in `project.godot`
- **Pattern**: `signalManager.EmitSignal(nameof(SignalManager.EventName), params)`
- **Key signals**: `EnteringLevel`, `ExitingLevel`, `KeenDead`, `TeleportComplete`
- **Subscribe pattern**: `signalManager.EventName += OnEventName;` in `_Ready()`

### Game State Management
Global state lives in static `game_stats` class:
- **Collectibles**: `KeyCards` (flags enum), `ShipParts` (flags enum), `Score`, `Lives`
- **Level state**: `CurrentLevel`, `Levels` dictionary for completion tracking
- **Pogo stick**: Special enum `PogoStickState` (No/Yes/YesAndEquipped)

### Scene Structure
- **Main scene**: `Main.cs` handles level transitions and pause menu
- **Levels**: Live in `scenes/levels/` with standardized naming (`ck1-*`)
- **Creatures**: Each has its own directory under `scenes/creatures/`
- **UI**: Modular UI components in `scripts/` (e.g., `ScoreUI.cs`, `LivesUI.cs`)

## Development Workflows

### Building & Export
- **Docker-based**: Uses `Dockerfile` with Godot 4.4.1 Mono + .NET 8
- **Build script**: `docker-build.sh` handles asset import and multi-platform export
- **Export presets**: Configured in `export_presets.cfg` for Windows/Linux/macOS
- **CI/CD**: GitHub Actions in `.github/workflows/build-release.yml`

### Running Locally
```bash
# Restore .NET dependencies
dotnet restore

# Open project in Godot Editor (required for C# development)
# - Install Godot 4.4.1 Mono version
# - Open project.godot in Godot Editor
# - Let Godot import assets and compile C# code
# - Press F5 to run, or use Play button in editor
```

### Docker Build (Production)
```bash
# Build using Docker (matches CI/CD environment)
docker build -t commander-keen-builder .

# On Windows (PowerShell):
docker run --rm -v ${PWD}:/workspace -w /workspace commander-keen-builder

# On Linux/macOS:
docker run --rm -v $(pwd):/workspace -w /workspace commander-keen-builder

# Outputs to artifact/ directory
```

### Testing Pattern
- Implement `ITakeDamage` interface for damageable entities
- Use `DeathZone.cs` script for instant-kill areas
- Test state transitions via debugger or Godot's remote debugger

## Godot-Specific Conventions

### Node Structure
- **Player**: `CharacterBody2D` with `StateMachine`, `AnimationTree`, `Camera2D` children
- **Creatures**: Follow similar pattern with creature-specific state machines
- **UI**: `CanvasLayer` nodes with script components for individual UI elements

### Resource Management
- **Autoloads**: `SignalManager`, `GameLogic` in `project.godot`
- **Scene loading**: Use `ResourceLoader.Load<PackedScene>(path).Instantiate()`
- **Global classes**: Use `[GlobalClass]` attribute (see `game_stats.cs`)

### Input Handling
- States handle input via `StateInput(InputEvent inputEvent)` method
- Global input (pause) handled in `Main.cs` `_Process()`
- Use `Input.IsActionJustPressed()` for discrete actions

## File Organization Logic
- **`player/`**: All player-related states and core player script
- **`scenes/creatures/`**: Enemy types with their state machines
- **`scenes/levels/`**: Game levels and overworld
- **`shared/`**: Global singletons and managers
- **`scripts/`**: UI components and utility scripts
- **`artifact/`**: Build output directory (generated)

## Key Dependencies
- **Godot 4.4.1** with Mono/.NET support
- **.NET 8.0** SDK
- **Docker** for consistent build environment
- Custom input helper addon: `addons/input_helper/`

## Critical Implementation Details
- **Collision**: Player uses `CharacterBody2D.MoveAndSlide()` pattern
- **Animation**: Driven by `AnimationTree` with state-based playbook
- **Level transitions**: Managed via `SignalManager` events, not direct scene changes
- **Save system**: Uses `game_stats` static properties, persisted via Godot's user data
- **Physics**: Relies on Godot's default gravity from project settings

## Third-Party Code Policy
- **Prefer Godot/C# built-in solutions** over external dependencies when possible
- **Always call out** any proposed 3rd party packages/libraries before implementation
- **Check licensing compatibility** - this project recreates a classic game, so licensing matters
- **Justify necessity** - explain why built-in Godot features can't solve the problem
- **Current dependencies**: Only uses Godot 4.4.1 Mono, .NET 8.0 SDK, and custom input helper addon

When modifying state machines, always update both the state class and the parent state machine's state enum. When adding new signals, declare them in `SignalManager.cs` first.
