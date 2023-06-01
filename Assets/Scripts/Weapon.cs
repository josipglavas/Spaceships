using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour {
    [SerializeField] Transform[] bulletSpawnPoints;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletVelocity = 100f;
    [SerializeField] private bool canShoot = true;
    [SerializeField] private float weaponShootCooldown = 1f;

    private bool isShooting = false;

    private void Update() {
        Shoot();
    }

    private void Shoot() {
        if (isShooting && canShoot) {
            StartCoroutine(ShootDelay());
        }
    }

    private IEnumerator ShootDelay() {
        canShoot = false;

        foreach (Transform spawnPoint in bulletSpawnPoints) {
            GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = bullet.transform.forward * bulletVelocity;

        }

        yield return new WaitForSeconds(weaponShootCooldown);
        canShoot = true;
    }


    #region Input Methods

    public void OnShoot(InputAction.CallbackContext context) {
        isShooting = context.performed;
    }

    #endregion
}
