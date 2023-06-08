using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System.Xml;
using static UnityEditor.FilePathAttribute;
using UnityEngine.UIElements;

public class Weapon : NetworkBehaviour {

    [Header("=== Bullet Settings ===")]
    [SerializeField] Transform[] bulletSpawnPoints;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletVelocity = 100f;
    [SerializeField] private float weaponShootCooldown = 1f;
    [SerializeField] private int fireDamage = 60;

    private bool canShoot = true;

    // Input Values
    private bool isShooting = false;

    private void Update() {
        if (!IsOwner) return;

        if (isShooting && canShoot) {
            StartCoroutine(ShootDelay());
        }
    }


    [ServerRpc]
    private void FireBulletServerRpc() {
        Shoot();
    }

    private IEnumerator ShootDelay() {
        canShoot = false;

        FireBulletServerRpc();

        yield return new WaitForSeconds(weaponShootCooldown);
        canShoot = true;
    }

    private void Shoot() {
        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        foreach (Transform spawnPoint in bulletSpawnPoints) {
            Vector3 aimDir = (screenCenter - spawnPoint.forward).normalized;

            GameObject bullet = NetworkObjectSpawner.SpawnNewNetworkObject(bulletPrefab, spawnPoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
            PrepareNewlySpawnedBulltet(bullet);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = ray.direction * bulletVelocity;
        }
    }


    private void PrepareNewlySpawnedBulltet(GameObject bullet) {
        Bullet bulletController = bullet.GetComponent<Bullet>();
        bulletController.SetDamage(fireDamage);
        // bulletController.characterData = m_characterData;
    }


    #region Input Methods

    public void OnShoot(InputAction.CallbackContext context) {

        isShooting = context.performed;
    }

    #endregion
}
