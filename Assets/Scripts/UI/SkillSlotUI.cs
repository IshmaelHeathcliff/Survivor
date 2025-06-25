using System.Text;
using GamePlay.Character.Player;
using GamePlay.Skill;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SkillSlotUI : MonoBehaviour, IController
    {

        void Awake()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
