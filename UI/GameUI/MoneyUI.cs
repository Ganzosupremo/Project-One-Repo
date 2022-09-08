using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI bitcoinText;
    public TextMeshProUGUI fiatText;

    private void OnEnable()
    {
        StaticEventHandler.OnMoneyChanged += StaticEventHandler_OnMoneyChangedEvent;
        PersistenceDataManager.Instance.LoadGame();
    }

    private void OnDisable()
    {
        StaticEventHandler.OnMoneyChanged -= StaticEventHandler_OnMoneyChangedEvent;
    }

    /// <summary>
    /// Updates the UI when the player pickups money
    /// </summary>
    private void StaticEventHandler_OnMoneyChangedEvent(MoneyChangedEventArgs moneyChangedEventArgs)
    {
        if (moneyChangedEventArgs.isBitcoin)
        {
            bitcoinText.text = moneyChangedEventArgs.value.ToString("#0.00");
        }
        else
        {
            fiatText.text = moneyChangedEventArgs.value.ToString();
        }
    }
}
