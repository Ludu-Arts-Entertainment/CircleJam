using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorialProvider
{
    ITutorialProvider CreateSelf();
    void Initialize(Action onReady);
}
