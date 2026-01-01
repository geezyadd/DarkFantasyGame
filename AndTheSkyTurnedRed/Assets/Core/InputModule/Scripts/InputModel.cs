using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Core.InputModule.Scripts {
    public class InputModel {
        public Color CurrentGamePadLightbarColor { get; private set; }
        public InputDevice CurrentActiveDevice { get; internal set; }
        public int CurrentActiveDeviceId { get; internal set; } = -1;
        public InputSystemUIInputModule InputSystemUIInputModule { get; internal set; }
        public event Action<Color> OnCurrentGamePadLightbarColorChanged ;

        internal void SetCurrentGamePadLightbarColor(Color color) {
            CurrentGamePadLightbarColor = color;
            OnCurrentGamePadLightbarColorChanged?.Invoke(color);
        }
    }
}