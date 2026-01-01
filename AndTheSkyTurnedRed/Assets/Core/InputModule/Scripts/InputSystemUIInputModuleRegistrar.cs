using UnityEngine;
using UnityEngine.InputSystem.UI;
using Zenject;

namespace Core.InputModule.Scripts {
    public class InputSystemUIInputModuleRegistrar : MonoBehaviour {
        [SerializeField] private InputSystemUIInputModule _inputSystemUIInputModule;
        private InputModel _inputModel;

        [Inject]
        private void InjectDependencies(InputModel inputModel) {
            _inputModel = inputModel;
        }

        private void Awake() =>
            _inputModel.InputSystemUIInputModule = _inputSystemUIInputModule;
    }
}