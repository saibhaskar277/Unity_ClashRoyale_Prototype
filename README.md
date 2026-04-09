# ⚔️ Clash Royale Style Game - Unity

> 🚧 **Currently in Development**

A scalable **Clash Royale–inspired real-time strategy card battler prototype** built in **Unity**, following **SOLID principles, clean architecture, state-driven AI, and ScriptableObject-based unit configuration**.

This project is designed as a **production-ready foundation** for building a polished multiplayer strategy card game with scalable gameplay systems.

---

# 🎯 Project Vision

The main goal of this project is to build a **modular and extensible RTS card battler architecture** that supports:

- ⚔️ Real-time troop battles
- 🏰 Tower defense mechanics
- 🎴 Deck-based troop spawning
- 🧠 Smart unit AI
- 🐉 Air and ground movement systems
- 🔥 Scalable future multiplayer support

---

# 🎮 Current Features

## ⚔️ Gameplay Systems
- ✅ Unit spawning system
- ✅ Elixir-based card deployment
- ✅ Clash Royale style deck UI
- ✅ Card cycling after deployment
- ✅ Health and damage system
- ✅ Melee and ranged combat
- ✅ Tower attack system
- ✅ Dynamic retargeting while moving
- ✅ Attack cooldown system using reusable timer
- ✅ Attack lock while attacking
- ✅ Unit death handling
- ✅ Tower-only unit logic *(Giant style)*
- ✅ Air / ground targeting separation
- ✅ Flying unit support architecture *(Dragon / Minions ready)*

---

## 🧠 AI State Machine

Each troop uses a **scalable state-driven AI controller**.

### Current States
- `IdleState`
- `MoveState`
- `AttackState`

### Current Behaviors
- Find nearest enemy
- Move toward target
- Retarget while moving
- Lock movement during attack
- Resume movement after target death
- Custom target logic support
- Tower prioritization support

This architecture allows easy creation of units like:

- 🗡️ Knight
- 🏹 Archer
- 👹 Goblin
- 🎯 Dart Goblin
- 🪨 Giant
- 🐉 Baby Dragon

---

# 📦 Data Driven Design

All troops are configured using **ScriptableObjects**.

Each `UnitData` contains:

- Unit ID
- Unit prefab
- Card sprite
- Health
- Damage
- Move speed
- Attack range
- Attack cooldown
- Elixir cost
- Attack type *(Melee / Ranged)*
- Movement type *(Ground / Air)*
- Target preference
- Tower priority support

This allows:

- ⚡ Quick balancing
- 🎴 Easy card additions
- 🔧 No code changes for stats
- 🚀 Designer-friendly workflow

---

# 🏗️ Architecture

Built using **SOLID principles + clean modular architecture**.

## 🔧 Core Systems
- `HealthManager`
- `AttackingSystem`
- `TargetingSystem`
- `TowerSystem`
- `UnitSpawnSystem`
- `UnitDeckUI`
- `UnitCardView`
- `UnitStateController`
- `Timer`
- `EventManager`

---

# 🧩 Design Patterns Used

This project uses multiple scalable software patterns:

- ✅ **State Pattern** → troop AI
- ✅ **Strategy Pattern** → movement & targeting
- ✅ **Observer/Event Pattern** → game events
- ✅ **ScriptableObject Data Pattern** → unit configs
- ✅ **Dependency Injection style references**
- ✅ **Interface-driven architecture**

---

# 🚀 Future Roadmap

Planned features:

- [ ] Multiplayer support
- [ ] Better card animations
- [ ] Unit abilities
- [ ] Spell cards
- [ ] Wave system
- [ ] Match timer
- [ ] Crown tower victory logic
- [ ] Better VFX / SFX
- [ ] Mobile optimization
- [ ] Event-driven UI refresh
- [ ] Air movement strategy
- [ ] Unit pooling system
