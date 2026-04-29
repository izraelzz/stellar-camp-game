using UnityEngine;
using Unity.Cinemachine;

public class CameraShake2D : MonoBehaviour
{
    public static CameraShake2D Instance;
    public static bool IsShaking { get; private set; }

    private float timer;

    private CinemachineBasicMultiChannelPerlin currentNoise;

    void Awake()
    {
        Instance = this;
        IsShaking = false;

        ResetAllCameras(); 
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                StopShake();
            }
        }
    }

    public void Shake(float duration, float amplitude, float frequency)
    {
        var brain = Camera.main.GetComponent<CinemachineBrain>();
        var activeCam = brain.ActiveVirtualCamera as CinemachineCamera;

        if (activeCam == null) return;

        currentNoise = activeCam.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (currentNoise == null)
        {
            Debug.LogWarning("CAMERA SEM NOISE");
            return;
        }

        timer = duration;
        IsShaking = true;

        currentNoise.AmplitudeGain = amplitude;
        currentNoise.FrequencyGain = frequency;
    }

    void StopShake()
    {
        if (currentNoise != null)
        {
            currentNoise.AmplitudeGain = 0f;
            currentNoise.FrequencyGain = 0f;
        }

        IsShaking = false;
    }

    void ResetAllCameras()
    {
        var allCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);

        foreach (var cam in allCams)
        {
            var noise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();

            if (noise != null)
            {
                noise.AmplitudeGain = 0f;
                noise.FrequencyGain = 0f;
            }
        }
    }
}