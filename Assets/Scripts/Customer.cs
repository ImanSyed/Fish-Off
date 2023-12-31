using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image bodySprite;
    [SerializeField] Sprite[] bodySprites;
    [SerializeField] List<Bounty> tier1Bounties, tier2Bounties, tier3Bounties;
    Bounty currentBounty;
    [SerializeField] GameObject bountyPopup;
    [SerializeField] Image bountyImage;

    public int currentTier = 1;
    [SerializeField] int tierIncreaseThreshold;
    int tierIncreaseCounter;
    bool canClick;
    FPSController playerController;
    ShopUI shopUI;
    [SerializeField] Animator animator;

    private void Start() 
    {
        shopUI = FindObjectOfType<ShopUI>();
        playerController = FindObjectOfType<FPSController>();    
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        bountyPopup.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        bountyPopup.SetActive(false);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(canClick)
        {
            if(playerController.fishCollection.Contains(currentBounty.fishType))
            {
                playerController.fishCollection.Remove(currentBounty.fishType);
                BountyComplete();
            }
            else
            {
                animator.Play("Incorrect", 0);
            }
        }
    }

    void BountyComplete()
    {
        animator.Play("Correct", 0);
        int rewardValue = currentBounty.reward + Random.Range(-currentBounty.rewardRandomiser, currentBounty.rewardRandomiser + 1);
        shopUI.ChangeMoney(rewardValue);

        tierIncreaseCounter += rewardValue;

        if(tierIncreaseCounter >= tierIncreaseThreshold)
        {
            currentTier++;
            tierIncreaseCounter = 0;
        }

        canClick = false;

        switch(currentTier)
        {
            case 1:
                tier1Bounties.Remove(currentBounty);
            break;

            case 2:
                tier2Bounties.Remove(currentBounty);
            break;

            case 3:
                tier3Bounties.Remove(currentBounty);
            break;
        }
    }

    public void GetCustomer()
    {
        animator.Play("Empty", 0);
    }

    public void ChangeForm()
    {
        switch(currentTier)
        {
            case 1:
                currentBounty = tier1Bounties[Random.Range(0, tier1Bounties.Count)];
            break;

            case 2:
                currentBounty = tier2Bounties[Random.Range(0, tier1Bounties.Count)];
            break;

            case 3:
                currentBounty = tier3Bounties[Random.Range(0, tier1Bounties.Count)];
            break;
        }
        bodySprite.sprite = bodySprites[Random.Range(0, bodySprites.Length)];
        bountyImage.sprite = currentBounty.sprite;
        canClick = true;

    }
}
