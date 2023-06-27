using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System;

public class Bullet : NetworkBehaviour {

    [SerializeField] private LayerMask layerMask;
    //[SerializeField] private float bulletExplosionRadius = 1f; // The maximum distance away from the explosion ships can be and are still affected.
    //[SerializeField] private float bulletExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
    [SerializeField] private float bulletMaxLifeTime = 3f;

    public CharacterDataSO characterData;

    private int damage = 0;

    public override void OnNetworkSpawn() {
        if (!IsServer) return;
        StartCoroutine(DespawnDelay()); // the bullet will get destroyed after 2 seconds if it didnt collide with anything

    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (other.TryGetComponent(out SpaceShipController spaceShipController)) {
            //spaceShipController.Hit(damage);
            NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
        }

        //Collider[] colliders = Physics.OverlapSphere(transform.position, bulletExplosionRadius, layerMask);

        //foreach (Collider collider in colliders) {
        //    Rigidbody targetRigidbody = collider.GetComponent<Rigidbody>();
        //    if (!targetRigidbody)
        //        continue;
        //    targetRigidbody.AddExplosionForce(bulletExplosionForce, transform.position, bulletExplosionRadius);

        //    Debug.Log("We hit a player");

        //    SpaceShipController spaceshipHealth = targetRigidbody.GetComponent<SpaceShipController>();
        //    if (spaceshipHealth != null) {
        //        spaceshipHealth.Hit(damage);
        //    }

        //    NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);

        //}
    }

    //private void Update() {
    //    if (!IsServer) return;

    //    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, layerMask);
    //    for (int i = 0; i > hitColliders.Length; i++) {
    //        Debug.Log("Hit something");
    //        if (hitColliders[i].TryGetComponent(out SpaceShipController spaceShipController)) {
    //            spaceShipController.Hit(damage);
    //            NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
    //        }
    //    }

    //    //int maxColliders = 10; // we wont have more than 10 players
    //    //Collider[] hitColliders = new Collider[maxColliders];
    //    //int colliders = Physics.OverlapSphereNonAlloc(transform.position, 2f, hitColliders, layerMask);
    //    //for (int i = 0; i > colliders; i++) {
    //    //    Debug.Log("Hit something");
    //    //    if (hitColliders[i].TryGetComponent(out SpaceShipController spaceShipController)) {
    //    //        spaceShipController.Hit(damage);
    //    //        NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);
    //    //    }
    //    //}

    //}
    //private void OnDrawGizmos() {
    //    Gizmos.DrawSphere(transform.position, 2f);
    //}

    private IEnumerator DespawnDelay() {
        yield return new WaitForSeconds(bulletMaxLifeTime);
        NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);

    }

    public void SetDamage(int damage) {
        this.damage = damage;
    }
}


/*
 using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
    public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
    public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
    public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
    public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
    public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.


    private void Start ()
    {
        // If it isn't destroyed by then, destroy the shell after it's lifetime.
        Destroy (gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter (Collider other)
    {
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; i++)
        {
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Add an explosion force.
            targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

            // Find the TankHealth script associated with the rigidbody.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            if (!targetHealth)
                continue;

            // Calculate the amount of damage the target should take based on it's distance from the shell.
            float damage = CalculateDamage (targetRigidbody.position);

            // Deal this damage to the tank.
            targetHealth.TakeDamage (damage);
        }

        // Unparent the particles from the shell.
        m_ExplosionParticles.transform.parent = null;

        // Play the particle system.
        m_ExplosionParticles.Play();

        // Play the explosion sound effect.
        m_ExplosionAudio.Play();

        // Once the particles have finished, destroy the gameobject they are on.
        Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);

        // Destroy the shell.
        Destroy (gameObject);
    }


    private float CalculateDamage (Vector3 targetPosition)
    {
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max (0f, damage);

        return damage;
    }
}
 */