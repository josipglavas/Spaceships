using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class TargetObject : NetworkBehaviour {
    [SerializeField] Camera playerCamera;

    private void Start() {
        // if (IsOwner) {
        UITargetIndicator.Instance.mainCamera = playerCamera;

        //} else {
        if (!IsOwner)
            UITargetIndicator.Instance.AddTargetIndicator(gameObject);

        // }

    }
}
