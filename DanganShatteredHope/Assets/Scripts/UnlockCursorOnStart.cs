using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockCursorOnStart : MonoBehaviour
{
    
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

}
