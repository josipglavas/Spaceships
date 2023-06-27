using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using System.Linq;

public class SpaceShipController : NetworkBehaviour {

    /// <summary>
    /// ONLY FOR PLAYTESTING
    [SerializeField]
    private bool Kill;
    /// </summary>

    public event EventHandler<int> OnHealthChanged;

    [HideInInspector]
    public CharacterDataSO characterData;

    [HideInInspector]
    public GameManager gameplayManager;

    public NetworkVariable<int> health = new NetworkVariable<int>();

    private bool isPlayerDefeated = false;

    private void Update() {
        if (Kill) {
            Hit(10000);
        }
    }

    public void Hit(int damage) {
        //if (!IsServer || isPlayerDefeated)
        if (isPlayerDefeated)
            return;
        Debug.Log("Taken damage amount: " + damage + ".");
        health.Value -= damage;
        UpdateUIClientRpc();
        //HitClientRpc();

        if (health.Value > 0) {
            //player is still alive, take some damage and play effects for taking damage
        } else {
            //player is dead
            isPlayerDefeated = true;

            // Tell the Gameplay manager that I've been defeated
            //gameplayManager.PlayerDeath(m_characterData.clientId);
            EmitParticleServerRpc();
            NetworkObjectDespawner.DespawnNetworkObject(NetworkObject);


        }
    }
    [ClientRpc]
    private void UpdateUIClientRpc() {
        OnHealthChanged?.Invoke(this, health.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void EmitParticleServerRpc() {
        ParticleSystem deathParticle = GameObject.FindGameObjectWithTag("ExplosionParticle").GetComponent<ParticleSystem>();
        EmitParicleClientRpc();
    }

    [ClientRpc]
    private void EmitParicleClientRpc() {
        ParticleSystem deathParticle = GameObject.FindGameObjectWithTag("ExplosionParticle").GetComponent<ParticleSystem>();
        deathParticle.transform.position = transform.position;

        deathParticle.Emit(1);
    }

    public void SetHealth(int health) {
        this.health.Value = health;
    }

    public int GetHealth() {
        return health.Value;
    }
}
