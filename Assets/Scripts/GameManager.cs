using XYZRPGSystem.Gameplay.Status;
using XYZRPGSystem.Gameplay.Modifier;
using Gameplay.Character.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using XYZRPGSystem.Gameplay.Stat;
using System.Collections.Generic;
using XYZRPGSystem.Data.SaveLoad;

public class GameManager : MonoBehaviour, IController
{
    PlayerModel _playerModel;
    ModifierSystem _modifierSystem;
    StatusCreateSystem _statusCreateSystem;
    DataPersistUtility _dataPersistUtility;

    [Button]
    public void Save()
    {
        _dataPersistUtility.SaveAllDataToFile();
    }

    [Button]
    public void Load()
    {
        _dataPersistUtility.LoadAllDataFromFile();
    }

    [Button]
    public void AddBuff()
    {
        IStatusWithTime status = _statusCreateSystem.CreateStatus("1", "player", 4, new List<int> { 20, 20, 20 });
        _playerModel.StatusContainer.AddStatus(status);
    }

    [Button]
    public void LoseHealth()
    {
        var health = _playerModel.Stats.GetStat("Health") as IConsumableStat;
        health.ChangeCurrentValue(-10);
    }

    [Button]
    public void GainHealth()
    {
        var health = _playerModel.Stats.GetStat("Health") as IConsumableStat;
        health.ChangeCurrentValue(10);
    }

    void Awake()
    {
        _modifierSystem = this.GetSystem<ModifierSystem>();
        _statusCreateSystem = this.GetSystem<StatusCreateSystem>();
        _dataPersistUtility = this.GetUtility<DataPersistUtility>();
    }

    void Start()
    {
        _playerModel = this.GetModel<PlayersModel>().Current;
        _playerModel.Stats.GetStat("WoodGain").BaseValue = 1;
        // Debug.Log("Game Start");
    }

    void Update()
    {
        // Debug.Log("Update");
    }

    public IArchitecture GetArchitecture()
    {
        return GameFrame.Interface;
    }
}
