# VR Test Demo - Meta Quest 3

A Unity VR project for Meta Quest 3 with interactive features including ball shooting, grabbing, scoring system, and locomotion.

## Features

### 🎮 Locomotion System
- **Continuous Movement**: Move using left joystick (head-direction based)
- **Snap Turn**: Rotate view using right joystick (45° increments)
- **Sprint**: Optional sprint functionality

### 🎯 Interaction System
- **Ball Shooter**: Shoot colored balls with right trigger
  - 8 blue-to-white gradient colors
  - Physics-based projectiles
  - Configurable lifetime (default 20 seconds)
  - Audio feedback support
- **Grab System**: Pick up balls with left trigger
  - Physics-based grabbing
  - Throw mechanics with velocity tracking
  - Configurable grab radius

### 🏆 Scoring System
- **Interactive Button**: Push-able button triggered by ball collisions
- **Score Manager**: Singleton score management system
- **VR UI Display**: World-space score display
- **Combo System**: Optional combo multiplier (configurable)

### 🛠️ Editor Tools
- **VR Locomotion Setup Wizard**: One-click locomotion system setup
- **Score System Setup Wizard**: Automated scoring system configuration

## Project Structure

```
Assets/
├── Scripts/
│   ├── VRBallShooter.cs          # Ball shooting system
│   ├── VRGrabbable.cs            # Grabbable object component
│   ├── VRGrabber.cs              # Grab controller
│   ├── VRButton.cs               # Interactive button
│   ├── ScoreManager.cs           # Score management
│   ├── ScoreUI.cs                # Score display UI
│   ├── VRContinuousMove.cs       # Movement controller
│   ├── VRSnapTurn.cs             # Rotation controller
│   ├── VRLocomotionManager.cs    # Locomotion system manager
│   ├── VRLocomotionDebugger.cs   # Debug tool
│   └── Editor/
│       ├── VRLocomotionSetupWizard.cs
│       └── ScoreSystemSetupWizard.cs
├── Scenes/
├── Materials/
├── Prefabs/
└── ... (other Unity folders)
```

## Requirements

- **Unity Version**: 2021.3 LTS or newer (recommended)
- **XR Interaction Toolkit**: Required
- **TextMeshPro**: Required for UI
- **Target Device**: Meta Quest 3
- **Input System**: New Unity Input System

## Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/YOUR_USERNAME/VR_TestDemo.git
cd VR_TestDemo
```

### 2. Open in Unity
- Open Unity Hub
- Click "Open" and select the project folder
- Wait for Unity to import all assets

### 3. Configure VR Settings
1. Go to **Edit → Project Settings → XR Plug-in Management**
2. Enable **Oculus** platform
3. Configure XR Interaction Toolkit settings

### 4. Setup Locomotion System
1. In Unity, go to **Tools → VR Locomotion Setup Wizard**
2. Click "Auto Find Objects"
3. Select features to setup
4. Click "Start Setup"

### 5. Setup Score System
1. Go to **Tools → VR Score System Setup Wizard**
2. Configure positions and options
3. Click "Start Setup"

### 6. Configure Input Actions
- Open the Input Actions asset
- Bind left joystick to Move Action
- Bind right joystick to Turn Action
- Bind right trigger to Shoot Action
- Bind left trigger to Grab Action

## Usage

### In Unity Editor
- Use the setup wizards for quick configuration
- Test in Play mode with XR Device Simulator

### On Meta Quest 3
1. Build the project (**File → Build Settings**)
2. Select Android platform
3. Connect Quest 3 via USB
4. Build and Run

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

## Development

### Code Style
- All scripts are documented with XML comments
- English language for code and comments
- Debug logs removed for production

### Adding Features
1. Create new scripts in `Assets/Scripts/`
2. Follow existing naming conventions
3. Add XML documentation comments
4. Test in both Editor and Quest 3

## Troubleshooting

### Common Issues

**Q: Locomotion not working?**
- Check Input Action bindings in Inspector
- Ensure XR Origin has VRContinuousMove and VRSnapTurn components

**Q: Balls not colliding with button?**
- Verify button has Rigidbody (non-kinematic) with FreezeAll constraints
- Check collision layers

**Q: Score not displaying?**
- Ensure ScoreManager exists in scene
- Check Canvas render mode is set to World Space
- Verify ScoreUI component is properly configured

## License

[Add your license here]

## Credits

Developed for Meta Quest 3 VR experience.

## Contact

[Add your contact information]

