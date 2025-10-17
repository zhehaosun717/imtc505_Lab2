# Lab2 VR Demo

A Unity VR project for Meta Quest 3 with interactive features including ball shooting, grabbing, scoring system, and locomotion.

## Features

### Locomotion System
- **Continuous Movement**: Move using left joystick (head-direction based)
- **Snap Turn**: Rotate view using right joystick (45° increments)
- **Sprint**: Optional sprint functionality

### Interaction System
- **Ball Shooter**: Shoot colored balls with right trigger
  - 8 blue-to-white gradient colors
  - Physics-based projectiles
  - Configurable lifetime (default 20 seconds)
  - Audio feedback support
- **Grab System**: Pick up balls with left trigger
  - Physics-based grabbing
  - Throw mechanics with velocity tracking
  - Configurable grab radius

### Scoring System
- **Interactive Button**: Push-able button triggered by ball collisions
- **Score Manager**: Singleton score management system
- **VR UI Display**: World-space score display
- **Combo System**: Optional combo multiplier (configurable)

## Controls

| Action | Input |
|--------|-------|
| Move | Left Joystick |
| Turn | Right Joystick |
| Shoot Ball | Right Trigger |
| Grab Object | Left Trigger |

## Configuration

### Ball Shooter Settings
- **Shoot Force**: Projectile velocity (default: 10)
- **Ball Lifetime**: Time before disappearing (default: 20s)
- **Shoot Sound**: Optional audio clip
- **Sound Volume**: Audio volume (0-1)

### Grab Settings
- **Grab Radius**: Detection radius (default: 0.3m)
- **Throw Multiplier**: Throw force multiplier (default: 1.5)

### Button Settings
- **Push Distance**: How far button moves (default: 0.1m)
- **Return Speed**: Button return speed (default: 5)
- **Trigger Threshold**: Activation threshold (default: 0.08)

### Score System
- **Score Per Hit**: Points per button hit (default: 1)
- **Max Score**: Maximum score limit (default: 9999)
- **Enable Combo**: Enable combo system
- **Combo Time Window**: Time window for combo (default: 2s)

