using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System.Xml;

public class Weapon : NetworkBehaviour {

    [Header("=== Bullet Settings ===")]
    [SerializeField] Transform[] bulletSpawnPoints;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletVelocity = 100f;
    [SerializeField] private float weaponShootCooldown = 1f;
    [SerializeField] private int fireDamage;

    private bool canShoot = true;

    // Input Values
    private bool isShooting = false;

    private void Update() {

        if (!IsOwner) return;
        FireBulletServerRpc();
    }


    [ServerRpc]
    private void FireBulletServerRpc() {

        if (isShooting && canShoot) {
            StartCoroutine(ShootDelay());
        }


    }
    private IEnumerator ShootDelay() {
        canShoot = false;
        Debug.Log("Shoot");
        foreach (Transform spawnPoint in bulletSpawnPoints) {
            GameObject bullet = NetworkObjectSpawner.SpawnNewNetworkObject(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
            PrepareNewlySpawnedBulltet(bullet);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = bullet.transform.forward * bulletVelocity;

        }

        yield return new WaitForSeconds(weaponShootCooldown);
        canShoot = true;
    }


    private void PrepareNewlySpawnedBulltet(GameObject bullet) {
        Bullet bulletController = bullet.GetComponent<Bullet>();
        bulletController.damage = fireDamage;
        //bulletController.characterData = m_characterData;
    }


    #region Input Methods

    public void OnShoot(InputAction.CallbackContext context) {
        isShooting = context.performed;
    }

    #endregion
}
