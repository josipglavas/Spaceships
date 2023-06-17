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
    [SerializeField] private int fireDamage = 10;

    private bool canShoot = true;

    private Vector3 screenCenter;
    public Camera mainCam;
    private Ray ray;
    // Input Values
    private bool isShooting = false;

    private void Start() {
        screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        mainCam = gameObject.GetComponentInChildren<Camera>();
    }

    private void Update() {
        ray = mainCam.ScreenPointToRay(screenCenter);

        if (!IsOwner) return;
        if (isShooting && canShoot) {
            StartCoroutine(ShootDelay(ray.direction));
        }
    }


    [ServerRpc]
    private void FireBulletServerRpc(Vector3 rayDirection) {
        Shoot(rayDirection);
    }

    private IEnumerator ShootDelay(Vector3 rayDirection) {
        canShoot = false;
        FireBulletServerRpc(rayDirection);

        yield return new WaitForSeconds(weaponShootCooldown);
        canShoot = true;
    }

    private void Shoot(Vector3 rayDirection) {
        foreach (Transform spawnPoint in bulletSpawnPoints) {
            Vector3 aimDir = (screenCenter - spawnPoint.forward).normalized;

            GameObject bullet = NetworkObjectSpawner.SpawnNewNetworkObject(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
            PrepareNewlySpawnedBulltet(bullet);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = rayDirection * bulletVelocity;
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
