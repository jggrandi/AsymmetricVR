using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HandleARGUI : MonoBehaviour {

    public bool lockTransform = false;

    public void buttonLock () {
        lockTransform = true;
    }

    public void buttonUnlock() {
        lockTransform = false;
    }

}
