using Features.InjectedPrefabFactory.Scripts;
using Features.SceneLoaderModule.Runtime.Scripts;
using UnityEngine;

namespace Features.CameraModule.Scripts
{
    public class CameraSpawnService : ICameraSpawnService
    {
        private readonly IInjectedPrefabFactory _injectedPrefabFactory;
        private readonly CameraConfiguration _cameraConfiguration;
        private readonly CameraModel _cameraModel;

        public CameraSpawnService(IInjectedPrefabFactory injectedPrefabFactory, CameraConfiguration cameraConfiguration, CameraModel cameraModel)
        {
            _injectedPrefabFactory = injectedPrefabFactory;
            _cameraConfiguration = cameraConfiguration;
            _cameraModel = cameraModel;
        }
        public CameraControllerBase SpawnCamera(CameraType cameraType, Vector3 position = default)
        {
            for (int i = 0; i < _cameraConfiguration.Cameras.Count; i++)
            {
                if(_cameraConfiguration.Cameras[i].CameraType != cameraType)
                    continue;

                if (_cameraModel.CurrentCamera == null) 
                    _cameraModel.CurrentCamera = _injectedPrefabFactory.CreatePrefab(_cameraConfiguration.Camera.gameObject, position, Quaternion.identity).GetComponent<Camera>();
                
                CameraControllerBase cameraController = _injectedPrefabFactory.CreatePrefab(_cameraConfiguration.Cameras[i].CameraPrefab, position, Quaternion.identity).GetComponent<CameraControllerBase>();
                _cameraModel.CurrentController = cameraController;
                return cameraController;
            }

            return null;
        }
    }
}
