using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public int money;
    [SerializeField] TMPro.TMP_Text moneyText;
    [SerializeField] GameObject shopFront;
    [SerializeField] int[] speedUpgradePrices, o2UpgradePrices;
    [SerializeField] Sprite[] speedSprites, o2Sprites;
    [SerializeField] Button speedButton, o2Button;
    int speedUpgradeCounter, o2UpgradeCounter;
    FPSController playerController;
    float speedIncreaseValue;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ShowShop(bool b)
    {
        shopFront.SetActive(b);
    }

    public void ChangeMoney(int changeValue)
    {
        money += changeValue;
        moneyText.text = money.ToString();

        if(speedButton.interactable && speedUpgradePrices[speedUpgradeCounter] > money)
        {
            speedButton.interactable = false;
        }
        else
        {
            speedButton.interactable = true;
        }

        if(o2Button.interactable && o2UpgradePrices[o2UpgradeCounter] > money)
        {
            o2Button.interactable = false;
        }
        else
        {
            o2Button.interactable = true;
        }

    }

    public void UpgradeSpeed()
    {
        ChangeMoney(-speedUpgradePrices[speedUpgradeCounter]);
        speedUpgradeCounter++;
        speedButton.image.sprite = speedSprites[speedUpgradeCounter];

        if(speedUpgradeCounter >= speedUpgradePrices.Length - 1)
        {
            Debug.Log("!");
            speedButton.interactable = false;
            speedButton = null;
        }

        playerController.walkingSpeed += speedIncreaseValue;
        playerController.runningSpeed += speedIncreaseValue;
    }

    public void UpgradeO2()
    {
        ChangeMoney(-o2UpgradePrices[o2UpgradeCounter]);
        o2UpgradeCounter++;
        o2Button.image.sprite = o2Sprites[o2UpgradeCounter];

        if(o2UpgradeCounter >= o2UpgradePrices.Length - 1)
        {
            o2Button.interactable = false;
            o2Button = null;
        }

        playerController.walkingSpeed += speedIncreaseValue;
        playerController.runningSpeed += speedIncreaseValue;
    }
}
