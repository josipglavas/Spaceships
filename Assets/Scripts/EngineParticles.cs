using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Spaceship))]
public class EngineParticles : MonoBehaviour {
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

    }

    private void Start() {

        SetFire(0, 0);
        SetSmoke(0, 0);
    }

    private void Update() {
        if (!spaceship.GetIsBoosting() && Mathf.Abs(spaceship.GetThrust1D()) < 0.1f) {
            // we are neither moving or boosting so particles will go to 0;
            SetFire(0, 0);
            SetSmoke(0, 0);
        } else if (!spaceship.GetIsBoosting() && Mathf.Abs(spaceship.GetThrust1D()) > 0.1f) {
            // we are moving BUT NOT boosting

            SetFire(startSpeedFire, startRateOverTimeFire);
            SetSmoke(startSpeedSmoke, startRateOverTimeSmoke);

        } else if (spaceship.GetIsBoosting() && Mathf.Abs(spaceship.GetThrust1D()) > 0.1f) {
            // we are moving and boosting
            SetFire(turboSpeedFire, turboRateOverTimeFire);
            SetSmoke(turboSpeedSmoke, turboRateOverTimeSmoke);

        }
    }

    private void SetFire(float speed, float rateOverTime) {
        foreach (var engineFireParticle in engineFireParticles) {
            ParticleSystem.MainModule fireMain = engineFireParticle.main;
            ParticleSystem.EmissionModule fireEmission = engineFireParticle.emission;

            fireMain.startSpeed = speed;
            fireEmission.rateOverTime = rateOverTime;
        }
    }

    private void SetSmoke(float speed, float rateOverTime) {
        foreach (var engineSmokeParticle in engineSmokeParticles) {
            ParticleSystem.MainModule smokeMain = engineSmokeParticle.main;
            ParticleSystem.EmissionModule smokeEmission = engineSmokeParticle.emission;

            smokeMain.startSpeed = speed;
            smokeEmission.rateOverTime = rateOverTime;
        }
    }
}
