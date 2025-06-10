using Character.State;
using Character.Modifier;
using Character.Player;
using SaveLoad;
using Sirenix.OdinInspector;
using UnityEngine;
using Character.Stat;

public class GameManager : MonoBehaviour, IController
{
    PlayerModel _playerModel;
    ModifierSystem _modifierSystem;
    StateCreateSystem _stateCreateSystem;
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
        IStateWithTime state = _stateCreateSystem.CreateState("1", "player", new[] { 20, 20, 20 }, 4);
        _playerModel.StateContainer.AddState(state);
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
        _stateCreateSystem = this.GetSystem<StateCreateSystem>();
        _saveLoadUtility = this.GetUtility<SaveLoadUtility>();
    }

    void Start()
    {
        _playerModel = this.GetModel<PlayersModel>().Current;
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
