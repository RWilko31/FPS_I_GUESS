using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Object : MonoBehaviour, IActivateFunction
{
    [SerializeField] IObjectFunction function;
    public void Activate() { if (function != null) function.Function(); }
}
