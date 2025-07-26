using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using Gameplay.Skill;
using XYZRPGSystem.Gameplay.Modifier;
using UnityEngine;
using XYZRPGSystem.Gameplay.Character;
using XYZRPGSystem.Gameplay.Item;
using XYZRPGSystem.Gameplay.Skill;
using XYZRPGSystem.Gameplay.Stat;

namespace Gameplay.Character.Player
{
    public class PlayerController : MyCharacterController<PlayerModel, PlayersModel>
    {
        [SerializeField] Vector3 _initialPosition;
        [SerializeField] string _skillPoolConfigPath;


        public void Respawn()
        {
            Model.Position = _initialPosition;
            CharacterStats.GetConsumableStat("Health").SetMaxValue();
        }

        protected override void SetStats()
        {
            base.SetStats();
            CharacterStats.LoadStats("PlayerStats.json", "Preset/JSON");
        }

        protected override void MakeSureID()
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = "player";
            }
        }

        protected override void OnInit()
        {
            SkillReleaseSystem skillReleaseSystem = this.GetSystem<SkillReleaseSystem>();
            skillReleaseSystem.RegisterConditions(Model);
            skillReleaseSystem.RegisterRelease(Model);
        }

        protected override void OnDeinit()
        {
            base.OnDeinit();
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            this.GetSystem<SkillGachaSystem>().InitSkillPool(Model, _skillPoolConfigPath);
            new ResourceGenerator(this.GetSystem<ResourceSystem>(), Model, 1f).StartGenerating(GlobalCancellation.GetCombinedTokenSource(this).Token).Forget();
        }
    }
}
