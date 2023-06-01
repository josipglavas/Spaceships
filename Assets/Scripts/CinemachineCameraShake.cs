using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CinemachineCameraShake : MonoBehaviour {

    public static CinemachineCameraShake Instance { get; private set; }

    private CinemachineVirtualCamera virtualCamera;

    private float startingIntensity = 0;

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
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(cinemachineBasicMultiChannelPerlin.m_AmplitudeGain, startingIntensity, Time.deltaTime);
    }

    private void Update() {
        Shake();

    }

}
