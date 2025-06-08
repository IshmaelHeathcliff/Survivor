using System;
using System.Collections.Generic;

public class ValueCounter
{
    public string ID { get; set; }
    public int Value { get; set; } = 0;
    public EasyEvent<CountChangedEvent> ValueChanged { get; set; } = new();

    public ValueCounter(string id)
    {
        ID = id;
    }

    public void IncrementCount(int amount)
    {
        Value += amount;
        ValueChanged?.Trigger(new CountChangedEvent(ID, Value));
    }

    public void DecrementCount(int amount)
    {
        Value -= amount;
        ValueChanged?.Trigger(new CountChangedEvent(ID, Value));
    }

}

public struct CountChangedEvent : ISkillReleaseEvent
{
    public string ID { get; set; }
    public int Value { get; set; }

    public CountChangedEvent(string id, int value)
    {
        ID = id;
        Value = value;
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

    public IUnRegister Register(string ID, Action<CountChangedEvent> onValueChanged)
    {
        if (!_countValues.ContainsKey(ID))
        {
            _countValues.Add(ID, new ValueCounter(ID));
        }

        return _countValues[ID].ValueChanged.Register(onValueChanged);
    }

    public void Unregister(string ID, Action<CountChangedEvent> onValueChanged)
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
