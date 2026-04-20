using UnityEngine;
using Unity.Cinemachine;

public class CameraShake2D : MonoBehaviour
{
    public static CameraShake2D Instance;

    // 🔥 FLAG GLOBAL (ESSENCIAL)
    public static bool IsShaking { get; private set; }

    private CinemachineBasicMultiChannelPerlin noise;

    private float timer;

    void Awake()
    {
        Instance = this;

        var vcam = GetComponent<CinemachineCamera>();
        noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();

        // 🔥 garante que começa parado
        if (noise != null)
        {
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 0f;
        }

        IsShaking = false;
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
        if (noise == null) return;

        timer = duration;

        IsShaking = true; // 🔥 ativa flag

        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;
    }

    void StopShake()
    {
        if (noise == null) return;

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;

        IsShaking = false; // 🔥 desativa flag
    }
}