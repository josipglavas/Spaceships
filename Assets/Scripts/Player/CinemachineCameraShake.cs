using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CinemachineCameraShake : MonoBehaviour {

    public static CinemachineCameraShake Instance { get; private set; }

    private CinemachineVirtualCamera virtualCamera;

    private float startingIntensity = 0;
    private float shakeStartStopMultiplier = 3f;

    private void Awake() {
        Instance = this;

    }

    private void Start() {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

    }

    public void ShakeCamera(float intensity) {
        startingIntensity = intensity;

    }
    private void Shake() {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(cinemachineBasicMultiChannelPerlin.m_AmplitudeGain, startingIntensity, Time.deltaTime * shakeStartStopMultiplier);
        if (cinemachineBasicMultiChannelPerlin.m_AmplitudeGain <= 0.5f && startingIntensity <= 0.1f) { // make the stopping quicker and camera shake wont go into crazy floating point numbers
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        }
    }

    private void Update() {
        Shake();

    }

}
