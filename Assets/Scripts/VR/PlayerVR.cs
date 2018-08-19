using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class PlayerVR : NetworkBehaviour
{

    public Transform head;
    public Transform leftController;
    public Transform rightController;

    Player player = null;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if (string.Compare(SceneManager.GetActiveScene().name, "SetupTest") == 0) return;

        if (isLocalPlayer)
        {
            player = Player.instance;

            if (player == null)
            {
                Debug.LogError("No VR Player instance found");
                Destroy(this.gameObject);
                return;
            }

            head.GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach(x => x.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly);

            head.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = false);
            leftController.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = false);
            rightController.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = false);
            gameObject.name = "VRPlayer (Local)";

        }
        else
        {
            head.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = true);
            leftController.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = true);
            rightController.GetComponentsInChildren<Renderer>(true).ToList().ForEach(x => x.enabled = true);
            gameObject.name = "VRPlayer (Remote)";
        }

    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        this.transform.position = Vector3.Lerp(this.transform.position, player.transform.position, 0.05f);
    }
}

