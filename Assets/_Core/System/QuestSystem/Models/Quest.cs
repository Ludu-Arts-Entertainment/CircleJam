using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string Id;
    public bool IsActive = true;
    public MetaData metaData;
    public QuestType Type;
    public int Order;
    public QuestData QuestData;
    public List<ProductBlock> RewardProducts;
    [HideInInspector]public float CurrentAmount;
    public QuestState State = QuestState.Waiting;
    public DateTime UpdateTime;
    public Quest Clone()
    {
        return new Quest()
        {
            Id = this.Id,
            IsActive = this.IsActive,
            metaData = this.metaData,
            Type = this.Type,
            Order = this.Order,
            CurrentAmount = this.CurrentAmount,
            State = this.State,
            QuestData = this.QuestData,
            RewardProducts = this.RewardProducts,
            UpdateTime = this.UpdateTime,
        };
    }
}