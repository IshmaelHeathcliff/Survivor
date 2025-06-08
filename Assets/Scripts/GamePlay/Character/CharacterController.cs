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
        ICharacterModel CharacterModel { get; }
        Stats Stats { get; }

        void DestroyController();
    }

    // 修复泛型约束，TModel 需要有无参构造函数
    public interface ICharacterController<TModel> : ICharacterController
        where TModel : ICharacterModel, new()
    {
        TModel Model { get; }
    }



    public abstract class MyCharacterController<TModel, TModels> : MonoBehaviour, IController, ICharacterController<TModel>
        where TModel : ICharacterModel, new()
        where TModels : CharactersModel<TModel>
    {
        [SerializeField] string _characterId;
        [SerializeField] int _baseHealth;
        [SerializeField] int _baseMana;
        [SerializeField] int _baseAccuracy;

        protected ModifierSystem ModifierSystem;

        public IAttackerController AttackerController { get; protected set; }
        public IDamageable Damageable { get; protected set; }
        public IMoveController MoveController { get; protected set; }
        public ICharacterModel CharacterModel => Model;

        public TModel Model { get; protected set; }

        public bool Initialized { get; set; }

        // ! Model、ID、Stats都需要在初始化后再调用
        public Stats Stats => Model.Stats;

        public string ID
        {
            get => _characterId;
            set => _characterId = value;
        }


        protected TControlled GetControlled<TControlled>() where TControlled : ICharacterControlled
        {
            TControlled controlled = GetComponentInChildren<TControlled>();
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

        protected abstract void MakeSureID();

        void MakeSureModel()
        {
            MakeSureID();
            TModels models = this.GetModel<TModels>();
            TModel model = models.AddModel(ID);
            model.BindTransform(MoveController.Transform);
            models.Current = model;
            Model = model;
        }

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
            this.GetModel<TModels>().RemoveModel(ID);
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

    public abstract class CharacterControllerWithFSM<TModel, TModels, TFSMID> : MyCharacterController<TModel, TModels>, IHasFSM<TFSMID>
        where TFSMID : struct, System.Enum
        where TModel : ICharacterModel, new()
        where TModels : CharactersModel<TModel>
    {
        public FSM<TFSMID> FSM { get; private set; } = new();

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
