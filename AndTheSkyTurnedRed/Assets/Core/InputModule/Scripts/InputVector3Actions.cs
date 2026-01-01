using System;
using UnityEngine;

namespace Core.InputModule.Scripts {
    public class InputVector3Actions : InputDefaultActions {
        public Action<Vector3> VectorChangedStarted;
        public Action<Vector3> VectorChangedPerformed;
        public Action<Vector3> VectorChangedCanceled;
    }
}