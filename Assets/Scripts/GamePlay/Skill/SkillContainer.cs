using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface ISkillContainer
{
    ISkill GetSkill(string ID);
    bool AddSkill(ISkill skill);
    bool RemoveSkill(string ID);
    bool HasSkill(string ID);
    bool HasSkills(IEnumerable<string> IDs);
    int HasCount(IEnumerable<string> IDs);
    int MaxCount { get; set; }
    int Count { get; }
}

public class SkillContainer : ISkillContainer
{
    readonly Dictionary<string, ISkill> _skills = new();
    public int MaxCount { get; set; }
    public int Count => _skills.Count;

    public SkillContainer(int maxCount = 0)
    {
        MaxCount = maxCount;
    }

    public ISkill GetSkill(string ID)
    {
        if (_skills.TryGetValue(ID, out ISkill skill))
        {
            return skill;
        }

        return null;
    }

    public bool AddSkill(ISkill skill)
    {
        if (_skills.TryAdd(skill.ID, skill))
        {
            skill.Enable();
            return true;
        }

        return false;
    }

    public bool HasSkill(string ID)
    {
        return _skills.ContainsKey(ID);
    }

    public bool HasSkills(IEnumerable<string> IDs)
    {
        return IDs.All(HasSkill);
    }

    public int HasCount(IEnumerable<string> IDs)
    {
        int count = 0;
        foreach (string id in IDs)
        {
            if (HasSkill(id))
            {
                count++;
            }
        }

        return count;
    }

    public bool RemoveSkill(string ID)
    {
        if (_skills.ContainsKey(ID))
        {
            _skills[ID].Disable();
            _skills.Remove(ID);
            return true;
        }

        return false;
    }
}
