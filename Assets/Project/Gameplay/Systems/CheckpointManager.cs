using UnityEngine;
using Unity.Cinemachine;
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public Vector3 lastCheckpointPosition;
    public bool hasCheckpoint;
    public string lastCameraName;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

public void SetCheckpoint(Vector3 pos)
{
    lastCheckpointPosition = pos;
    hasCheckpoint = true;

    if (CameraManager.ActiveCamera != null)
    {
        lastCameraName = CameraManager.ActiveCamera.name;
        Debug.Log("Checkpoint salvo. Câmera: " + lastCameraName);
    }
    else
    {
        Debug.LogWarning("ActiveCamera é NULL!");
    }
}
}