using System;
using System.Collections.Generic;

public interface IData
{
    T GetData<T>(GameDataType key);
    object GetData(GameDataType key);
    void SetData<T>(GameDataType key, T value);
    void SetData(Dictionary<string, string> data, bool markDirty = false);
    Type GetDataType();
    void ClearDirty();
    bool IsDirty(GameDataType key);
    void MarkAllDirty();
    HashSet<GameDataType> GetDirtyDataTypes();
}