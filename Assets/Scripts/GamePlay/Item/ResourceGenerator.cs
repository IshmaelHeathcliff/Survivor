using System.Threading;
using Cysharp.Threading.Tasks;
using GamePlay.Character;
using GamePlay.Item;
using UnityEngine;

public class ResourceGenerator
{
    readonly ResourceSystem _resourceSystem;
    readonly ICharacterModel _model;
    public float Interval { get; set; }

    public ResourceGenerator(ResourceSystem resourceSystem, ICharacterModel model, float interval)
    {
        _resourceSystem = resourceSystem;
        _model = model;
        Interval = interval;
    }

    public void GenerateResource(string resourceID, int amount)
    {
        if (_model is IHasResources resources)
        {
            _resourceSystem.AcquireResource(resourceID, amount, resources);
        }
    }

    public async UniTask StartGenerating(CancellationToken cancellationToken)
    {
        float leftTime = Interval;

        while (!cancellationToken.IsCancellationRequested)
        {
            await UniTask.WaitForFixedUpdate(cancellationToken);
            leftTime -= Time.fixedDeltaTime;
            if (leftTime <= 0)
            {
                GenerateResource("Coin", (int)_model.Stats.GetStat("CoinGain").Value);
                leftTime += Interval;
            }
        }
    }
}
