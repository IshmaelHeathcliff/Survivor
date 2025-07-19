using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;

namespace XYZRPGSystem.Core
{
    public class InputSystem : AbstractSystem
    {
        InputActionAsset _input;

        public InputActionMap PlayerActionMap { get; private set; }


        protected override void OnInit()
        {
            _input = Addressables.LoadAssetAsync<InputActionAsset>("PlayerInput").WaitForCompletion();
            PlayerActionMap = _input.FindActionMap("Player");
        }
    }
}
