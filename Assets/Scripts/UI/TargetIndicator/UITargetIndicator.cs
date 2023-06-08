using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITargetIndicator : MonoBehaviour {

    public static UITargetIndicator Instance { get; private set; }

    [SerializeField] private Canvas canvas;
    [SerializeField] List<TargetIndicator> targetIndicators = new List<TargetIndicator>();
    [SerializeField] GameObject targetIndicatorPrefab;

    public Camera mainCamera;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (targetIndicators != null) {
            foreach (TargetIndicator targetIndicator in targetIndicators) {
                targetIndicator.UpdateTargetIndicator();
            }
        }
    }

    public void AddTargetIndicator(GameObject target) {
        if (mainCamera == null) return;
        TargetIndicator indicator = Instantiate(targetIndicatorPrefab, canvas.transform).GetComponent<TargetIndicator>();
        indicator.InitialiseTargetIndicator(target, mainCamera, canvas);
        targetIndicators.Add(indicator);
    }
}
