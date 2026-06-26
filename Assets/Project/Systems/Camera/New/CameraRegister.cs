using Unity.Cinemachine;
using UnityEngine;

public class CameraRegister : MonoBehaviour
{
    private CinemachineCamera cam;

    private void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
    }

    private void OnEnable()
    {
        CameraManager.Register(cam);

        if (CheckpointManager.Instance != null &&
            CheckpointManager.Instance.hasCheckpoint)
        {
            if (cam.name == CheckpointManager.Instance.lastCameraName)
            {
                cam.Priority = 20;
                CameraManager.ActiveCamera = cam;
            }
            else
            {
                cam.Priority = 10;
            }
        }
        else
        {
            if (cam.Priority >= 20)
            {
                CameraManager.ActiveCamera = cam;
            }
        }
    }

    private void OnDisable()
    {
        CameraManager.Unregister(cam);
    }
}