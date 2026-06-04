# Elephant vs Mouse Architecture

## Project Goal

Build a Unity 6 local multiplayer couch co-op racing game for 2 players sharing one keyboard.

The experience is asymmetric:

- Elephant races above ground on a simpler path and influences the underground maze.
- Mouse races underground through a more complex tunnel network and disrupts the Elephant.

The MVP focuses on clean separation between player controllers, race flow, and interaction effects.

## Guiding Principles

- Keep movement, race rules, and interaction effects in separate classes.
- Make level content data-driven where possible so the 3 handcrafted levels reuse the same systems.
- Keep split-screen presentation independent from gameplay logic.
- Treat interactions as reusable effects rather than one-off level scripts.

## System Architecture

### Runtime Layers

1. Input layer
- Reads shared keyboard input for both players.
- Converts raw keys into movement, jump, and interaction intents.

2. Player control layer
- ElephantController handles surface movement, jumping, and stomp triggers.
- MouseController handles tunnel movement and mouse-hole triggers.

3. Race flow layer
- RaceManager owns round start, round end, score tracking, winner detection, and level transitions.

4. Interaction layer
- StompPoint represents an Elephant-triggered level interaction node.
- MouseHole represents a Mouse-triggered level interaction node.
- InteractionEffect is the shared abstraction for gameplay consequences.

5. Presentation layer
- Split-screen camera rig renders the top and bottom halves independently.
- UI layer shows score, round state, and winner feedback.

## Script Map

### Core Controllers

| Script | Responsibility | Key Dependencies |
| --- | --- | --- |
| RaceManager | Round lifecycle, score, winner detection, level progression | ElephantController, MouseController, UI |
| ElephantController | Surface movement, jump, stomp detection, finish-line state | Input, StompPoint |
| MouseController | Underground movement, maze navigation, mouse-hole detection, finish-line state | Input, MouseHole |

### Interaction System

| Script | Responsibility | Key Dependencies |
| --- | --- | --- |
| StompPoint | Detect Elephant activation and choose an effect | InteractionEffect |
| MouseHole | Detect Mouse activation and choose an effect | InteractionEffect |
| InteractionEffect | Base contract for world or camera-side effects | RaceManager, level objects |
| BirdAttackEffect | Initial MouseHole effect that obscures part of the Elephant view | UI, camera overlay |

### Suggested Supporting Scripts

| Script | Responsibility |
| --- | --- |
| LevelDefinition | Stores per-level checkpoints, interaction placements, and finish-line data |
| SplitScreenCameraRig | Owns the top and bottom cameras and their viewport rects |
| ScoreboardUI | Displays round count, wins, and winner state |
| RoundTransitionUI | Shows between-round and end-of-match transitions |

## Scene Hierarchy

### Recommended Scene Set

- `Boot` or persistent preload scene
- `Level_01`
- `Level_02`
- `Level_03`

### Persistent Bootstrap Hierarchy

- `GameBootstrap`
  - `RaceManager`
  - `InputRouter`
  - `MatchState`
  - `AudioRoot`

### Per-Level Scene Hierarchy

- `SceneRoot`
  - `Systems`
    - `RaceManager` reference or scene-local proxy
    - `SplitScreenCameraRig`
    - `UIRoot`
  - `World`
    - `SurfaceTrack`
    - `UndergroundMaze`
    - `LevelBoundaries`
    - `FinishLine`
    - `StompPoints`
    - `MouseHoles`
  - `Players`
    - `Elephant`
      - `SpriteRenderer`
      - `Rigidbody2D` or equivalent controller body
      - `ElephantController`
    - `Mouse`
      - `SpriteRenderer`
      - `Rigidbody2D` or equivalent controller body
      - `MouseController`
  - `Cameras`
    - `ElephantCamera`
    - `MouseCamera`
  - `LightingAndFX`
    - `Ambient`
    - `BirdAttackOverlay`
    - `ImpactFX`

## Split-Screen Plan

- Elephant camera renders the top half of the screen.
- Mouse camera renders the bottom half of the screen.
- The cameras should follow their respective players independently.
- UI should either be shared in a center-safe area or duplicated per half if readability requires it.

## Gameplay Flow

1. Load level and initialize players.
2. Start round countdown.
3. Players race toward the finish line.
4. Elephant activates Stomp Points to modify Mouse routes.
5. Mouse activates Mouse Holes to trigger effects on Elephant.
6. First player to finish wins the round.
7. RaceManager updates score and either advances to the next round or ends the match.

## MVP Implementation Plan

### Phase 1: Project Foundation

- Create script folders and class skeletons.
- Add a shared input abstraction.
- Add basic scene bootstrap and split-screen camera setup.

### Phase 2: Player Movement

- Implement Elephant left/right movement and jump.
- Implement Mouse movement with maze-friendly navigation.
- Add finish-line detection for both players.

### Phase 3: Race Flow

- Implement round start, round end, score tracking, and match winner detection.
- Add between-round UI and restart flow.

### Phase 4: Interactions

- Implement StompPoint activation.
- Implement MouseHole activation.
- Add BirdAttackEffect as the first MouseHole effect.

### Phase 5: Level Content

- Build 3 handcrafted levels using shared gameplay prefabs.
- Tune obstacle placement, route branching, and interaction cadence.

### Phase 6: Polish

- Add audio, effects, and camera polish.
- Validate readability, responsiveness, and round pacing.

## MVP Scope Guardrails

- No procedural generation.
- No online multiplayer.
- No advanced meta-progression.
- No complex combat or combat abilities beyond the defined interaction system.
