using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Features.AssetLoaderModule.Scripts {
    public class AddressablesGroupHandleContainer {
        public readonly Dictionary<string, AsyncOperationHandle> CompletedHandles = new();
        public readonly Dictionary<string, List<AsyncOperationHandle>> AllHandles = new();
    }
}