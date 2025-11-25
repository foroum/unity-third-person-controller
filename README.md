# Unity 3D – Third Person Controller

Reusable third-person controller prototype built in Unity.

This project is a small standalone testbed for:
- Player movement in 3D
- Third-person camera control
- Basic movement animations
- Input System setup

A lot of the logic here can be reused in other projects.

---

## 1. Project Overview

This repository contains a **general 3D third-person controller** in Unity.  
It focuses on clean, readable code and modular systems that can be dropped into other projects.

### Implemented

- New **Input System** setup (actions for move, look, jump, sprint - even if some are WIP)
- **Player movement** on flat ground (forward / backward / strafe)
- **Camera follow + orbit** around the player
- Basic **movement animation** (idle / walk or run blend) driven by movement input
- Jumping (with proper grounding checks)
- Sprinting (with speed multiplier + animation)
- Falling / landing states
- Better behaviour on **stairs and slopes**
- Extra polish on camera collision and smoothing

### In Progress / Planned
- TBD


---

## 2. Gameplay / Controls

_Default keyboard & mouse setup:_

- **W / A / S / D** – Move the character
- **Mouse movement** – Rotate camera around the player
- **Space** – Jump (logic currently in progress)
- **Left Shift** – Sprint (logic currently in progress)

These are mapped using Unity’s **New Input System** via an `InputActions` asset, with actions such as:

- `Player/Move` – Vector2 movement
- `Player/Look` – Mouse delta / right stick
- `Player/Jump` – Button
- `Player/Sprint` – Button

---

## 3. Technical Details

- **Engine:** Unity (2022.3.62f2)
- **Render Pipeline:** URP
- **Language:** C#
- **Target Platform:** PC (Windows)

### Main Systems

- **Player Movement**
  - Reads input from the New Input System
  - Moves the player relative to camera direction
  - Normalized movement to keep consistent speed

- **Camera Controller**
  - Smooth follow of the player
  - Mouse-controlled orbit
  - Clamped vertical rotation to avoid flipping under/over the level

- **Animation Controller**
  - Animator with basic states (Idle, Walk/Run)
  - Blend based on movement magnitude
  - Hooks ready for future states (Jump, Fall, Land, Sprint)


