using UnityEngine;
using System.Collections;

public class GoblinManager : MonoBehaviour
{
	[SerializeField]
	public NonPlayerCharacter goblin;
	public GameObject spriteObject;
	public GameObject smoke;

	private SpriteRenderer renderer;
	private int charId;

	void Awake () {
		gameObject.SetActive (false);
		this.renderer = spriteObject.GetComponent<SpriteRenderer> ();
	}

	public void init(Coord c, GameManager gm, int cid) {
		goblin.init (c, gm);
		charId = cid;

		Vector3 pos = new Vector3(goblin.getPos ().x, goblin.getPos ().y, transform.position.z);
		transform.position = pos;

		gameObject.SetActive (true);
	}

	void setDirection(Character.Direction d) {
		if (d != Character.Direction.NONE) {
			this.renderer.sprite = goblin.sprites [(int)d - 1];
			goblin.setDirection (d);
		}
	}

	public int attack() {
		return goblin.makeAttack ();
	}

	public bool defend(int hit) {
		Debug.Log ("Goblin " + charId.ToString() + " defends against " + hit.ToString ());
		bool res = goblin.makeDefense (hit);
		Debug.Log ("Goblin " + charId.ToString() + " is now at " + goblin.life.ToString () + " hp");
		if (goblin.life <= 0) {
			this.renderer.sprite = goblin.sprites [4];
			this.renderer.sortingLayerName = "Items";
			smoke.SetActive (true);
			AudioManager.instance.Play (Sound.SoundType.DEATH);
		}
		return res;
	}

	public void manageTurn() {
		Character.NextOrder move = new Character.NextOrder(
			(Character.OrderType)((int)Random.Range(0,2)),
			(Character.Direction)((int)Random.Range(0,5))
		);


		setDirection (move.dir);
		goblin.manageOrder (move);
		Coord pos = goblin.getPos ();
		transform.position = new Vector3 (pos.x, pos.y, transform.position.z);
	}

	public bool isDead() {
		return goblin.life <= 0;
	}
}

