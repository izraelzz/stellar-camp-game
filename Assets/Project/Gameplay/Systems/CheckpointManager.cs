using UnityEngine;
using Unity.Cinemachine;
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public Vector3 lastCheckpointPosition;
    public bool hasCheckpoint;
    public CinemachineCamera lastCamera;

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

    lastCamera = CameraManager.ActiveCamera;
}
}