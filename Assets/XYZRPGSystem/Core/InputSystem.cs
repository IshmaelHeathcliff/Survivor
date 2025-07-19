using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace XYZRPGSystem.Core
{
    public class InputSystem : AbstractSystem
    {
        InputActionAsset _input;

        public InputActionMap PlayerActionMap { get; private set; }


        protected override void OnInit()
        {
            // TODO: 异步加载 InputActionAsset
            _input = Addressables.LoadAssetAsync<InputActionAsset>("PlayerInput").WaitForCompletion();
            PlayerActionMap = _input.FindActionMap("Player", true);
        }
    }
}
