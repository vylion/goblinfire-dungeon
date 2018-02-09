using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
	float lerpTime = 1f;
	float currentLerpTime;

	public GameObject player;

	// Use this for initialization
	void Start ()
	{
		transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 endPos = new Vector3 (player.transform.position.x, player.transform.position.y, transform.position.z);
		if (transform.position != endPos) { 
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > lerpTime) {
				currentLerpTime = lerpTime;
			}

			//lerp!
			float perc = currentLerpTime / lerpTime;

			transform.position = Vector3.Lerp (transform.position, endPos, 0.2f);
		}
		if(transform.position == endPos && currentLerpTime > 0f)
			currentLerpTime = 0f;
	}
}

