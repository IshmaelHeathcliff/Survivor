using Gameplay.Status;
using Gameplay.Modifier;
using Gameplay.Character.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Gameplay.Stat;
using Data.SaveLoad;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, IController
{
    PlayerModel _playerModel;
    ModifierSystem _modifierSystem;
    StatusCreateSystem _statusCreateSystem;
    SaveLoadUtility _saveLoadUtility;

    [Button]
    public void Save()
    {
        _saveLoadUtility.SaveAllDataToFile();
    }

    [Button]
    public void Load()
    {
        _saveLoadUtility.LoadAllDataFromFile();
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
        _saveLoadUtility = this.GetUtility<SaveLoadUtility>();
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
