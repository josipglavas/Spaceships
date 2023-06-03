using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
[RequireComponent(typeof(Spaceship))]
public class EngineParticles : NetworkBehaviour {
    [SerializeField] private ParticleSystem[] engineFireParticles;

    [SerializeField] private ParticleSystem[] engineSmokeParticles;

    [Header("=== Fire particle ===")]
    [SerializeField] private float startSpeedFire;
    [SerializeField] private float startRateOverTimeFire;

    [SerializeField] private float turboSpeedFire;
    [SerializeField] private float turboRateOverTimeFire;

    [Header("=== Smoke particle ===")]
    [SerializeField] private float startSpeedSmoke;
    [SerializeField] private float startRateOverTimeSmoke;

    [SerializeField] private float turboSpeedSmoke;
    [SerializeField] private float turboRateOverTimeSmoke;

    private Spaceship spaceship;

    private void Awake() {
        spaceship = GetComponent<Spaceship>();
        Debug.Log(spaceship.name);
    }

    //public override void OnNetworkSpawn() {
    //    SetFireServerRpc(0, 0);
    //    SetSmokeClientRpc(0, 0);
    //}

    private void Start() {
        SetFireClientRpc(0, 0);
        SetSmokeClientRpc(0, 0);
    }

    private void Update() {
        HandleParticlesServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void HandleParticlesServerRpc() {
        if (!IsOwner) return;
        if (!spaceship.GetIsBoosting() && Mathf.Abs(spaceship.GetThrust1D()) < 0.1f) {
            // we are neither moving or boosting so particles will go to 0;

            SetFireClientRpc(0, 0);
            SetSmokeClientRpc(0, 0);


        } else if (!spaceship.GetIsBoosting() && Mathf.Abs(spaceship.GetThrust1D()) > 0.1f) {
            // we are moving BUT NOT boosting

            SetFireClientRpc(startSpeedFire, startRateOverTimeFire);
            SetSmokeClientRpc(startSpeedSmoke, startRateOverTimeSmoke);

        } else if (spaceship.GetIsBoosting() && spaceship.GetThrust1D() > 0.1f) { // we are boosting and going forward
            // we are moving and boosting
            SetFireClientRpc(turboSpeedFire, turboRateOverTimeFire);
            SetSmokeClientRpc(turboSpeedSmoke, turboRateOverTimeSmoke);

        } else {
            SetFireClientRpc(0, 0);
            SetSmokeClientRpc(0, 0);
        }
    }


    [ClientRpc]
    private void SetFireClientRpc(float speed, float rateOverTime) {
        foreach (var engineFireParticle in engineFireParticles) {
            ParticleSystem.MainModule fireMain = engineFireParticle.main;
            ParticleSystem.EmissionModule fireEmission = engineFireParticle.emission;

            fireMain.startSpeed = speed;
            fireEmission.rateOverTime = rateOverTime;
        }
    }
    [ClientRpc]

    private void SetSmokeClientRpc(float speed, float rateOverTime) {
        foreach (var engineSmokeParticle in engineSmokeParticles) {
            ParticleSystem.MainModule smokeMain = engineSmokeParticle.main;
            ParticleSystem.EmissionModule smokeEmission = engineSmokeParticle.emission;

            smokeMain.startSpeed = speed;
            smokeEmission.rateOverTime = rateOverTime;
        }
    }
}
