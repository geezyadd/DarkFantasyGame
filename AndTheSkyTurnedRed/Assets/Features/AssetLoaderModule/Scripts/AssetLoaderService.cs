using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Features.AssetLoaderModule.Scripts {
    public class AssetLoaderService : IAssetLoaderService {
        private readonly IAddressablesAssetLoaderService  _addressablesAssetLoaderService;

        public AssetLoaderService(IAddressablesAssetLoaderService addressablesAssetLoaderFacadeService) => _addressablesAssetLoaderService = addressablesAssetLoaderFacadeService;

        public async UniTask<TAsset> LoadAssetAsync<TAsset>(string key, string groupName = "Default") where TAsset : Object =>
            await _addressablesAssetLoaderService.LoadAssetAsync<TAsset>(key, groupName);

        public TAsset LoadAsset<TAsset>(string key, string groupName = "Default") where TAsset : Object =>
            _addressablesAssetLoaderService
                .LoadAsset<TAsset>(key, groupName);

        public void ReleaseAssetsInGroup(string groupName = "Default") =>
            _addressablesAssetLoaderService.ReleaseAssetsInGroup(groupName);

        public void ReleaseAllAssets() =>
            _addressablesAssetLoaderService.ReleaseAllAssets();

        public bool HasLoadedAsset(string key, string groupName = "Default") =>
            _addressablesAssetLoaderService
                .HasLoadedAsset(key, groupName);

        public IAddressablesAssetLoaderService GetAssetLoaderService() => _addressablesAssetLoaderService;
    }
}