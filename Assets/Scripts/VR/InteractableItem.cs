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

    private Transform interactionPoint;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        interactionPoint = new GameObject().transform;
        velocityFactor /= rigidbody.mass;
        rotationFactor /= rigidbody.mass;
	}
	
	// Update is called once per frame
	void FixedUpdate() {
        if (attachedWand && currentlyInteracting)
        {
            posDelta = attachedWand.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
            {
                angle -= 360;
            }

            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
        }
    }

    public void BeginInteraction(VRCloseInteraction controller) {
        attachedWand = controller;
        interactionPoint.position = controller.transform.position;
        interactionPoint.rotation = controller.transform.rotation;
        interactionPoint.SetParent(transform, true);

        currentlyInteracting = true;
    }

    public void EndInteraction(VRCloseInteraction controller) {
        if (controller == attachedWand) {
            attachedWand = null;
            currentlyInteracting = false;
        }
    }

    public bool IsInteracting() {
        return currentlyInteracting;
    }
}
