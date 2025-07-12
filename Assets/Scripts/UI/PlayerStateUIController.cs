using System.Collections.Generic;
using GamePlay.State;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GamePlay.Character.Player;


namespace UI
{
    public class PlayerStateUIController : StateUIController
    {
        protected override void SetStateContainer()
        {
            StateContainer = this.GetModel<PlayersModel>().Current.StateContainer;
        }
    }
}
