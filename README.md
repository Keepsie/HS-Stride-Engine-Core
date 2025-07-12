# HS Stride Engine Core

A foundational library for Stride Engine that provides Unity-style script lifecycle, improved entity management, and essential utilities for game development.

## 🎯 Why This Library Exists

Stride Engine is powerful but lacks some conveniences that Unity developers expect. This core library bridges that gap by providing:

**The Problem:** Stride's base scripts don't match common game development patterns:
- No `OnAwake()`, `OnStart()`, `OnEnable()`, `OnDisable()` lifecycle
- No built-in entity finding utilities  
- No consistent random number generation
- No easy way to start scripts disabled
- Manual enable/disable management is verbose

**The Solution:** Drop-in base classes that work exactly like Unity's MonoBehaviour but for Stride.

## 🔄 Seamless Integration with Existing Projects

**Zero Breaking Changes** - HS Core works alongside your existing Stride code:

✅ **Mix and Match:** Use HS scripts and regular Stride scripts in the same project  
✅ **Gradual Migration:** Convert scripts one at a time, no pressure  
✅ **Existing Code Safe:** Your current Stride scripts continue working unchanged  
✅ **Drop-in Addition:** Add HS Core to existing projects without modification  

### Migration Example
```csharp
// Your existing Stride script - STILL WORKS
public class OldEnemyAI : SyncScript 
{
    public override void Update() { /* existing logic */ }
}

// Your new HS script - ALSO WORKS  
public class NewPlayerController : HSSyncScript
{
    public override void OnUpdate() { /* new lifecycle */ }
}

// They can even communicate normally!
var enemy = EntityFinder.FindEntityByName("Enemy"); // HS utility
var aiScript = enemy.Get<OldEnemyAI>();             // Standard Stride
```

## 🚀 Installation

### Recommended: One-Click Installation
1. **Download [HS Stride Packer](https://github.com/Keepsie/HS-Stride-Packer)**
2. **Download** this library's `.stridepackage` file from [Releases](https://github.com/Keepsie/HS-Stride-Engine-Core/releases)
3. **Import** using HS Stride Packer - everything installs automatically!

### Alternative: Manual Installation
- Code is open source - you can still copy files manually if preferred
- See legacy installation instructions 
- HS Stride Packer handles all dependencies and setup automatically

### Prerequisites
- Stride Engine 4.2.0.2381 or newer
- HS Stride Packer (for easy installation)
```csharp
// Before (Stride way)
public class MyScript : SyncScript
{
    public override void Start() { /* everything mixed together */ }
    public override void Update() { /* your update logic */ }
}

// After (Happenstance way)  
public class MyScript : HSSyncScript
{
    public override void OnAwake() { /* initialization */ }
    public override void OnStart() { /* after awake */ }
    public override void OnUpdate() { /* your update logic */ }
    public override void OnEnable() { /* when enabled */ }
    public override void OnDisable() { /* when disabled */ }
}
```

## ✨ Core Features

### 🔄 Enhanced Script Lifecycle
Unity-style lifecycle methods that work exactly as expected.

**HSAsyncScript / HSSyncScript / HSStartupScript:**
```csharp
public override void OnAwake()    // Called first, for initialization
public override void OnStart()    // Called after Awake, for setup  
public override void OnUpdate()   // Called every frame (SyncScript only)
public override void OnExecute()  // Called for async operations (AsyncScript only)
public override void OnEnable()   // Called when entity/component enabled
public override void OnDisable()  // Called when entity/component disabled  
public override void OnDestroy()  // Called when entity destroyed
```

**StartDisabled Support:**
```csharp
[DataMember]
public bool StartDisabled { get; set; } // Set in Stride Studio
```
- Script won't initialize until manually enabled
- Perfect for objects that should spawn inactive
- Handles lifecycle correctly when enabled later

### 🔍 HSEntityFinder - Advanced Entity Management
Powerful utilities for finding entities and components throughout your scene.

**Find by Name:**
```csharp
// Find any entity by name (recursive through children)
Entity player = EntityFinder.FindEntityByName("Player");

// Find child by name (immediate children only)
Entity weapon = EntityFinder.FindChildByName(player, "Weapon");

// Find child by name (recursive through hierarchy)
Entity scope = EntityFinder.FindChildByNameRecursive(player, "Scope");
```

**Find by Component:**
```csharp
// Find all entities that have a specific component
List<Entity> cameras = EntityFinder.FindEntitiesWithComponent<CameraComponent>();

// Find all components of a specific type in the scene
List<AudioEmitterComponent> audioSources = EntityFinder.FindAllComponents<AudioEmitterComponent>();

// Find components that implement an interface
List<IDamageable> damageables = EntityFinder.FindAllComponentsWithInterface<IDamageable>();
```

**Component from Child:**
```csharp
// Get component from immediate child
AudioEmitterComponent audio = EntityFinder.GetComponentFromChild<AudioEmitterComponent>(player, "AudioSource");

// Get component from anywhere in hierarchy
ModelComponent model = EntityFinder.GetComponentFromChildRecursive<ModelComponent>(player, "PlayerModel");
```

**UI Element Finding:**
```csharp
// Find UI elements in UI pages
Button startButton = EntityFinder.GetUIElement<Button>(mainMenuPage, "StartButton");
List<Button> allButtons = EntityFinder.GetUIElements<Button>(mainMenuPage);
```

### 🎲 HSRandom - Consistent Random Utilities
Thread-safe random number generation with game-development focused methods.

**Basic Random:**
```csharp
// Random float between min and max (inclusive)
float damage = HSRandom.Range(10.0f, 25.0f);

// Random int between min and max (max exclusive)
int coins = HSRandom.Range(1, 10); // Returns 1-9

// Random value 0.0 to 1.0
float chance = HSRandom.Value;
```

**Game-Specific Random:**
```csharp
// Random item from list
List<string> weapons = new List<string> {"Sword", "Bow", "Staff"};
string randomWeapon = HSRandom.GetRandom(weapons);

// Percentage-based dice roll
if (HSRandom.DiceRoll(25f)) // 25% chance
{
    SpawnRareItem();
}

// Multiple dice rolls with success threshold
bool criticalHit = HSRandom.MultiDiceRoll(50f, 3, 2); // 3 rolls at 50%, need 2 successes
```

**Advanced Random:**
```csharp
// Weighted random selection
List<string> items = new List<string> {"Common", "Rare", "Epic"};
List<float> weights = new List<float> {70f, 25f, 5f};
string loot = HSRandom.WeightedRandom(items, weights);

// Unique random selection (no duplicates)
List<string> randomCards = HSRandom.GetRandomUnique(allCards, 5);

// Bell curve distribution (values cluster toward middle)
float statRoll = HSRandom.BellCurve(1f, 20f, 3); // More likely to roll ~10-11
```
## ⚙️ Entity Management

```csharp
// Enable/disable (HS scripts only)
SetActive(false); // Disables this HS script entity and all children

// Parent/child management (works on ANY entity, even non-HS entities!)
SetParent(parentEntity);   // Make this entity a child of parent
SetChild(anyEntity);       // Make ANY entity a child of this one
ClearParent();             // Remove from parent (become root)
ClearChild(specificChild); // Remove specific child
ClearChildren();           // Remove all children
```

### 🗣️ HSLogger - Advanced Logging System
Controlled logging system with debug mode, file output, and multiple destinations.

**Why HSLogger over Stride's default logging:**
- **Debug Mode Control:** Toggle debug logging on/off without code changes
- **Automatic File Logging:** Saves error logs to files for release builds
- **Multiple Outputs:** Console + file logging simultaneously
- **Tag Removal:** Cleans up formatted messages for file output
- **Centralized Control:** One place to manage all logging behavior

**Automatic Logger Access in HS Scripts:**
```csharp
public class MyScript : HSSyncScript
{
    public override void OnStart()
    {
        Logger.Debug("Debug info (only shows if DebugMode = true)");
        Logger.Info("Script started successfully");
        Logger.Warning("This is a warning");
        Logger.Error("Something went wrong"); // Always logged to error_log.txt
    }
}
```

**HSLogger Configuration:**
```csharp
// In your scene, add HSLogger component
public class GameLogger : HSLogger
{
    public override void Start()
    {
        DebugMode = true; // Enable debug logging and game_log.txt
        base.Start();
    }
}
```

**Release vs Debug Benefits:**
- **Development:** Full logging to console + file with debug info
- **Release:** Error-only file logging, no console spam
- **Troubleshooting:** Users can send you error_log.txt files for support
- **Performance:** Zero debug logging overhead when DebugMode = false
- `%APPDATA%/Happenstance/YourGame/Logs/game_log.txt` (debug mode only)
- `%APPDATA%/Happenstance/YourGame/Logs/error_log.txt` (always enabled)

### 🌍 System Language Support
Enumeration for handling different system languages in your game.

```csharp
// HSSystemLanguage enum includes all major languages
HSSystemLanguage currentLang = HSSystemLanguage.English;
```

## 🏗️ Architecture Patterns

### Script Types

**HSSyncScript** - For frame-based updates:
```csharp
public class PlayerController : HSSyncScript
{
    public override void OnUpdate() 
    { 
        HandleInput();
        UpdateMovement(); 
    }
}
```

**HSAsyncScript** - For async operations:
```csharp
public class GameManager : HSAsyncScript
{
    public override async Task OnExecute()
    {
        await LoadGameData();
        await InitializeSystems();
        StartGameplay();
    }
}
```

**HSStartupScript** - For one-time initialization:
```csharp
public class GameInitializer : HSStartupScript
{
    public override void OnStart()
    {
        SetupGameSettings();
        InitializeServices();
    }
}
```

### Common Patterns

**Manager Pattern:**
```csharp
public class AudioManager : HSSyncScript
{
    public static AudioManager Instance { get; private set; }
    
    public override void OnAwake()
    {
        Instance = this;
    }
}
```

**Component Communication:**
```csharp
public class PlayerHealth : HSSyncScript
{
    public override void OnStart()
    {
        var ui = EntityFinder.FindAllComponents<HealthUI>().FirstOrDefault();
        ui?.UpdateHealthDisplay(currentHealth);
    }
}
```

## 🔧 Integration Examples

### Controlling Any Entity's Hierarchy
```csharp
public class SpawnManager : HSSyncScript
{
    public override void OnStart()
    {
        // Spawn enemies and organize them under this manager
        var enemy1 = SpawnEnemy();
        var enemy2 = SpawnEnemy();
        var pickupItem = SpawnPickup();
        
        // Parent ANY entity to this manager (even non-HS entities)
        SetChild(enemy1);     // Now enemy1 is child of SpawnManager
        SetChild(enemy2);     // Now enemy2 is child of SpawnManager
        SetChild(pickupItem); // Even pickup items can be organized
        
        // Much easier than: enemy1.Transform.Parent = Entity.Transform;
        
        Logger.Info($"Spawned and organized {Entity.Transform.Children.Count} entities");
    }
    
    public void CleanupAllSpawned()
    {
        // Remove all spawned entities from hierarchy (makes them root entities)
        ClearChildren();
    }
}
```

### Finding and Configuring Systems
```csharp
public class GameBootstrap : HSStartupScript
{
    public override void OnStart()
    {
        // Find all managers in the scene
        var audioManager = EntityFinder.FindAllComponents<AudioManager>().FirstOrDefault();
        var inputManager = EntityFinder.FindAllComponents<InputManager>().FirstOrDefault();
        
        // Configure them
        audioManager?.Initialize();
        inputManager?.LoadBindings();
        
        Logger.Info("Game systems initialized");
    }
}
```

### Dynamic Object Spawning with Easy Organization
```csharp
public class EnemySpawner : HSAsyncScript
{
    public override async Task OnExecute()
    {
        while (IsEnabled)
        {
            if (HSRandom.DiceRoll(spawnChance))
            {
                var enemy = SpawnEnemy();
                
                // Instantly organize under this spawner (works on ANY entity!)
                SetChild(enemy); // Much easier than enemy.Transform.Parent = Entity.Transform
                
                Logger.Debug($"Spawned enemy, total children: {Entity.Transform.Children.Count}");
            }
            
            await Script.NextFrame();
        }
    }
    
    public void DespawnAllEnemies()
    {
        // Easy cleanup - remove all children from hierarchy
        ClearChildren();
    }
}
```

## ⚠️ Important Notes

### Migration from Stride Base Classes
- Replace `SyncScript` with `HSSyncScript`
- Replace `AsyncScript` with `HSAsyncScript`  
- Replace `StartupScript` with `HSStartupScript`
- Move initialization from `Start()` to `OnAwake()` or `OnStart()`
- Move update logic from `Update()` to `OnUpdate()`

**⚠️ CRITICAL: Remove Base Method Calls**
**NEVER call `base.Start()`, `base.Update()`, or `base.Execute()` in HS scripts!**

```csharp
// ❌ OLD Stride Script
public override void Start() 
{
    base.Start(); // This was normal in Stride
    // your code
}

// ✅ NEW HS Script  
public override void OnStart()
{
    // base.OnStart(); // Only if needed, NO base.Start()!
    // your code
}
```

**Why:** HS scripts seal these methods to manage lifecycle. Calling `base.Start()` creates infinite recursion and **stack overflow crash**.

**⚠️ IMPORTANT: Sealed Methods**
The base Stride methods (`Start()`, `Update()`, `Execute()`, `Cancel()`) are sealed in HS scripts to prevent bugs. You **must** use the OnMethod equivalents:
```csharp
// ❌ This will cause compile errors
public override void Start() { }     // SEALED - use OnStart()
public override void Update() { }    // SEALED - use OnUpdate() 
public override Task Execute() { }   // SEALED - use OnExecute()

// ✅ Use these instead  
public override void OnAwake() { }
public override void OnStart() { }
public override void OnUpdate() { }
public override async Task OnExecute() { }
```

### Performance Considerations
- EntityFinder methods are recursive and can be expensive on large scenes
- Cache frequently accessed entities/components
- Use specific finding methods rather than broad searches when possible

### Logging Integration
- **Setup Required:** Copy HSLogger prefab into scene OR attach HSLogger component to any entity
- Scripts will log errors if HSLogger is not found in the scene
- Logger is automatically discovered and cached by all HS scripts
- Set `DebugMode = true` on HSLogger for development, `false` for release
- Error logs are always saved to file regardless of debug mode

### Entity Control
- **SetActive()** only works on entities with HS scripts (enables/disables the entity hierarchy)
- **Parent/Child management** works on ANY entity in the scene, even non-HS entities
- **SetChild(anyEntity)** from any HS script can parent ANY entity to it
- **Much easier than Stride's manual Transform.Parent manipulation**
- **Perfect for dynamic spawning, organization, and hierarchy management**

### Dynamic Spawning Benefits
```csharp
// Instead of this (Stride way):
spawnedEntity.Transform.Parent = managerEntity.Transform;

// You get this (HS way):
managerScript.SetChild(spawnedEntity);
```

## 🛠️ Extending the System

### Custom Base Scripts
```csharp
public abstract class MyCustomScript : HSSyncScript
{
    protected MyGameManager GameManager { get; private set; }
    
    public override void OnAwake()
    {
        base.OnAwake();
        GameManager = EntityFinder.FindAllComponents<MyGameManager>().FirstOrDefault();
    }
}
```

### Interface Integration
```csharp
public interface IDamageable
{
    void TakeDamage(float amount);
}

public class HealthSystem : HSSyncScript
{
    public override void OnStart()
    {
        // Find all damageable objects in scene
        var damageables = EntityFinder.FindAllComponentsWithInterface<IDamageable>();
        RegisterDamageables(damageables);
    }
}
```

## 🤝 Contributing

This library evolves based on real game development needs. If you encounter patterns that could be simplified or utilities that are missing, contributions are welcome.


By submitting a pull request, you agree to license your contribution under the MIT License.

## 📄 License

MIT License - see LICENSE.txt for full text.

**Happenstance Stride Engine Core**  
Copyright © 2025 Happenstance Games LLC
