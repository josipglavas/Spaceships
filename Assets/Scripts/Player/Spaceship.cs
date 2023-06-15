using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;
[RequireComponent(typeof(Rigidbody))]
public class Spaceship : NetworkBehaviour {

    public event EventHandler<float> OnBoostChanged;

    [Header("=== Refrences ===")]
    [SerializeField] private ParticleSystem speedLineParticles;


    [Header("=== Ship Movement Settings ===")]
    [SerializeField]
    private float yawTorque = 500f;
    [SerializeField]
    private float pitchTorque = 1000f;
    [SerializeField]
    private float rollTorque = 1000f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f;

    [SerializeField]
    private float strafeThrust = 50f;

    [Header("=== Ship Boost Settings ===")]
    [SerializeField]
    private float maxBoostAmount;
    [SerializeField]
    private float boostDeprecationRate = 0.25f;
    [SerializeField]
    private float boostRechargeRate = 0.5f;
    [SerializeField]
    private float boostMultiplier = 5f;
    [SerializeField] private bool boosting = false;
    [SerializeField] private float currentBoostAmount = 2f;


    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;

    [HideInInspector]
    public GameManager gameplayManager;

    private float glide = 0.8f;
    private float horizontalGlide = 0.8f;
    private float verticalGlide = 0.8f;

    private Rigidbody rb;

    // Input Values
    private float thrust1D;
    private float upDown1D;
    private float strafe1D;
    private float roll1D;
    private Vector2 pitchYaw;

    private void Awake() {
        // remove the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        //currentBoostAmount = maxBoostAmount;
    }

    private void FixedUpdate() {
        if (!IsOwner) return;
        HandleBoosting();
        HandleMovement();
    }

    private void HandleBoosting() {
        if (boosting && currentBoostAmount > 0f && thrust1D > 0.1f) {
            currentBoostAmount -= boostDeprecationRate;
            OnBoostChanged?.Invoke(this, currentBoostAmount);
            if (currentBoostAmount <= 0f) {
                boosting = false;
            }
        } else {
            if (currentBoostAmount < maxBoostAmount) {
                currentBoostAmount += boostRechargeRate;
                OnBoostChanged?.Invoke(this, currentBoostAmount);
            }
        }
    }

    private void HandleMovement() {
        //Roll
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);
        //Pitch
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        //Yaw
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);

        //Thrust

        if (thrust1D == 0f) {
            speedLineParticles.Stop();
            CinemachineCameraShake.Instance.ShakeCamera(0f);
        }

        if (Mathf.Abs(thrust1D) > 0.1f) {
            float currentThrust;

            if (boosting && thrust1D > 0.1f) { //  we are not using mathf.abs on trust because we dont want to go back boosting
                currentThrust = thrust * boostMultiplier;

                speedLineParticles.Play();

                CinemachineCameraShake.Instance.ShakeCamera(2f);

            } else {
                currentThrust = thrust;
                speedLineParticles.Stop();
                CinemachineCameraShake.Instance.ShakeCamera(0f);
            }
            rb.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.deltaTime);
            glide = thrust;
        } else {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction; // if we want to slowly move forward after stopping the keypress
        }

        //UP and DOWN 
        if (Mathf.Abs(upDown1D) > 0.1f) {
            rb.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.fixedDeltaTime);
            verticalGlide = upDown1D * upThrust;
        } else {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.fixedDeltaTime);
            verticalGlide *= upDownGlideReduction;
        }

        //Strafing

        //TODO: add 15 degrees of roll to already existing roll if we are rolling and if we are not we just return to the previous rotation we had.

        //float strafeRotationAngle = 15f;
        if (Mathf.Abs(strafe1D) > 0.1f) {
            rb.AddRelativeForce(Vector3.right * strafe1D * strafeThrust * Time.fixedDeltaTime);
            horizontalGlide = strafe1D * strafeThrust;
            //Quaternion desiredRotation = Quaternion.Euler(Vector3.back * strafe1D * strafeRotationAngle * Time.deltaTime);
            //if (transform.rotation > strafeRotationAngle) {
            //    desiredRotation.z = strafeRotationAngle;
            //}
            //Debug.Log(desiredRotation);
            //transform.rotation *= desiredRotation;
            //if (transform.rotation.z > desiredRotation.z) {
            //    desiredRotation = Quaternion.identity;
            //}
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.back * strafe1D * strafeRotationAngle) * transform.rotation, Time.deltaTime); // rotate on the z
        } else {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightGlideReduction;
            //if (Mathf.Abs(roll1D) <= 0.1f) { // we are not rolling so we can rotate back to zero  && Mathf.Abs(transform.rotation.z) <= (strafe1D * strafeRotationAngle)
            //    transform.rotation *= Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime); // rotate back to zero
            //}

        }

    }


    #region Input Methods

    public void OnThrust(InputAction.CallbackContext context) {

        thrust1D = context.ReadValue<float>();
    }

    public void OnStrafe(InputAction.CallbackContext context) {

        strafe1D = context.ReadValue<float>();
    }

    public void OnUpDown(InputAction.CallbackContext context) {

        upDown1D = context.ReadValue<float>();
    }

    public void OnRoll(InputAction.CallbackContext context) {

        roll1D = context.ReadValue<float>();
    }

    public void OnPitchYaw(InputAction.CallbackContext context) {

        pitchYaw = context.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext context) {

        boosting = context.performed;
    }
    #endregion


    #region Getters and Setters

    public bool GetIsBoosting() {
        return boosting;
    }

    public float GetThrust1D() {
        return thrust1D;

    }

    public float GetBoost() {
        return maxBoostAmount;
    }

    public void SetBoost(float boost) {
        this.maxBoostAmount = boost;
    }

    #endregion
}
