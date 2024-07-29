using NaughtyAttributes;
using UnityEngine;

public partial class Customizer
{
    [OnValueChanged("OnLoginProviderValueChanged")]
    [EnumFlags]
    public LoginProviderEnums LoginProvider;
    
    private void OnLoginProviderValueChanged()
    {
        Debug.Log($"The {nameof(LoginProvider)} from {nameof(GameInstaller)}.{nameof(Customizer)} value is : {LoginProvider}");
    }
}