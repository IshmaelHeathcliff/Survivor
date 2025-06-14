using System.Collections.Generic;
using System.Linq;

namespace GamePlay.Character.Stat
{
    public interface IKeywordStat : IStat
    {
        IEnumerable<string> KeywordsToQuery { get; set; }
        float GetValueByKeywords(IEnumerable<string> keywords);
        float GetValueByKeywords(float baseValue, IEnumerable<string> keywords, float addedMultiplier = 1);
    }


    public class KeywordStat : Stat, IKeywordStat
    {
        public IEnumerable<string> KeywordsToQuery { get; set; }

        public KeywordStat(string id, IEnumerable<string> keywords) : base(id)
        {
            KeywordsToQuery = keywords;
        }

        // ! 只要包含任意一个关键词，就会计算在内
        public override float AddedValue => AddedValueModifiers.Values.Where(x => x.Keywords.All(KeywordsToQuery.Contains)).Sum(x => x.Value);
        public override float Increase => IncreaseModifiers.Values.Where(x => x.Keywords.All(KeywordsToQuery.Contains)).Sum(x => x.Value);
        public override float More => MoreModifiers.Values.Where(x => x.Keywords.All(KeywordsToQuery.Contains)).Aggregate(1f, (acc, mod) => acc * ((float)mod.Value / 100 + 1));
        public override float FixedValue => FixedValueModifiers.Values.Where(x => x.Keywords.All(KeywordsToQuery.Contains)).Sum(x => x.Value);


        public float GetValueByKeywords(IEnumerable<string> keywords)
        {
            KeywordsToQuery = keywords;
            return Calculate();
        }

        public float GetValueByKeywords(float baseValue, IEnumerable<string> keywords, float addedMultiplier = 1)
        {
            KeywordsToQuery = keywords;
            BaseValue = baseValue;
            return Calculate(addedMultiplier);
        }
    }
}
