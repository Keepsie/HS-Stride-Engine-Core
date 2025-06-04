// HS Stride Engine Core (c) 2025 Happenstance Games LLC - MIT License

using Happenstance.SE.Logger.Core;
using Happenstance.SE.Logger.Interfaces;
using Stride.Core;
using Stride.Engine;
using System.Linq;
using System.Threading.Tasks;

namespace Happenstance.SE.Core
{
    public abstract class HSAsyncScript : AsyncScript
    {

        //
        [DataMember]
        public bool StartDisabled { get; set; }
        private bool _hasInitialized = false;

        //
        protected IHSLogger Logger { get; private set; }
        protected HSEntityFinder EntityFinder { get; private set; }
        private HSEnableComponent _enableComponent;
        private bool _isDestroyed = false;
        public bool IsEnabled { get; private set; }

        public override sealed async Task Execute()
        {
            EntityFinder = new HSEntityFinder(SceneSystem);

            Logger = EntityFinder.FindAllComponents<HSLogger>().FirstOrDefault();
            if (Logger == null)
            {
                Log.Warning($"[{GetType().Name}] Could not find HSLogger");
                Logger = new HSLoggerDummy();
            }

            //If starting diabled call before connecting the on disable or enable and awake
            if (StartDisabled)
            {
                OnStartDisabled();
            }

            _enableComponent = Entity.GetOrCreate<HSEnableComponent>();
            _enableComponent.RegisterCallbacks(
                () => HandleEnable(),
                () => OnDisable(),
                EnableWatcher
            );

            IsEnabled = _enableComponent.Enabled;


            if (!StartDisabled)
            {
                Initialize();

                // Only run OnExecute if the entity starts enabled
                await OnExecute();
            }
            else
            {
                // For disabled entities, we can wait indefinitely until they're enabled
                // This keeps the task alive but not actively running
                while (StartDisabled && !_hasInitialized)
                {
                    await Script.NextFrame();
                }

                // Once enabled and initialized, then execute
                if (IsEnabled)
                {
                    await OnExecute();
                }
            }
        }

        private void Initialize()
        {
            if (!_hasInitialized)
            {
                OnAwake();
                OnStart();
                _hasInitialized = true;
            }
        }

        public void Destroy()
        {
            Entity.Scene.Entities.Remove(Entity);
        }

        private void OnStartDisabled() => SetActive(false);

        public void SetActive(bool active)
        {
            Entity.EnableAll(active, true);
        }

        public override sealed void Cancel()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;

            OnDestroy();
            base.Cancel();
        }

        private void HandleEnable()
        {
            // Run initialization if this is first enable
            if (!_hasInitialized)
            {
                Initialize();
            }

            // Always run normal enable logic
            OnEnable();
        }

        private void EnableWatcher(bool enable)
        {
            IsEnabled = enable;
        }


        /// <summary>
        /// Makes target Entity a parent of this entity
        /// </summary>
        /// <param name="child"></param>
        public void SetParent(Entity parent)
        {
            Entity.Transform.Parent = parent?.Transform;
        }

        /// <summary>
        /// Makes target Entity a child of this entity
        /// </summary>
        /// <param name="child"></param>
        public void SetChild(Entity child)
        {
            if (child != null)
            {
                child.Transform.Parent = Entity.Transform;
            }
        }

        /// <summary>
        /// Removes this entity's parent (makes it a root entity)
        /// </summary>
        public void ClearParent()
        {
            Entity.Transform.Parent = null;
        }

        /// <summary>
        /// Removes a specific child from this entity (makes it a root entity)
        /// </summary>
        /// <param name="child">The child entity to remove</param>
        public void ClearChild(Entity child)
        {
            if (child != null && child.Transform.Parent == Entity.Transform)
            {
                child.Transform.Parent = null;
            }
        }

        /// <summary>
        /// Removes all children from this entity (makes them root entities)
        /// Note: This clears ALL children - no selective removal
        /// </summary>
        public void ClearChildren()
        {
            // Need to iterate through a copy since we're modifying the collection
            var children = Entity.Transform.Children.ToList();

            foreach (var child in children)
            {
                child.Parent = null;
            }
        }

        public virtual void OnAwake() { }
        public virtual void OnStart() { }
        public virtual async Task OnExecute()
        {
            await Task.CompletedTask;
        }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }
    }
}
