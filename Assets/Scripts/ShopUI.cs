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
    [SerializeField] float speedIncreaseValue, o2IncreaseValue;
    [SerializeField] string maxedMessage;
    [SerializeField] Sprite[] speedSprites, o2Sprites;
    [SerializeField] Button speedButton, o2Button;
    [SerializeField] TMPro.TMP_Text speedPopupText, o2PopupText;

    Animator animator;
    int speedUpgradeCounter, o2UpgradeCounter;
    FPSController playerController;
    bool canStart, started;
    public bool pauseGame;

    void Start()
    {
        pauseGame = true;

        playerController = FindObjectOfType<FPSController>();
        moneyText.text = money.ToString();
                
        o2PopupText.text = o2UpgradePrices[o2UpgradeCounter].ToString();
        speedPopupText.text = speedUpgradePrices[speedUpgradeCounter].ToString();

        animator = GetComponent<Animator>();

    }
    
    public void ShowShop(bool b)
    {
        if(!started)
        {
            return;
        }

        shopFront.SetActive(b);
        pauseGame = b;
        playerController.canShoot = !pauseGame;
        playerController.canMove = !pauseGame;
        Cursor.visible = b;
        
        if(!b)
        {
            playerController.Dive();
            Cursor.lockState = CursorLockMode.Locked;
            playerController.RefreshO2();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void ChangeMoney(int changeValue)
    {
        money += changeValue;
        moneyText.text = money.ToString();

        if(speedButton != null)
        {
            if(speedButton.interactable && speedUpgradePrices[speedUpgradeCounter] > money)
            {
                speedButton.interactable = false;
            }
            else
            {
                speedButton.interactable = true;
            }
        }
        
        if(o2Button != null)
        {
            if(o2Button != null && o2Button.interactable && o2UpgradePrices[o2UpgradeCounter] > money)
            {
                o2Button.interactable = false;
            }
            else
            {
                o2Button.interactable = true;
            }
        }
    }

    public void UpgradeSpeed()
    {
        if(speedUpgradePrices[speedUpgradeCounter] > money)
        {
            return;
        }

        ChangeMoney(-speedUpgradePrices[speedUpgradeCounter]);
        speedUpgradeCounter++;
        speedButton.image.sprite = speedSprites[speedUpgradeCounter];
        speedPopupText.text = speedUpgradePrices[speedUpgradeCounter].ToString();

        if(speedUpgradeCounter >= speedUpgradePrices.Length - 1)
        {
            speedButton.interactable = false;
            speedButton = null;
            speedPopupText.text = maxedMessage;
        }

        playerController.walkingSpeed += speedIncreaseValue;
        playerController.runningSpeed += speedIncreaseValue;
    }

    public void UpgradeO2()
    {
        if(o2UpgradePrices[o2UpgradeCounter] > money)
        {
            return;
        }

        ChangeMoney(-o2UpgradePrices[o2UpgradeCounter]);
        o2UpgradeCounter++;
        o2Button.image.sprite = o2Sprites[o2UpgradeCounter];
        o2PopupText.text = o2UpgradePrices[o2UpgradeCounter].ToString();


        if(o2UpgradeCounter >= o2UpgradePrices.Length - 1)
        {
            o2Button.interactable = false;
            o2Button = null;
            o2PopupText.text = maxedMessage;
        }

        playerController.maxO2 += o2IncreaseValue;
        playerController.runningSpeed += o2IncreaseValue;
    }

    public void MainMenuAnimation()
    {
        canStart = true;
    }

    public void StartGame()
    {
        if(canStart)
        {
            FindObjectOfType<Customer>().GetCustomer();
            animator.Play("Start", 0);
            started = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
