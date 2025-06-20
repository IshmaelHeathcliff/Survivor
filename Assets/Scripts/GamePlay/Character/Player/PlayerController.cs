using Core;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Modifier;
using GamePlay.Character.Stat;
using GamePlay.Item;
using GamePlay.Skill;
using UnityEngine;

namespace GamePlay.Character.Player
{
    public class PlayerController : MyCharacterController<PlayerModel, PlayersModel>
    {
        [SerializeField] Vector3 _initialPosition;

        public void Respawn()
        {
            Model.Position = _initialPosition;
            (CharaterStats.GetStat("Health") as IConsumableStat)?.SetMaxValue();
        }

        protected override void SetStats()
        {
            base.SetStats();

            IStatModifier healthModifier = ModifierSystem.CreateStatModifier("health_increase", "player", 100);
            healthModifier.Register();
            (CharaterStats.GetStat("Health") as IConsumableStat)?.SetMaxValue();
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
            skillReleaseSystem.RegisterRewards(Model);
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
            new ResourceGenerator(this.GetSystem<ResourceSystem>(), Model, 1f).StartGenerating(GlobalCancellation.GetCombinedTokenSource(this).Token).Forget();
        }
    }
}
