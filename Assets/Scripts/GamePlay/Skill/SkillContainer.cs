using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface ISkillContainer
{
    bool TryGetSkill(string ID, out ISkill skill);
    ISkill GetSkill(string ID);
    bool AddSkill(ISkill skill);
    bool RemoveSkill(string ID);
    bool ReleaseSkill(string ID, out ISkill skill);
    bool HasSkill(string ID);
    bool HasSkills(IEnumerable<string> IDs);
    int HasCount(IEnumerable<string> IDs);
    int MaxCount { get; set; }
    int Count { get; }
    IEnumerable<ISkill> GetAllSkills();
    void Clear();
}

// ! 一个角色同一ID的技能只能拥有一个
public class SkillContainer : ISkillContainer
{
    readonly Dictionary<string, ISkill> _skills = new();
    public int MaxCount { get; set; }
    public int Count => _skills.Count;

    public SkillContainer(int maxCount = 0)
    {
        MaxCount = maxCount;
    }

    public IEnumerable<ISkill> GetAllSkills()
    {
        return _skills.Values;
    }

    public bool TryGetSkill(string ID, out ISkill skill)
    {
        return _skills.TryGetValue(ID, out skill);
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

    public bool ReleaseSkill(string ID, out ISkill skill)
    {
        if (_skills.TryGetValue(ID, out skill))
        {
            skill.Disable();
            _skills.Remove(ID);
            return true;
        }

        return false;
    }

    public void Clear()
    {
        string[] skillIDs = _skills.Keys.ToArray();
        foreach (string id in skillIDs)
        {
            RemoveSkill(id);
        }
    }
}
