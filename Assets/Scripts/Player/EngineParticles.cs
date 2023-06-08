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

    //private void Awake() {
    //    if (!IsOwner) return;
    //    spaceship = GetComponent<Spaceship>();
    //    Debug.Log(spaceship.name);
    //}

    public override void OnNetworkSpawn() {
        spaceship = GetComponent<Spaceship>();

        SetFireServerRpc(0, 0);
        SetSmokeServerRpc(0, 0);
    }

    //private void Start() {
    //    SetFireClientRpc(0, 0);
    //    SetSmokeClientRpc(0, 0);
    //}

    private void Update() {
        HandleParticles();
    }

    private void HandleParticles() {
        if (!IsOwner) return;
        if (!spaceship.GetIsBoosting() && Mathf.Abs(spaceship.GetThrust1D()) < 0.1f) {
            // we are neither moving or boosting so particles will go to 0;

            SetFireServerRpc(0, 0);
            SetSmokeServerRpc(0, 0);

        } else if (!spaceship.GetIsBoosting() && Mathf.Abs(spaceship.GetThrust1D()) > 0.1f) {
            // we are moving BUT NOT boosting

            SetFireServerRpc(startSpeedFire, startRateOverTimeFire);
            SetSmokeServerRpc(startSpeedSmoke, startRateOverTimeSmoke);

        } else if (spaceship.GetIsBoosting() && spaceship.GetThrust1D() > 0.1f) { // we are boosting and going forward
                                                                                  // we are moving and boosting
            SetFireServerRpc(turboSpeedFire, turboRateOverTimeFire);
            SetSmokeServerRpc(turboSpeedSmoke, turboRateOverTimeSmoke);

        } else if (spaceship.GetIsBoosting() && spaceship.GetThrust1D() < 0.1f) {
            // we are boosting and going backwards, the boosting here isnt working and we are not increasing speed but we need to sync the particles
            SetFireServerRpc(startSpeedFire, startRateOverTimeFire);
            SetSmokeServerRpc(startSpeedSmoke, startRateOverTimeSmoke);
        } else {
            SetFireServerRpc(0, 0);
            SetSmokeServerRpc(0, 0);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetFireServerRpc(float speed, float rateOverTime) {
        SetFireClientRpc(speed, rateOverTime);
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

    [ServerRpc(RequireOwnership = false)]
    private void SetSmokeServerRpc(float speed, float rateOverTime) {
        SetSmokeClientRpc(speed, rateOverTime);
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
