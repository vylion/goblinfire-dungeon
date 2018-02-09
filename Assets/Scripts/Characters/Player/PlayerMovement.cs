using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	PlayerManager pc;
	Character.Direction d;
	Character.OrderType o;

	// Use this for initialization
	void Start ()
	{
		pc = GetComponent<PlayerManager> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (pc.isReady ()) {

			if (Input.GetKeyDown (KeyCode.Z) && o == Character.OrderType.MOVE) {
				o = Character.OrderType.ATTACK;
				pc.setArrows (true);
			} else if (Input.GetKeyDown (KeyCode.X) && o != Character.OrderType.MOVE) {
				o = Character.OrderType.MOVE;
				pc.setArrows (false);
			} else if (Input.GetKeyDown (KeyCode.X)) {
				pc.manageTurn (Character.NextOrder.stall());
				return;
			}

			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				d = Character.Direction.UP;
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				d = Character.Direction.DOWN;
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				d = Character.Direction.LEFT;
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				d = Character.Direction.RIGHT;
			}
		}

		if (d != Character.Direction.NONE) {
			pc.manageTurn (new Character.NextOrder (o, d));

			o = Character.OrderType.MOVE;
			d = Character.Direction.NONE;
		}
	}
}

