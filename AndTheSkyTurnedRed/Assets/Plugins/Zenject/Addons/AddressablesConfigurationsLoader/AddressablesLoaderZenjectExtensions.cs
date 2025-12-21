using Cysharp.Threading.Tasks;
using Features.AssetLoaderModule.Scripts;
using UnityEngine;
using Zenject;

namespace Plugins.Zenject.Addons.AddressablesConfigurationsLoader {
    public static class AddressablesLoaderZenjectExtensions {
        public static ScopeConcreteIdArgConditionCopyNonLazyBinder BindConfigurationFromAddressables<T>(this DiContainer container, string addressableKey, string groupName = "Default") where T : ScriptableObject {
            T configuration = container.Resolve<IAssetLoaderService>().LoadAsset<T>(addressableKey, groupName);
            
            return container.Bind<T>().FromScriptableObject(configuration);
        }
        
        public static ScopeConcreteIdArgConditionCopyNonLazyBinder BindConfigurationFromAddressables<T>(this DiContainer container, string addressableKey, IAssetLoaderService assetLoaderFacadeService, string groupName = "Default") where T : ScriptableObject {
            T configuration = assetLoaderFacadeService.LoadAsset<T>(addressableKey, groupName);
    
            return container.Bind<T>().FromScriptableObject(configuration);
        }
            
        public static async UniTask<ScopeConcreteIdArgConditionCopyNonLazyBinder> BindConfigurationFromAddressablesAsync<T>(this DiContainer container, string addressableKey, string groupName = "Default") where T : ScriptableObject {
            T configuration = await container.Resolve<IAssetLoaderService>().LoadAssetAsync<T>(addressableKey, groupName);
    
            return container.Bind<T>().FromScriptableObject(configuration);
        }
            
        public static async UniTask<ScopeConcreteIdArgConditionCopyNonLazyBinder> BindConfigurationFromAddressablesAsync<T>(this DiContainer container, string addressableKey, IAssetLoaderService assetLoaderFacadeService, string groupName = "Default") where T : ScriptableObject {
            T configuration = await assetLoaderFacadeService.LoadAssetAsync<T>(addressableKey, groupName);
    
            return container.Bind<T>().FromScriptableObject(configuration);
        }
            
        public static ScopeConcreteIdArgConditionCopyNonLazyBinder BindConfigurationFromAddressablesToInterface<TConfiguration, TInterface>(this DiContainer container, string addressableKey, string groupName = "Default") where TConfiguration : ScriptableObject, TInterface {
            TConfiguration configuration = container.Resolve<IAssetLoaderService>().LoadAsset<TConfiguration>(addressableKey, groupName);
    
            return container.Bind<TInterface>().To<TConfiguration>().FromScriptableObject(configuration);
        }
    }
}