using ICities;
using UnityEngine;

namespace IINS.ExtendedInfo
{

    public class ModLoad : LoadingExtensionBase
    {

        private HideUI hideUI;

        public override void OnLevelLoaded(LoadMode mode)
        {
            var cameraController = GameObject.FindObjectOfType<CameraController>();
            hideUI = cameraController.gameObject.AddComponent<HideUI>();
        }

        public override void OnLevelUnloading()
        {
            GameObject.Destroy(hideUI);
        }
    }

}
