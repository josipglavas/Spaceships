using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class PlayerCamera : NetworkBehaviour {
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private AudioListener listener;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            listener.enabled = true;
            virtualCamera.Priority = 12;

        } else {
            listener.enabled = false;
            virtualCamera.Priority = 0;
        }

    }
}
