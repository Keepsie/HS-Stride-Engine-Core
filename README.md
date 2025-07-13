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
var enemy = Entity.Scene.FindEntityByName_HS("Enemy"); // HS utility
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
    protected override void OnAwake() { /* initialization */ }
    protected override void OnStart() { /* after awake */ }
    protected override void OnUpdate() { /* your update logic */ }
    protected override void OnEnable() { /* when enabled */ }
    protected override void OnDisable() { /* when disabled */ }
}
```

## ✨ Core Features

### 🔄 Enhanced Script Lifecycle
Unity-style lifecycle methods that work exactly as expected.

**HSAsyncScript / HSSyncScript / HSStartupScript:**
```csharp
protected override void OnAwake()    // Called first, for initialization
protected override void OnStart()    // Called after Awake, for setup  
protected override void OnUpdate()   // Called every frame (SyncScript only)
protected override Task OnExecute()  // Called for async operations (AsyncScript only)
protected override void OnEnable()   // Called when entity/component enabled
protected override void OnDisable()  // Called when entity/component disabled  
protected override void OnDestroy()  // Called when entity destroyed
protected override void OnTriggerEnter(Entity other) // Called when another entity enters trigger
protected override void OnTriggerExit(Entity other)  // Called when another entity exits trigger
```

**StartDisabled Support:**
```csharp
[DataMember]
public bool StartDisabled { get; set; } // Set in Stride Studio
```
- Script won't initialize until manually enabled
- Perfect for objects that should spawn inactive
- Handles lifecycle correctly when enabled later

### 🎯 HSOnTriggerComponent - Unity-Style Trigger Events
Automatic trigger detection system that provides Unity-familiar OnTriggerEnter/OnTriggerExit callbacks for Stride's Bullet physics system.

## ⚠️ **IMPORTANT: Enable CollisionDetection First**
**You MUST enable `CollisionDetection = true` in the inspector or constructor before the script starts.** This cannot be changed at runtime due to performance optimization.

```csharp
public class TriggerZone : HSSyncScript
{
    public TriggerZone()
    {
        // ✅ Enable collision detection at construction
        CollisionDetection = true;
    }
    
    // OR set CollisionDetection = true in the Stride inspector
}
```

## **Automatic Integration:**
```csharp
// HSOnTriggerComponent is automatically added to all HS scripts
// Just override the methods you need:
public class TriggerZone : HSSyncScript
{
    protected override void OnTriggerEnter(Entity other)
    {
        Logger.Info($"Entity {other.Name} entered trigger zone");
        
        // Check for specific components
        var player = other.Get<PlayerController>();
        if (player != null)
        {
            OnPlayerEntered(player);
        }
    }
    
    protected override void OnTriggerExit(Entity other)
    {
        Logger.Info($"Entity {other.Name} left trigger zone");
    }
}
```

## **Requirements:**
- **CollisionDetection = true** in inspector or constructor
- Entity must have a `PhysicsComponent`
- Works with Stride's current Bullet physics system
- Will be updated when Bepu physics becomes stable

## **Performance Design:**
The system uses an **early return pattern** for optimal performance:
- Scripts with `CollisionDetection = false` have **zero overhead**
- Only entities that need triggers consume CPU cycles
- **Cannot be enabled at runtime** - this is intentional for performance

If you need to enable/disable triggers at runtime, you'll need custom implementation:

## **Features:**
- **Automatic Cleanup:** Dead entities are properly removed from trigger tracking
- **Performance Optimized:** Periodic cleanup prevents memory leaks  
- **Enable/Disable Control:** Use `CollisionDetection` property in inspector
- **Thread Safe:** Handles async collision detection properly
- **Zero Overhead:** Scripts without collision detection have no performance cost

## **Common Use Cases:**
```csharp
// ✅ Trigger zones (enable CollisionDetection)
public class CheckpointZone : HSSyncScript { }

// ✅ Pickups (enable CollisionDetection) 
public class HealthPack : HSSyncScript { }

// ✅ Damage zones (enable CollisionDetection)
public class LavaZone : HSSyncScript { }

// ✅ Regular scripts (leave CollisionDetection = false)
public class WeaponController : HSSyncScript { }
public class AnimationController : HSSyncScript { }
```

## **Debugging Tips:**
- Check that `CollisionDetection = true` in inspector
- Verify entity has `PhysicsComponent`
- Use Logger to confirm OnTriggerEnter/Exit are being called
- Remember: Only the **receiving** entity needs collision detection enabled

### 🔍 HSEntity, HSScene, HSTransform - Extension Method APIs
Unity-style extension methods that make entity management, scene searching, and transform operations intuitive and clean.

**⚠️ Extension Method Naming**
All extension methods use `_HS` suffix to prevent future conflicts with Stride Engine updates. This ensures the API remains stable even if Stride adds similar methods later.

### 🎯 HSEntity - Entity Hierarchy Management
Find children, get components, and manage entity relationships with Unity-familiar syntax.

**Find Children:**
```csharp
// Find child by name (immediate children only)
Entity weapon = player.FindChildByName_HS("Weapon");

// Find child by name (recursive through hierarchy)
Entity scope = player.FindChildByNameRecursive_HS("Scope");
```

**Component from Children:**
```csharp
// Get component from immediate child
AudioEmitterComponent audio = player.GetComponentFromChild_HS<AudioEmitterComponent>("AudioSource");

// Get component from anywhere in hierarchy
ModelComponent model = player.GetComponentFromChildRecursive_HS<ModelComponent>("PlayerModel");
```

**Unity-Style Component Finding:**
```csharp
// Get all components of type in immediate children
List<Light> lights = player.GetComponentsInChildren_HS<Light>();

// Get all components in entire hierarchy (recursive)
List<Weapon> allWeapons = player.GetComponentsInAllDescendants_HS<Weapon>();

// Find component in parent entities (walking up hierarchy)
GameManager manager = someEntity.GetComponentInParent_HS<GameManager>();
```

**UI Element Finding:**
```csharp
// Find UI elements in entity's UI hierarchy
Button startButton = entity.GetUIElement_HS<Button>("StartButton");
List<Button> allButtons = entity.GetUIElements_HS<Button>();
```

### 🌍 HSScene - Scene-Wide Searching
Find entities and components throughout the entire scene with clean extension syntax.

**Find by Name:**
```csharp
// Find any entity by name (recursive through entire scene)
Entity player = Entity.Scene.FindEntityByName_HS("Player");
```

**Find by Component:**
```csharp
// Find all entities that have a specific component
List<Entity> cameras = Entity.Scene.FindEntitiesWithComponent_HS<CameraComponent>();

// Find all components of a specific type in the scene
List<AudioEmitterComponent> audioSources = Entity.Scene.FindAllComponents_HS<AudioEmitterComponent>();

// Find components that implement an interface
List<IDamageable> damageables = Entity.Scene.FindAllComponentsWithInterface_HS<IDamageable>();
```

### 🎯 HSTransform - Unity-Style Transform Operations
Clean, semantic transform operations that make rotation, positioning, and directional calculations simple and intuitive.

**LookAt Operations (Rotation Made Simple):**
```csharp
// Instant snap to face target - no complex matrix math needed!
entity.Transform.LookAt_HS(player.Transform);
entity.Transform.LookAt_HS(player);                    // Convenience overload
entity.Transform.LookAt_HS(targetPosition);

// Smooth rotation towards target - perfect for AI
entity.Transform.SmoothLookAt_HS(player.Transform, rotationSpeed, deltaTime);
entity.Transform.SmoothLookAt_HS(targetPosition, rotationSpeed, deltaTime);
```

**Rotation & Euler Angles (Unity-Style Simplicity):**
```csharp
// Get rotation as degrees - no quaternion math!
Vector3 rotation = entity.Transform.GetEulerAngles_HS();

// Set rotation from degrees - incredibly simple
entity.Transform.SetEulerAngles_HS(new Vector3(45, 90, 0));

// Convert between quaternion and euler easily
Vector3 euler = someQuaternion.ToEulerAngles_HS();
Quaternion quat = HSTransform.FromEulerAngles_HS(new Vector3(pitch, yaw, roll));

// Smooth rotation control
entity.Transform.SmoothRotateTo_HS(targetRotation, speed, deltaTime);
```

**Distance Calculations:**
```csharp
// Clean distance methods - no manual Vector3.Distance calls
float distance = entity.Transform.DistanceFrom_HS(target.Transform);
float distance = entity.Transform.DistanceFrom_HS(target);          // Convenience
float distance = entity.Transform.DistanceFrom_HS(worldPosition);

// Performance optimized for comparisons
float distSq = entity.Transform.DistanceSquaredFrom_HS(target.Transform);
```

**Direction & Positioning:**
```csharp
// Clean directional access
Vector3 forward = entity.Transform.GetForward_HS();
Vector3 right = entity.Transform.GetRight_HS();
Vector3 up = entity.Transform.GetUp_HS();

// Position utilities
Vector3 pos = entity.Transform.GetWorldPosition_HS();      // Force update
Vector3 pos = entity.Transform.GetWorldPositionFast_HS(); // When matrix is current

// Direction calculations for AI
float yaw = HSTransform.DirectionToYaw_HS(moveDirection);
float angle = entity.Transform.AngleTo_HS(target.Transform); // Vision cones
Quaternion lookRot = HSTransform.LookRotation_HS(direction);
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
Entity.SetParent_HS(parentEntity);   // Make this entity a child of parent
Entity.SetChild_HS(anyEntity);       // Make ANY entity a child of this one
Entity.ClearParent_HS();             // Remove from parent (become root)
Entity.ClearChild_HS(specificChild); // Remove specific child
Entity.ClearChildren_HS();           // Remove all children
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
    protected override void OnStart()
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
    protected override void OnUpdate() 
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
    protected override async Task OnExecute()
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
    protected override void OnStart()
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
    
    protected override void OnAwake()
    {
        Instance = this;
    }
}
```

**Component Communication:**
```csharp
public class PlayerHealth : HSSyncScript
{
    protected override void OnStart()
    {
        var ui = Entity.Scene.FindAllComponents_HS<HealthUI>().FirstOrDefault();
        ui?.UpdateHealthDisplay(currentHealth);
    }
}
```

## 🔧 Integration Examples

### Controlling Any Entity's Hierarchy
```csharp
public class SpawnManager : HSSyncScript
{
    protected override void OnStart()
    {
        // Spawn enemies and organize them under this manager
        var enemy1 = SpawnEnemy();
        var enemy2 = SpawnEnemy();
        var pickupItem = SpawnPickup();
        
        // Parent ANY entity to this manager (even non-HS entities)
        Entity.SetChild_HS(enemy1);     // Now enemy1 is child of SpawnManager
        Entity.SetChild_HS(enemy2);     // Now enemy2 is child of SpawnManager
        Entity.SetChild_HS(pickupItem); // Even pickup items can be organized
        
        // Much easier than: enemy1.Transform.Parent = Entity.Transform;
        
        Logger.Info($"Spawned and organized {Entity.Transform.Children.Count} entities");
    }
    
    public void CleanupAllSpawned()
    {
        // Remove all spawned entities from hierarchy (makes them root entities)
        Entity.ClearChildren_HS();
    }
}
```

### Finding and Configuring Systems
```csharp
public class GameBootstrap : HSStartupScript
{
    protected override void OnStart()
    {
        // Find all managers in the scene
        var audioManager = Entity.Scene.FindAllComponents_HS<AudioManager>().FirstOrDefault();
        var inputManager = Entity.Scene.FindAllComponents_HS<InputManager>().FirstOrDefault();
        
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
    protected override async Task OnExecute()
    {
        while (IsEnabled)
        {
            if (HSRandom.DiceRoll(spawnChance))
            {
                var enemy = SpawnEnemy();
                
                // Instantly organize under this spawner (works on ANY entity!)
                Entity.SetChild_HS(enemy); // Much easier than enemy.Transform.Parent = Entity.Transform
                
                Logger.Debug($"Spawned enemy, total children: {Entity.Transform.Children.Count}");
            }
            
            await Script.NextFrame();
        }
    }
    
    public void DespawnAllEnemies()
    {
        // Easy cleanup - remove all children from hierarchy
        Entity.ClearChildren_HS();
    }
}
```

### Unity-Style Trigger Detection
```csharp
public class PickupItem : HSSyncScript
{
    [DataMember] public float healAmount = 25f;
    [DataMember] public AudioComponent pickupSound;
    
    protected override void OnTriggerEnter(Entity other)
    {
        // Check if player entered the pickup area
        var player = other.Get<PlayerController>();
        if (player != null)
        {
            // Heal the player
            var health = player.Entity.Get<PlayerHealth>();
            health?.Heal(healAmount);
            
            // Play pickup sound
            pickupSound?.Play();
            
            Logger.Info($"Player picked up health item: +{healAmount} HP");
            
            // Remove the pickup item
            Entity.Scene.Entities.Remove(Entity);
        }
    }
}
```

```csharp
public class DamageZone : HSSyncScript
{
    [DataMember] public float damagePerSecond = 10f;
    [DataMember] public string damageType = "Fire";
    
    private HashSet<Entity> entitiesInDamageZone = new HashSet<Entity>();
    
    protected override void OnTriggerEnter(Entity other)
    {
        var damageable = other.Get<IDamageable>();
        if (damageable != null)
        {
            entitiesInDamageZone.Add(other);
            Logger.Info($"{other.Name} entered {damageType} damage zone");
        }
    }
    
    protected override void OnTriggerExit(Entity other)
    {
        if (entitiesInDamageZone.Contains(other))
        {
            entitiesInDamageZone.Remove(other);
            Logger.Info($"{other.Name} left {damageType} damage zone");
        }
    }
    
    protected override void OnUpdate()
    {
        // Apply damage to all entities in zone
        foreach (var entity in entitiesInDamageZone)
        {
            var damageable = entity.Get<IDamageable>();
            damageable?.TakeDamage(damagePerSecond * (float)Game.UpdateTime.Elapsed.TotalSeconds);
        }
    }
}
```

```csharp
public class TeleportPad : HSSyncScript
{
    [DataMember] public Entity destinationPad;
    [DataMember] public AudioComponent teleportSound;
    [DataMember] public ParticleSystemComponent teleportEffect;
    
    protected override void OnTriggerEnter(Entity other)
    {
        // Only teleport entities with a transform and physics
        if (other.Get<TransformComponent>() != null && other.Get<RigidbodyComponent>() != null)
        {
            TeleportEntity(other);
        }
    }
    
    private void TeleportEntity(Entity entity)
    {
        if (destinationPad == null) return;
        
        // Play effects
        teleportSound?.Play();
        teleportEffect?.Play();
        
        // Teleport to destination
        entity.Transform.Position = destinationPad.Transform.Position;
        
        Logger.Info($"Teleported {entity.Name} to {destinationPad.Name}");
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
protected override void OnStart()
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
protected override void OnAwake() { }
protected override void OnStart() { }
protected override void OnUpdate() { }
protected override async Task OnExecute() { }
```

### Performance Considerations
- EntityFinder methods are recursive and can be expensive on large scenes
- Cache frequently accessed entities/components
- Use specific finding methods rather than broad searches when possible

### Logging Integration
- **Setup Required:** Copy HSLogger prefab into scene OR attach HSLogger component to any entity
- Scripts will log errors if HSLogger is not found in the scene
- Logger is automatically discovered and cached by all HS scripts
- Set **`DebugMode = true`** on HSLogger for development, `false` for release 
- Error logs are always saved to file regardless of debug mode

### Entity Control
- **SetActive()** only works on entities with HS scripts (enables/disables the entity hierarchy)
- **Parent/Child management** works on ANY entity in the scene, even non-HS entities
- **Entity.SetChild_HS(anyEntity)** from any HS script can parent ANY entity to it
- **Much easier than Stride's manual Transform.Parent manipulation**
- **Perfect for dynamic spawning, organization, and hierarchy management**

### Dynamic Spawning Benefits
```csharp
// Instead of this (Stride way):
spawnedEntity.Transform.Parent = managerEntity.Transform;

// You get this (HS way):
managerEntity.SetChild_HS(spawnedEntity);
```

## 🛠️ Extending the System

### Custom Base Scripts
```csharp
public abstract class MyCustomScript : HSSyncScript
{
    protected MyGameManager GameManager { get; private set; }
    
    protected override void OnAwake()
    {
        base.OnAwake();
        GameManager = Entity.Scene.FindAllComponents_HS<MyGameManager>().FirstOrDefault();
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
    protected override void OnStart()
    {
        // Find all damageable objects in scene
        var damageables = Entity.Scene.FindAllComponentsWithInterface_HS<IDamageable>();
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
