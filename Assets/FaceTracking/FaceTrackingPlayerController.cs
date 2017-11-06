using UnityEngine;

// Include the namespace required to use Unity UI
using UnityEngine.UI;
using UnityEngine.XR.iOS;

using System.Collections;
using System.Collections.Generic;

public class FaceTrackingPlayerController : MonoBehaviour {
	
	// Create public variables for player speed, and for the Text UI game objects
	public float speed;
	public Text countText;
	public Text winText;

	// Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
	private Rigidbody rb;
	private int count;

    private bool isTrackingEnabled = false;
    private Dictionary<string, float> currentBlendShapes;

	// At the start of the game..
	void Start ()
	{
		// Assign the Rigidbody component to our private rb variable
		rb = GetComponent<Rigidbody>();

		// Set the count to zero 
		count = 0;

		// Run the SetCountText function to update the UI (see below)
		SetCountText ();

		// Set the text property of our Win Text UI to an empty string, making the 'You Win' (game over message) blank
		winText.text = "";

        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
        UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;
    }

	// Each physics step..
	void FixedUpdate ()
	{
        if (!isTrackingEnabled) {
            return;
        }

        // Set some local float variables equal to the value of our Horizontal and Vertical Inputs
        float left = currentBlendShapes["eyeBlink_R"];
        float right = currentBlendShapes["eyeBlink_L"];
        float up = (currentBlendShapes["eyeLookUp_L"] + currentBlendShapes["eyeLookUp_R"]) / 2.0f * 2.0f;
        float down = (currentBlendShapes["eyeLookDown_L"] + currentBlendShapes["eyeLookDown_R"]) / 2.0f * 2.0f;

        float moveHorizontal = -left + right;
        float moveVertical = -up + down;

		// Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		// Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
		// multiplying it by 'speed' - our public player speed that appears in the inspector
		rb.AddForce (movement * speed);
	}

    void OnGUI() {
        if (isTrackingEnabled) {
            string info = "\n";
            info += "eyeBlink_L = " + currentBlendShapes["eyeBlink_L"].ToString("P0") + "\n";
            info += "eyeBlink_R = " + currentBlendShapes["eyeBlink_R"].ToString("P0") + "\n";
            info += "eyeLookUp_L = " + currentBlendShapes["eyeLookUp_L"].ToString("P0") + "\n";
            info += "eyeLookUp_R = " + currentBlendShapes["eyeLookUp_R"].ToString("P0") + "\n";
            info += "eyeLookDown_L = " + currentBlendShapes["eyeLookDown_L"].ToString("P0") + "\n";
            info += "eyeLookDown_R = " + currentBlendShapes["eyeLookDown_R"].ToString("P0") + "\n";

            GUI.skin.box.fontSize = 30;
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            GUILayout.Box(info);
            GUILayout.EndHorizontal();
        }
    }

	// When this game object intersects a collider with 'is trigger' checked, 
	// store a reference to that collider in a variable named 'other'..
	void OnTriggerEnter(Collider other) 
	{
		// ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			// Make the other game object (the pick up) inactive, to make it disappear
			other.gameObject.SetActive (false);

			// Add one to the score variable 'count'
			count = count + 1;

			// Run the 'SetCountText()' function (see below)
			SetCountText ();
		}
	}

	// Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
	void SetCountText()
	{
		// Update the text field of our 'countText' variable
		countText.text = "Count: " + count.ToString ();

		// Check if our 'count' is equal to or exceeded 12
		if (count >= 12) 
		{
			// Set the text value of our 'winText'
			winText.text = "You Win!";
		}
	}

    void FaceAdded (ARFaceAnchor anchorData) {
        isTrackingEnabled = true;
        currentBlendShapes = anchorData.blendShapes;
    }

    void FaceUpdated (ARFaceAnchor anchorData) {
        currentBlendShapes = anchorData.blendShapes;
    }

    void FaceRemoved (ARFaceAnchor anchorData) {
        isTrackingEnabled = false;
    }
}