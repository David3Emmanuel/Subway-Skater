using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Animator animator;

void Start(){
    animator = GetComponent<Animator>();
}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectCoin();
            animator.SetTrigger("Collected");
            Destroy(gameObject, 1.5f);
        }
    }
}
