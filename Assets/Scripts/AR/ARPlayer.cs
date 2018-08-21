using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ARPlayer : NetworkBehaviour {

    public Transform tablet;

    // Use this for initialization
    void Start () {
        if (string.Compare(SceneManager.GetActiveScene().name, "SetupTest") == 0) return;
        if (tablet == null) return;

        if (isLocalPlayer)
        {
            if (tablet == null)
            {
                Debug.LogError("No AR Player instance found");
                Destroy(this.gameObject);
                return;
            }

            tablet.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = false);
            gameObject.name = "ARPlayer (Local)";

        }
        else
        {
            tablet.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = true);
            gameObject.name = "ARPlayer (Remote)";
        }
	}
	
}
