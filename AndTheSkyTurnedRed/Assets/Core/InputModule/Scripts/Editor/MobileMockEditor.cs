using Features.Input.Scripts;
using UnityEditor;
using UnityEngine.InputSystem;

namespace Core.InputModule.Scripts.Editor {
    [InitializeOnLoad]
    public class MobileMockEditor {
        static MobileMockEditor() {
            InputSystem.RemoveDevice(new MobileMock());
            InputSystem.AddDevice<MobileMock>();
        }
    }
}