using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public interface ILoginProvider
{
    /// <summary>
    /// Create a new instance of the dummy login system.
    /// </summary>
    /// <returns></returns>
    ILoginProvider CreateSelf();
    
    /// <summary>
    /// Initialize the dummy login system.
    /// </summary>
    /// <param name="onReady"></param>
    void Initialize(LoginManager loginManager ,Action onReady);
    
    /// <summary>
    /// Check if the user is already registered to game-title.
    /// </summary>
    bool IsRegistered();
    
    /// <summary>
    /// Check if the user is already logged in to game-title.
    /// </summary>
    bool HaveBeenLoggedInBefore();
    
    /// <summary>
    /// If the user is already registered to game-title, this function will login to the game-title.
    /// </summary>
    /// <returns></returns>
    UniTask Login(Action onReady);
    
    /// <summary>
    /// This function register to game-title as a new user. That means it will create a new account and login for the user.
    /// </summary>
    UniTask Register(Action onReady);
    
    UniTask LogOut();
}