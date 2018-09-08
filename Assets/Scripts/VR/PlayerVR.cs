using UnityEngine;
using System.Collections;
using System.Linq;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class PlayerVR : MonoBehaviour
{

    public Transform head;
    public Transform leftController;
    public Transform rightController;

    Player player = null;

    void Awake()
    {
//        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
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

    private void Update()
    {

        this.transform.position = Vector3.Lerp(this.transform.position, player.transform.position, 0.05f);
    }
}

