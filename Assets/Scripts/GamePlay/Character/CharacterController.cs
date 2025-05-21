using Character.Damage;
using Character.Modifier;
using Character.Stat;
using UnityEngine;
using Core;

namespace Character
{
    public interface ICharacterController : ICanInit
    {
        IAttackerController AttackerController { get; }
        IDamageable Damageable { get; }
        IMoveController MoveController { get; }
        ICharacterModel Model { get; }
        Stats Stats { get; }
        void DestroyController();
    }



    public interface ICharacterControlled : ICanInit
    {
        ICharacterController CharacterController { get; set; }
    }

    public abstract class CharacterControlled : MonoBehaviour, ICharacterControlled
    {
        public bool Initialized { get; set; }
        public ICharacterController CharacterController { get; set; }

        public void Init()
        {
            if (CharacterController == null)
            {
                CharacterController = GetComponentInParent<ICharacterController>();
            }

            if (!CharacterController.Initialized)
            {
                CharacterController.Init();
            }

            OnInit();
            Initialized = true;
        }

        public void Deinit()
        {
            OnDeinit();
            CharacterController.Deinit();
            Initialized = false;
        }

        protected abstract void OnInit();
        protected abstract void OnDeinit();

        protected virtual void Awake()
        {
            if (!Initialized)
            {
                Init();
            }
        }

    }

    public abstract class MyCharacterController : MonoBehaviour, IController, ICharacterController
    {
        [SerializeField] string _characterId;

        protected ModifierSystem ModifierSystem;

        public IAttackerController AttackerController { get; protected set; }
        public IDamageable Damageable { get; protected set; }
        public IMoveController MoveController { get; protected set; }
        public ICharacterModel Model { get; protected set; }

        public bool Initialized { get; set; }

        // ! 由于Model和ID都需要在子类的Awake中设置，所以Stats不能在Awake中调用
        public Stats Stats => Model.Stats;

        public string ID
        {
            get => _characterId;
            set => _characterId = value;
        }

        protected T GetController<T>() where T : ICharacterControlled
        {
            T controller = GetComponentInChildren<T>();
            controller.CharacterController = this;
            return controller;
        }

        protected abstract void SetStats();

        public void DestroyController()
        {
            Destroy(gameObject);
        }

        public void Init()
        {
            OnInit();

            ModifierSystem = this.GetSystem<ModifierSystem>();
            Stats.FactoryID = ID;
            ModifierSystem.RegisterFactory(Stats);
            Initialized = true;
        }

        public void Deinit()
        {
            OnDeinit();
            Initialized = false;
        }

        /// <summary>
        /// 需要获取AttackerController, MoveController, Damageable, Model的引用
        /// </summary>
        protected abstract void OnInit();

        protected abstract void OnDeinit();

        protected virtual void Awake()
        {
            if (!Initialized)
            {
                Init();
            }
        }

        protected virtual void Start()
        {
            SetStats();
        }

        protected virtual void OnDisable()
        {
            ModifierSystem.UnregisterFactory(ID);
        }

        protected virtual void OnDestroy()
        {
            if (Initialized)
            {
                Deinit();
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }

    public abstract class CharacterControllerWithFSM<T> : MyCharacterController, IHasFSM<T> where T : struct, System.Enum
    {
        public FSM<T> FSM { get; private set; } = new();

        protected abstract void AddStates();

        protected virtual void Update()
        {
            FSM.Update();
        }

        protected virtual void FixedUpdate()
        {
            FSM.FixedUpdate();
        }

        protected override void Start()
        {
            base.Start();
            AddStates();
        }
    }
}
