README - Game Concept
Elephant vs Mouse
Overview

Current prototype update

The project now boots a visible test match automatically when you press Play.

What was added:

- A runtime bootstrap that creates the match systems if the scene is empty.
- Cube-shaped placeholder characters for the Elephant and the Mouse.
- Automatic match start so input works immediately without manual scene setup.
- A camera reset to full-screen view so both placeholders stay visible in the editor.

This is still a prototype layer, not the final art or final scene setup.

Prototype gameplay changes (runtime)

- Single shared camera: the scene is no longer split-screen. The Elephant occupies the top half while the Mouse occupies the bottom half.
- Camera follow: the camera follows the player who is further along the race (higher X). The camera smoothly follows the leader's X position while keeping the vertical framing so the top/bottom layout remains visible.
- Lose condition: if a trailing player falls too far behind and leaves the visible camera bounds (there is a small safety buffer so touching the edge won't immediately cause a loss), the trailing player loses the round.
- Size and movement:
	- The Elephant is three times the size of the Mouse (prototype cubes).
	- The Mouse can move in all directions but cannot jump.
	- The Elephant can move left/right and jump only.

Actions for both players (stomp, mouse-hole effects) are TODO and will be implemented later.

	Level layout update

	- The central ground/platform is now much longer so the race has more horizontal room.
	- The Mouse area now uses a smaller maze-like path made from tighter wall segments and corridor markers only below the platform.
	- The Elephant lane stays open above the ground so it can keep moving forward and jumping over obstacles.

	Match end update

	- When the match is won, a full-screen win screen appears.
	- The screen counts down and then rebuilds the runtime race layout automatically.

Elephant vs Mouse is a local 2-player competitive couch co-op racing game built in Unity.

Two players race simultaneously toward the finish line, but each character experiences a completely different path and gameplay style.

The game is played on a split-screen layout:

The Elephant runs on the upper half of the screen.
The Mouse navigates underground tunnels on the lower half of the screen.

Although both players are racing toward the same finish line, they can actively interfere with each other's progress through special interaction points placed throughout the level.

The first player to reach the finish line wins the round.

Core Gameplay
Elephant

The Elephant travels along a relatively simple above-ground path.

Abilities:

Move left and right.
Jump over obstacles.
Activate special Stomp Points.

Strengths:

Easy path navigation.
Powerful environmental influence.

Weaknesses:

Slower movement speed.
Mouse

The Mouse travels through an underground maze-like tunnel system.

Abilities:

Move through tunnels.
Navigate multiple route choices.
Activate Mouse Holes.

Strengths:

Faster movement speed.
Multiple route options.

Weaknesses:

More complex navigation.
Can be affected by Elephant actions.
Interaction System

The game's main mechanic is player interference.

Stomp Points (Elephant)

When the Elephant jumps on a Stomp Point, it can affect the Mouse's underground maze.

Examples:

Block a tunnel.
Open a shortcut.
Collapse a passage.
Redirect the Mouse to another route.
Mouse Holes (Mouse)

When the Mouse reaches a Mouse Hole, it can trigger effects against the Elephant.

Examples:

Bird Attack

A bird drops an egg onto the Elephant's screen.

Effect:

Temporarily obscures part of the Elephant player's view.
Sand Pile

Creates a pile of dirt on the Elephant's path.

Effect:

Forces the Elephant to jump over the obstacle.
Winning Condition

The first player to cross the finish line wins the round.

The game tracks score across multiple rounds.

Example:

Round 1 → Elephant wins
Round 2 → Mouse wins
Round 3 → Mouse wins

Final Score:
Mouse 2 - Elephant 1

MVP Scope
Round System
Best of 3 rounds.
Scoreboard between rounds.
Restart button.
Level Content
3 handcrafted levels.
No procedural generation.
Elephant MVP
Movement.
Jumping.
1 Stomp Point interaction.
Mouse MVP
Movement.
Maze navigation.
1 Mouse Hole interaction.
Bird Attack ability.
UI
Split-screen view.
Score display.
Winner screen.
Technical Requirements
Engine

Unity 6

Input

Single keyboard shared by both players.

Controls
Elephant
A = Move Left
D = Move Right
Space = Jump
Mouse
Left Arrow = Move Left
Right Arrow = Move Right
Up Arrow = Move Up
Down Arrow = Move Down
Design Goals

The game should create:

Friendly competition.
Chaotic moments.
Constant interaction between players.
Simple controls.
Short replayable matches (1–3 minutes).
Strong asymmetry between characters.
