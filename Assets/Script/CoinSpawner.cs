using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public int maxCoins = 5;
    public float chanceToSpawn = 0.5f;
    public bool forceSpawnAll = false;

    private GameObject[] coins;

    void Awake() {
        coins = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            coins[i] = transform.GetChild(i).gameObject;
        }
        OnDisable();
    }

    void OnEnable() {
        if (Random.Range(0f, 1f) > chanceToSpawn) return;
        int coinsToSpawn = forceSpawnAll ? maxCoins : Random.Range(1, maxCoins + 1);
        for (int i = 0; i < coinsToSpawn; i++) coins[i].SetActive(true);
    }

    void OnDisable() {
        foreach (GameObject coin in coins) coin.SetActive(false);
    }
}
