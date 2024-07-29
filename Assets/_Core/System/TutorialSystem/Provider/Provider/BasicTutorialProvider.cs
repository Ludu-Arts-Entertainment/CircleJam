using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTutorialProvider : ITutorialProvider
{
    public ITutorialProvider CreateSelf()
    {
        return new BasicTutorialProvider();
    }

    public void Initialize(System.Action onReady)
    {
        onReady?.Invoke();
    }
}
