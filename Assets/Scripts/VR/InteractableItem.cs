using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour {
    public Rigidbody rigidbody;

    private bool currentlyInteracting;

    private float velocityFactor = 20000f;
    private Vector3 posDelta;

    private float rotationFactor = 400f;
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis;

    private VRCloseInteraction attachedWand;

    private GameObject interactionPoint;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        //interactionPoint = new GameObject();
        velocityFactor /= rigidbody.mass;
        rotationFactor /= rigidbody.mass;
	}
	
	// Update is called once per frame
	void FixedUpdate() {
        if (attachedWand && currentlyInteracting)
        {
            posDelta = attachedWand.transform.position - interactionPoint.transform.position;
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.transform.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
                angle -= 360;

            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
        }
    }

    public void BeginInteraction(VRCloseInteraction controller) {
        attachedWand = controller;
        interactionPoint = new GameObject();
        interactionPoint.transform.position = controller.transform.position;
        interactionPoint.transform.rotation = controller.transform.rotation;
        interactionPoint.transform.SetParent(transform, true);

        currentlyInteracting = true;
    }

    public void EndInteraction(VRCloseInteraction controller) {
        if (controller == attachedWand) {
            attachedWand = null;
            currentlyInteracting = false;
        }
        //Destroy(interactionPoint);
    }

    public bool IsInteracting() {
        return currentlyInteracting;
    }

    public const float minScale = 0.03f;
    public const float maxScale = 0.4f;

    public void ApplyScale(float scalestep)
    {
        var finalScale = this.gameObject.transform.localScale.x + scalestep;
        finalScale = Mathf.Min(Mathf.Max(finalScale, minScale), maxScale); //limit the scale min and max
        this.gameObject.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
    }

}
