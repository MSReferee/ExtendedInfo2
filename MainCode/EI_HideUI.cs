using UnityEngine;

namespace IINS.ExtendedInfo
{
    class HideUI : MonoBehaviour
    {

        private bool uiHidden = false;

        void OnDestroy()
        {
            if (uiHidden)
            {
                Show();
            }
        }

        void Hide()
        {
            var cameraController = GameObject.FindObjectOfType<CameraController>();
            var camera = cameraController.gameObject.GetComponent<Camera>();
            camera.gameObject.AddComponent<CameraHook>();
            camera.GetComponent<OverlayEffect>().enabled = false;

            var cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (var cam in cameras)
            {
                if (cam.name == "UIView")
                {
                    cam.enabled = false;
                    break;
                }
            }
        }

        void Show()
        {
            var cameraController = GameObject.FindObjectOfType<CameraController>();
            var camera = cameraController.gameObject.GetComponent<Camera>();
            Destroy(cameraController.GetComponent<CameraHook>());
            camera.GetComponent<OverlayEffect>().enabled = true;

            var cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (var cam in cameras)
            {
                if (cam.name == "UIView")
                {
                    cam.enabled = true;
                    break;
                }
            }
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.F11))
                {
                    if (uiHidden)
                    {
                        Show();
                        uiHidden = false;
                    }
                    else
                    {
                        Hide();
                        uiHidden = true;
                    }
                }
            }

            if (Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.F11))
                {
                    if (uiHidden)
                    {
                        Show();
                        uiHidden = false;
                    }
                    else
                    {
                        Hide();
                        uiHidden = true;
                    }
                }
            }
             
        }

    }
}
