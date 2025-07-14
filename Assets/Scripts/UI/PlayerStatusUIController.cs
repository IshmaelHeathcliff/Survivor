using System.Collections.Generic;
using GamePlay.Status;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GamePlay.Character.Player;


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
