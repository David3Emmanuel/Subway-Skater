using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Animator animator;

void Awake(){
    animator = GetComponent<Animator>();
}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectCoin();
            animator.SetTrigger("Collected");
        }
    }

    void OnEnable() {
        animator.SetTrigger("Spawn");
    }
}
