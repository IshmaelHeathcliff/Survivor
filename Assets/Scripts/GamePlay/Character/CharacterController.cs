using Character.Damage;
using Character.Modifier;
using Character.Stat;
using UnityEngine;
using Core;
using UnityEngine.AddressableAssets;

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



    public abstract class MyCharacterController : MonoBehaviour, IController, ICharacterController
    {
        [SerializeField] string _characterId;
        [SerializeField] int _baseHealth;
        [SerializeField] int _baseMana;
        [SerializeField] int _baseAccuracy;

        protected ModifierSystem ModifierSystem;

        public IAttackerController AttackerController { get; protected set; }
        public IDamageable Damageable { get; protected set; }
        public IMoveController MoveController { get; protected set; }
        public ICharacterModel Model { get; protected set; }

        public bool Initialized { get; set; }

        // ! Model、ID、Stats都需要在初始化后再调用
        public Stats Stats => Model.Stats;

        public string ID
        {
            get => _characterId;
            set => _characterId = value;
        }

        protected T GetControlled<T>() where T : ICharacterControlled
        {
            T controlled = GetComponentInChildren<T>();
            controlled.CharacterController = this;
            return controlled;
        }

        protected virtual void SetStats()
        {
            Stats.Health.BaseValue = _baseHealth;
            Stats.Mana.BaseValue = _baseMana;
            Stats.Accuracy.BaseValue = _baseAccuracy;

            Stats.Health.SetMaxValue();
            Stats.Mana.SetMaxValue();
        }

        protected abstract void MakeSureModel();

        public void DestroyController()
        {
            Addressables.ReleaseInstance(gameObject);
        }

        public void Init()
        {
            if (Initialized)
            {
                return;
            }

            AttackerController = GetControlled<IAttackerController>();
            MoveController = GetControlled<IMoveController>();
            Damageable = GetControlled<IDamageable>();

            ModifierSystem = this.GetSystem<ModifierSystem>();

            // 初始化ID、Model、Stats
            MakeSureModel();

            OnInit();
            Stats.FactoryID = ID;
            ModifierSystem.RegisterFactory(Stats);
            SetStats();

            Initialized = true;
        }

        public void Deinit()
        {
            OnDeinit();
            Initialized = false;
        }

        /// <summary>
        /// 需要完成ID的初始化并获取Model的引用
        /// </summary>
        protected abstract void OnInit();

        protected virtual void OnDeinit()
        {
            ModifierSystem.UnregisterFactory(ID);
        }

        protected virtual void Awake()
        {
            if (!Initialized)
            {
                Init();
            }
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

        protected override void OnInit()
        {
            AddStates();
        }

        protected override void OnDeinit()
        {
            base.OnDeinit();
            FSM.Clear();
        }
    }
}
