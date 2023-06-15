using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
public class UIPlayerGameplay : NetworkBehaviour {
    //private CharacterDataSO characterDataSO;

    private GameObject player;

    [SerializeField] private Image boostBar;
    [SerializeField] private TextMeshProUGUI boostText;

    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private Image shipImage;

    private float maxBoostAmount = 1;
    private int maxHealthAmount = 1;

    private float currentBoostAmount = 1;
    private int currentHealthAmount = 1;

    private void Spaceship_OnHealthChanged(object sender, int healthAmount) {
        currentHealthAmount = healthAmount;
    }

    private void Spaceship_OnBoostChanged(object sender, float boostAmount) {
        currentBoostAmount = boostAmount;
    }

    private void Update() {
        UpdateBoost();
        UpdateHealth();

    }

    private void UpdateBoost() {
        boostBar.fillAmount = (int)currentBoostAmount / maxBoostAmount;
        boostText.text = $"{(int)currentBoostAmount} / {(int)maxBoostAmount}";
    }


    private void UpdateHealth() {

        healthBar.fillAmount = (int)currentHealthAmount / maxHealthAmount;
        healthText.text = $"{currentHealthAmount} / {maxHealthAmount}";
    }

    public void SetPlayerUI(GameObject playerSpaceship) {
        player = playerSpaceship;
        //shipImage.sprite = characterDataSO.characterShipSprite;
        float boost = playerSpaceship.GetComponent<Spaceship>().GetBoost();
        boostBar.fillAmount = 1; // its full 20 / 20
        boostText.text = $"{boost} / {boost}";
        maxBoostAmount = boost;
        currentBoostAmount = maxBoostAmount;

        int health = playerSpaceship.GetComponent<SpaceShipController>().GetHealth();
        healthBar.fillAmount = 1;
        healthText.text = $"{health} / {health}";
        maxHealthAmount = health;
        currentHealthAmount = maxHealthAmount;

        playerSpaceship.GetComponent<Spaceship>().OnBoostChanged += Spaceship_OnBoostChanged;
        playerSpaceship.GetComponent<SpaceShipController>().OnHealthChanged += Spaceship_OnHealthChanged;
    }
}
