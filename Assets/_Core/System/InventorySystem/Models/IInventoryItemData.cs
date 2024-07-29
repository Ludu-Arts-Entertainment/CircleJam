#if !InventoryManager_Modified
public interface IInventoryItemData
{
    string ItemId { get; set; }
    float Amount { get; set; }
    string GetString();
    string GetId();
    void SetId(string id);
    InventoryItem GetMetadata();
}
#endif
