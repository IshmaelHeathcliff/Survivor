using System;
using System.Collections.Generic;

public class ValueCounter
{
    public string ID { get; set; }
    public int Value { get; set; } = 0;
    public EasyEvent<int> ValueChanged { get; set; } = new();

    public ValueCounter(string id)
    {
        ID = id;
    }

    public void IncrementCount(int amount)
    {
        Value += amount;
        ValueChanged?.Trigger(Value);
    }

    public void DecrementCount(int amount)
    {
        Value -= amount;
        ValueChanged?.Trigger(Value);
    }

}

public class CountSystem : AbstractSystem
{
    Dictionary<string, ValueCounter> _countValues = new();

    public int GetCount(string ID)
    {
        if (!_countValues.ContainsKey(ID))
        {
            _countValues.Add(ID, new ValueCounter(ID));
        }

        return _countValues[ID].Value;
    }

    public IUnRegister Register(string ID, Action<int> onValueChanged)
    {
        if (!_countValues.ContainsKey(ID))
        {
            _countValues.Add(ID, new ValueCounter(ID));
        }

        return _countValues[ID].ValueChanged.Register(onValueChanged);
    }

    public void Unregister(string ID, Action<int> onValueChanged)
    {
        if (_countValues.ContainsKey(ID))
        {
            _countValues[ID].ValueChanged.UnRegister(onValueChanged);
        }
    }

    public void IncrementKillCount(int amount)
    {
        IncrementCount("KillCount", amount);
    }

    public void IncrementCount(string ID, int amount)
    {
        if (!_countValues.ContainsKey(ID))
        {
            _countValues.Add(ID, new ValueCounter(ID));
        }

        _countValues[ID].IncrementCount(amount);
    }

    protected override void OnInit()
    {
        _countValues.Add("KillCount", new ValueCounter("KillCount"));
    }

}
