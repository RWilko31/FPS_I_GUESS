using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script defines the interfaces for interacting. 

public interface IInteractFunction
{
    void Interact();
    string InteractType();
    string InteractText();
}
public interface IActivateFunction
{
    void Activate();
}

public interface IObjectFunction
{
    void Function();
}