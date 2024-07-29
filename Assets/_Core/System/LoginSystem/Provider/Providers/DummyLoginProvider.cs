using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DummyLoginProvider : ILoginProvider
{
    public ILoginProvider CreateSelf()
    {
        return new DummyLoginProvider();
    }
    
    public void Initialize(LoginManager loginManager, Action onReady)
    {
        Debug.Log($"{nameof(DummyLoginProvider)}.{nameof(Initialize)} was called");
        onReady?.Invoke();
    }
    
    public bool IsRegistered()
    {
        Debug.Log($"{nameof(DummyLoginProvider)}.{nameof(IsRegistered)} was called");
        return true;
    }

    public bool HaveBeenLoggedInBefore()
    {
        return false;
    }

    public async UniTask Login(Action onReady)
    {
        Debug.Log($"{nameof(DummyLoginProvider)}.{nameof(Login)} was called");
        onReady?.Invoke();
    }
    
    public async UniTask Register(Action onReady)
    {
        Debug.Log($"{nameof(DummyLoginProvider)}.{nameof(Register)} was called");
        onReady?.Invoke();
    }

    public async UniTask LogOut()
    {
        Debug.Log($"{nameof(DummyLoginProvider)}.{nameof(LogOut)} was called");
    }

    public async UniTask UpdateUsername(string username)
    {
        Debug.Log($"{nameof(DummyLoginProvider)}.{nameof(UpdateUsername)} was called");
        await Task.Yield();
    }

    public async UniTask UpdateAvatar(int avatarIndex)
    {
        Debug.Log($"{nameof(DummyLoginProvider)}.{nameof(UpdateAvatar)} was called");
        await Task.Yield();
    }
}