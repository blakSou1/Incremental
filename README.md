<div align="center">

# Incremental

![Unity](https://img.shields.io/badge/Unity-6000.3.19f1-000000?logo=unity&logoColor=white&style=flat-square)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white&style=flat-square)
![ShaderLab](https://img.shields.io/badge/ShaderLab-76B900?logo=unity&logoColor=white&style=flat-square)
![Status](https://img.shields.io/badge/Status-WIP-yellow?style=flat-square)

A grid-based board-game prototype in Unity: flip pieces on a matrix, fight through level-based
runs, and beat an AI opponent. Data-driven, editor-friendly, built for fast iteration.

</div>

---

## Overview

**Incremental** is a turn-based board game prototype built around a square matrix. Pieces are
placed on a grid and flip captured pieces (Reversi-style rules), while levels add combat,
enemy HP and win/lose conditions on top. Progression is split into levels configured from
the editor through a CMS-style data layer — no scene edits needed to add content.

> ⚠️ The project is a work in progress: the core loop works, but content, balance and UX are still being built out.

## Key features

- **Matrix board** — `MatrixModel` / `GridBox` / `GridController` own the grid state; visual boxes render each cell.
- **Piece system** — `PieceFactory` spawns pieces from config; rules are pluggable (`ReversiPieceRule`, base `PieceRule`), pieces have rarities, modifiers and special types (`PieceShield`, `PieceFullMove`).
- **Flip mechanics** — captured pieces flip color (`FlipOfPiece`), combos are computed per move.
- **Combat layer** — enemy HP, damage, win/lose conditions per level (`HpManager`, `EnemyModel`, `ConditionsOfVictoryAndDefeat`).
- **AI opponent** — built-in `AI` and rule "brains" (`FightLvlBrain`, `TutorialBrain`) drive enemy moves and a step-by-step tutorial.
- **Level config from editor** — `ConfigGame` / `ConfigLevel` + CMS entities; levels are pure data, loaded by index at runtime.
- **Service layer** — static `G` service locator + `EventManager`, pause handling (`ManagedBehaviour.PausableUpdate`), audio/scene services, pause panel and FAQ.
- **Juice** — DOTween, TextMesh Pro, Febucci text animation, AllIn1 Sprite Shader, VFX and animation folders per piece/enemy.

## Tech stack

| | |
|---|---|
| Engine | Unity `6000.3.19f1` (see `ProjectSettings/ProjectVersion.txt`) |
| Language | C#, ShaderLab/HLSL for shaders |
| Data | CMS Megurt — entity/component definitions authored in the editor |
| Libraries | DOTween, TextMesh Pro, Febucci Text Animator, AllIn1 Sprite Shader, SerializeReference Extensions, new Input System |

## Project structure

```
Assets/
├── MineGame/
│   ├── Scripts/
│   │   ├── Config/        # ConfigGame, ConfigLevel, ConfigMain — level data
│   │   ├── Game/
│   │   │   ├── Boards/    # Matrix, MatrixField, GridBox, GridController
│   │   │   ├── Pieces/    # PieceBase, PieceFactory, Shield, FullMove, Modifiers
│   │   │   ├── Combat/    # HpManager, EnemyModel, win/loss conditions
│   │   │   ├── Brains/    # AI brains: fight level, tutorial
│   │   │   ├── Players/   # PlayerController, AI
│   │   │   ├── Controllers/ # GameLogic, PieceController
│   │   │   ├── Core/      # Bootstrap, MainEnterPoint, G, EventManager
│   │   │   └── View/      # piece/enemy views, win/loss animation
│   │   ├── Services/      # Audio, SceneLoader, UI, pause, FAQ
│   │   ├── Ui/            # HUD and panels
│   │   └── Utils/
│   ├── Animation/  Material/  Prefabs/  VFX/  Visuals/
├── Plugins/        # CMS Megurt, DOTween, Inputs, TextMesh Pro, …
├── Resources/  Settings/
└── Packages/  ProjectSettings/
```

## Getting started

1. Open the project in **Unity 6000.3.19f1** (Unity Hub → Add → select repo root).
2. Let the editor import packages and regenerate `.csproj`/`.sln`.
3. Press **Play** — bootstrap lives in `MineGame/Scripts/Game/Core/Bootstrap.cs` / `MainEnterPoint.cs`.
4. Levels and piece definitions are edited via the CMS data assets, not in scenes.

```bash
git clone https://github.com/blakSou1/Incremental.git
cd Incremental
# open with Unity Hub (6000.3.19f1)
```

## Roadmap

- [ ] Finish level content and progression pacing
- [ ] Balance piece modifiers and enemy brains
- [ ] Polish tutorial flow
- [ ] Sound and juice pass

## License

No license added yet — all rights reserved by default. If you want others to use the code, add an MIT license file.
