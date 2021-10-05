using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IClickable
{
    public void OnClick()
    {
        Debug.Log("Меня нажали");
    }
}
