using System.Collections.Generic;
using XYZRPGSystem.Gameplay.Status;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Gameplay.Character.Player;
using XYZRPGSystem.UI;


namespace UI
{
    public class PlayerStatusUIController : StatusUIController
    {
        protected override void SetStatusContainer()
        {
            StatusContainer = this.GetModel<PlayersModel>().Current.StatusContainer;
        }
    }
}
