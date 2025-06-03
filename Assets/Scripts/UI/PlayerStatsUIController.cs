using System.Text;
using Character.Player;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Character.Stat
{
    public class PlayerStatsUIController : MonoBehaviour, IController
    {
        [SerializeField] TextMeshProUGUI _text;

        PlayerModel _playerModel;

        [Button]
        // TODO: 优化属性更新方式，不再一次性更新所有
        void UpdateStatsInfo()
        {
            var info = new StringBuilder();
            info.Append(GenerateStatInfo(_playerModel.Stats.Health));
            info.Append(GenerateStatInfo(_playerModel.Stats.Mana));
            info.Append(GenerateStatInfo(_playerModel.Stats.Damage));
            info.Append(GenerateStatInfo(_playerModel.Stats.Accuracy));

            _text.text = info.ToString();
        }

        StringBuilder GenerateStatInfo(IStat stat)
        {
            var info = new StringBuilder();
            info.Append($"{stat.Name}: {(int)stat.Value}\n");
            info.Append($"  {stat.Name}基础值: {(int)stat.BaseValue}\n");
            info.Append($"  {stat.Name}附加值: {(int)stat.AddedValue}\n");
            info.Append($"  {stat.Name}固定值: {(int)stat.FixedValue}\n");
            info.Append($"  {stat.Name}提高: {(int)stat.Increase}%\n");
            info.Append($"  {stat.Name}总增: {(int)((stat.More - 1) * 100)}%\n");
            return info;
        }

        void Start()
        {
            _playerModel = this.GetModel<PlayersModel>().Current();
            foreach (IStat stat in _playerModel.Stats.GetAllStats())
            {
                stat.Register(UpdateStatsInfo);
            }
            UpdateStatsInfo();
        }

        void OnValidate()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
