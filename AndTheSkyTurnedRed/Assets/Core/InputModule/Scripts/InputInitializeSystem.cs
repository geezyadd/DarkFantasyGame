using Features.Input.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using Zenject;

namespace Core.InputModule.Scripts {
    public class InputInitializeSystem : IInitializable {
        public void Initialize() {
            if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ) && !Application.isEditor)
                RegisterMobileDevice();
        }

        private static void RegisterMobileDevice() {
            InputSystem.RegisterLayout<MobileMock>(matches: new InputDeviceMatcher().WithInterface(nameof(MobileMock)));

            InputSystem.AddDevice<MobileMock>();

            MobileMock mockDevice = InputSystem.GetDevice<MobileMock>();
            Debug.Assert(mockDevice == null, "No MobileMock device found.");
        }
    }
}