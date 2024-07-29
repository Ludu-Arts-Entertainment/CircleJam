using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public interface IDataProvider
{
    void Initialize(DataManager dataManager);
    IDataProvider CreateSelf();
    UniTask<IData> Load(IData data);
    UniTask<IData> LoadAll(IData data);
    void Save(IData data);
    void SaveAll(IData data);
}
