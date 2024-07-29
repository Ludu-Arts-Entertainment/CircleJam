using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class InventorySystemTest : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            var types = GameInstaller.Instance.SystemLocator.InventoryManager.GetHasInventoryTypes();
            foreach (var pair in types)
            {
                var items = GameInstaller.Instance.SystemLocator.InventoryManager.Get(pair);
                foreach (var item in items)
                {
                    Debug.Log(item.Key);
                    foreach (var data in item.Value)
                    {
                        if (pair == InventoryBlockTypes.Item)
                        {
                            Debug.Log("\t" + data.GetString());
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            GameInstaller.Instance.SystemLocator.InventoryManager.Add(InventoryBlockTypes.Item,InventoryItemTypes.Item1, new BasicInventoryItemData(){ItemId = InventoryItemTypes.Item1});
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            
            var item = GameInstaller.Instance.SystemLocator.InventoryManager.Get(InventoryBlockTypes.Item,InventoryItemTypes.Item1)?[0];
            if (item != null)
            {
                item.Amount += 100;
                GameInstaller.Instance.SystemLocator.InventoryManager.Update(InventoryBlockTypes.Item, InventoryItemTypes.Item1, item);
            }
            
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var item = GameInstaller.Instance.SystemLocator.InventoryManager.Get(InventoryBlockTypes.Item, InventoryItemTypes.Item1)?[0];
            if (item != null)
            {
                GameInstaller.Instance.SystemLocator.InventoryManager.Increase(InventoryBlockTypes.Item, InventoryItemTypes.Item1,
                    item.GetId(), 5);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            var item = GameInstaller.Instance.SystemLocator.InventoryManager.Get(InventoryBlockTypes.Item, InventoryItemTypes.Item1)?[0];
            if (item != null)
            {
                GameInstaller.Instance.SystemLocator.InventoryManager.Decrease(InventoryBlockTypes.Item, InventoryItemTypes.Item1,
                    item.GetId(), 5);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            var item = GameInstaller.Instance.SystemLocator.InventoryManager.Get(InventoryBlockTypes.Item, InventoryItemTypes.Item1)?[0];
            if (item != null)
            {
                GameInstaller.Instance.SystemLocator.InventoryManager.Remove(InventoryBlockTypes.Item, InventoryItemTypes.Item1, item.GetId());
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            GameInstaller.Instance.SystemLocator.ExchangeManager.DoExchange(CurrencyExtension.GetString(Currency.Coin), 9f);
            TrackingService.Feed(TrackType.CurrencyGained, CurrencyExtension.GetString(Currency.Coin), 9f);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            GameInstaller.Instance.SystemLocator.ExchangeManager.DoExchange(CurrencyExtension.GetString(Currency.Coin), .2f);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log(GameInstaller.Instance.SystemLocator.ExchangeManager.GetExchange(CurrencyExtension.GetString(Currency.Coin),0f));
            Debug.Log((typeof(InventoryBlockTypes)).GetAllPublicConstantValues<string>());

        }
    }
}
public class BasicInventoryItemData: IInventoryItemData
{
    public float Amount { get; set; }
    public string uuid;
    public string ItemId { get; set; }
    public string GetString()
    {
        return uuid + " " + ItemId + " " + Amount + " " + GetMetadata().Name;
    }

    public string GetId()
    {
        return uuid;
    }

    public void SetId(string id)
    {
        uuid = id;
    }

    public InventoryItem GetMetadata()
    {
        return InventoryDataService.Get(InventoryBlockTypes.Item, ItemId);
    }
}