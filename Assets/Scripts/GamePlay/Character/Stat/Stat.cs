using System;
using System.Collections.Generic;
using System.Linq;
using Character.Modifier;

namespace Character.Stat
{
    public interface IStat : IReadonlyBindableProperty<float>
    {
        string Name { get; }
        float BaseValue { get; set; }
        float AddedValue { get; }
        float FixedValue { get; }
        float Increase { get; }
        float More { get; }
        float GetValue();
        float GetValue(float baseValue, float addedMultiplier = 1);
        void AddAddedValueModifier(string key, IStatModifier<float> mod);
        void AddFixedValueModifier(string key, IStatModifier<float> mod);
        void AddIncreaseModifier(string key, IStatModifier<float> mod);
        void AddMoreModifier(string key, IStatModifier<float> mod);
        void RemoveAddedValueModifier(string key);
        void RemoveFixedValueModifier(string key);
        void RemoveIncreaseModifier(string key);
        void RemoveMoreModifier(string key);
    }

    public class Stat : IStat
    {
        float _baseValue;
        public string Name { get; private set; }
        public float Value => GetValue();
        public float BaseValue
        {
            get => _baseValue;
            set
            {
                _baseValue = value;
                _onValueChanged.Trigger(Value);
            }
        }

        public virtual float AddedValue => AddedValueModifiers.Sum(x => x.Value.Value);
        public virtual float FixedValue => FixedValueModifiers.Sum(x => x.Value.Value);
        public virtual float Increase => IncreaseModifiers.Sum(x => x.Value.Value);
        public virtual float More => MoreModifiers.Aggregate(1f, (acc, mod) => acc * ((float)mod.Value.Value / 100 + 1));

        protected Dictionary<string, IStatModifier<float>> AddedValueModifiers = new();
        protected Dictionary<string, IStatModifier<float>> IncreaseModifiers = new();
        protected Dictionary<string, IStatModifier<float>> MoreModifiers = new();
        protected Dictionary<string, IStatModifier<float>> FixedValueModifiers = new();


        readonly EasyEvent<float> _onValueChanged = new();

        public Stat(string name)
        {
            Name = name;
        }


        public void AddAddedValueModifier(string key, IStatModifier<float> mod)
        {
            AddedValueModifiers[key] = mod;
            _onValueChanged.Trigger(Value);
        }

        public void AddFixedValueModifier(string key, IStatModifier<float> mod)
        {
            FixedValueModifiers[key] = mod;
            _onValueChanged.Trigger(Value);
        }

        public void AddIncreaseModifier(string key, IStatModifier<float> mod)
        {
            IncreaseModifiers[key] = mod;
            _onValueChanged.Trigger(Value);
        }

        public void AddMoreModifier(string key, IStatModifier<float> mod)
        {
            MoreModifiers[key] = mod;
            _onValueChanged.Trigger(Value);
        }


        public void RemoveAddedValueModifier(string key)
        {
            AddedValueModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveFixedValueModifier(string key)
        {
            FixedValueModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveIncreaseModifier(string key)
        {
            IncreaseModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        public void RemoveMoreModifier(string key)
        {
            MoreModifiers.Remove(key);
            _onValueChanged.Trigger(Value);
        }

        protected float Calculate(float addedMultiplier = 1)
        {
            return (BaseValue + AddedValue * addedMultiplier) * (1 + Increase / 100f) * More + FixedValue;
        }


        public float GetValue()
        {
            return Calculate();
        }

        public float GetValue(float baseValue, float addedMultiplier = 1)
        {
            BaseValue = baseValue;
            return Calculate(addedMultiplier);
        }

        public IUnRegister Register(Action onValueChanged)
        {
            return Register(Action);
            void Action(float _) => onValueChanged();
        }

        public IUnRegister Register(Action<float> onValueChanged)
        {
            return _onValueChanged.Register(onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(Action<float> onValueChanged)
        {
            onValueChanged(GetValue());
            return Register(onValueChanged);
        }

        public void UnRegister(Action<float> onValueChanged)
        {
            _onValueChanged.UnRegister(onValueChanged);
        }


    }

    public interface IConsumableStat : IStat
    {
        float CurrentValue { get; }
        void ChangeCurrentValue(float value);
        void SetCurrentValue(float value);
        void SetMaxValue();
    }

    public class ConsumableStat : Stat, IConsumableStat
    {
        float _currentValue;
        public float CurrentValue
        {
            get => _currentValue;
            private set
            {
                _currentValue = value;
                _onValueChanged.Trigger(value, Value);
            }
        }

        readonly EasyEvent<float, float> _onValueChanged = new();

        public float CheckValue(float value)
        {
            float maxValue = GetValue();
            if (value < 0)
            {
                value = 0;
            }

            if (value > maxValue)
            {
                value = maxValue;
            }

            return value;
        }

        public void ChangeCurrentValue(float value)
        {
            float newCurrentValue = CurrentValue + value;
            CurrentValue = CheckValue(newCurrentValue);
        }

        public void SetCurrentValue(float value)
        {
            CurrentValue = CheckValue(value);
        }

        public void SetMaxValue()
        {
            CurrentValue = GetValue();
        }

        public ConsumableStat(string name) : base(name)
        {
            CurrentValue = GetValue();
        }

        public IUnRegister Register(Action<float, float> onValueChanged)
        {
            return _onValueChanged.Register(onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(Action<float, float> onValueChanged)
        {
            onValueChanged(CurrentValue, GetValue());
            return Register(onValueChanged);
        }

        public void UnRegister(Action<float, float> onValueChanged)
        {
            _onValueChanged.UnRegister(onValueChanged);
        }
    }


    public interface IKeywordStat : IStat
    {
        IEnumerable<string> KeywordsToQuery { get; set; }
        float GetValueByKeywords(IEnumerable<string> keywords);
        float GetValueByKeywords(float baseValue, IEnumerable<string> keywords, float addedMultiplier = 1);
    }


    public class KeywordStat : Stat, IKeywordStat
    {
        public IEnumerable<string> KeywordsToQuery { get; set; }

        public KeywordStat(string name, IEnumerable<string> keywords) : base(name)
        {
            KeywordsToQuery = keywords;
        }

        // ! 只要包含任意一个关键词，就会计算在内
        public override float AddedValue => AddedValueModifiers.Values.Where(x => x.Keywords.All(KeywordsToQuery.Contains)).Sum(x => x.Value);
        public override float Increase => IncreaseModifiers.Values.Where(x => x.Keywords.All(KeywordsToQuery.Contains)).Sum(x => x.Value);
        public override float More => MoreModifiers.Values.Where(x => x.Keywords.All(KeywordsToQuery.Contains)).Aggregate(1f, (acc, mod) => acc * ((float)mod.Value / 100 + 1));
        public override float FixedValue => FixedValueModifiers.Values.Where(x => x.Keywords.All(KeywordsToQuery.Contains)).Sum(x => x.Value);


        public float GetValueByKeywords(IEnumerable<string> keywords)
        {
            KeywordsToQuery = keywords;
            return Calculate();
        }

        public float GetValueByKeywords(float baseValue, IEnumerable<string> keywords, float addedMultiplier = 1)
        {
            KeywordsToQuery = keywords;
            BaseValue = baseValue;
            return Calculate(addedMultiplier);
        }
    }


}
