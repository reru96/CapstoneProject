using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public int coins = 0;
    public int GetCoins() => coins;

    public event Action<int> OnCoinsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            LoadCoins();
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }
    }


    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            SaveCoins();
            return true;
        }
        return false;
    }

    public void SetCoins(int amount)
    {
        coins = Mathf.Max(0, amount);
        SaveCoins();
    }

    private void NotifyChange()
    {
        OnCoinsChanged?.Invoke(coins);
    }

    public void SaveCoins()
    {
        SaveData data = SaveSystem.Load();
        data.coins = coins;
        SaveSystem.Save(data);
        NotifyChange();
    }

    public void LoadCoins()
    {
        SaveData data = SaveSystem.Load();
        coins = data.coins;
    }

}
