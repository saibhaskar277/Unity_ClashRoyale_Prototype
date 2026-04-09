# ⚔️ Clash Royale Style Game - Unity
currently its in developing stage 

A scalable **Clash Royale–inspired strategy game prototype** built in **Unity**, following **SOLID principles**, **state-driven AI**, **ScriptableObject-based unit configs**, and **clean architecture**.

This project focuses on:
- Clean scalable architecture
- Unit combat & targeting
- Tower attack system
- NavMesh movement
- ScriptableObject unit configuration
- State machine based unit behavior
- Air / Ground targeting separation
- Extensible Clash Royale style mechanics

---

Features

 Core Gameplay
- Unit spawning system
- Tower attack system
- Health & damage system
- Melee and ranged attacks
- Air and ground units
- Tower-only units (Giant behavior)
- Retargeting while moving
- Attack lock while attacking

---
Architecture
Built using **SOLID principles** and clean modular systems:

Systems
- `HealthManager`
- `AttackingSystem`
- `TargetingSystem`
- `UnitStateController`
- `TowerSystem`
- `EventManager`
- `Timer`

AI States
- `IdleState`
- `MoveState`
- `AttackState`

Data Driven
All units use **ScriptableObjects**:

