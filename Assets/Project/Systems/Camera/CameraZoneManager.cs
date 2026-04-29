using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraZoneManager : MonoBehaviour
{
    [System.Serializable]
    public class CameraZone
    {
        public float minX;
        public float maxX;
        public CinemachineCamera cam;
    }

    public Transform player;
    public List<CameraZone> zones;

    void Update()
    {
        float playerX = player.position.x;

        foreach (var zone in zones)
        {
            if (playerX >= zone.minX && playerX < zone.maxX)
            {
                SetActiveCamera(zone.cam);
                return;
            }
        }
    }

    void SetActiveCamera(CinemachineCamera activeCam)
    {
        foreach (var zone in zones)
        {
            zone.cam.Priority = (zone.cam == activeCam) ? 20 : 0;
        }
    }
}