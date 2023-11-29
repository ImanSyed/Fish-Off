using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAnimationTrigger : MonoBehaviour
{
    Customer customer;

    // Start is called before the first frame update
    void Start()
    {
        customer = FindAnyObjectByType<Customer>();
    }

    public void ChangeFormNow()
    {
        customer.ChangeForm();
    }
}
