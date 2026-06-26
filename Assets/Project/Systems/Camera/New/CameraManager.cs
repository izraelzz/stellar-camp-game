using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    static List<CinemachineCamera> cameras = new List<CinemachineCamera>();

    public static CinemachineCamera ActiveCamera = null;

    public static bool IsActiveCamera(CinemachineCamera camera)
    {
        return camera == ActiveCamera;
    }

public static void SwitchCamera(CinemachineCamera newCamera)
{
    if (newCamera == null)
        return;

    ActiveCamera = newCamera;

    foreach (CinemachineCamera cam in cameras)
    {
        cam.Priority = (cam == newCamera) ? 20 : 10;
    }
}

    public static void Register(CinemachineCamera camera)
    {
        cameras.Add(camera);
    }

    public static void Unregister(CinemachineCamera camera)
    {
        cameras.Remove(camera);
    }
}