using System;
using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Data.Config;
using GamePlay.Character;
using UnityEngine;

namespace GamePlay.Skill.Effect
{
    public class FixedRepeatEffect : NestedSkillEffect<FixedRepeatEffectConfig>
    {
        readonly CancellationTokenSource _cts;
        public FixedRepeatEffect(FixedRepeatEffectConfig skillEffectConfig, ICharacterModel model, IEnumerable<IEffect> childEffects) : base(skillEffectConfig, model, childEffects)
        {
            _cts = GlobalCancellation.GetCombinedTokenSource(Model.Controller as MonoBehaviour);
        }

        async UniTaskVoid Repeat()
        {
            while (true)
            {
                _cts.Token.ThrowIfCancellationRequested();
                await UniTask.Delay(TimeSpan.FromSeconds(SkillEffectConfig.Interval), cancellationToken: _cts.Token);
                base.OnApply();
            }
        }

        protected override void OnApply()
        {
            Repeat().Forget();
        }

        protected override void OnCancel()
        {
            _cts.Cancel();
        }
    }
}
