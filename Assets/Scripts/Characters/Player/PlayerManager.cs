using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{

	public int life;
	public IntRangeRand attackDamage;
	public int evade;
	public int detectRange;

	public Sprite[] sprites = new Sprite[5];
	public GameObject spriteObject;

	private SpriteRenderer renderer;
	private PlayerCharacter pc;
	private CameraShake shake;

	private bool idle;
	private bool ready;
	private bool dead;

	// Use this for initialization
	void Awake ()
	{
		pc = new PlayerCharacter (new Coord(0,0), life, attackDamage, evade, detectRange);
		gameObject.SetActive (false);
		idle = true;
		ready = false;
		dead = false;
		this.renderer = spriteObject.GetComponent<SpriteRenderer> ();
		shake = GetComponent<CameraShake> ();
	}

	public void init(Coord c, GameManager gm) {
		pc.init (c, gm);

		Vector3 pos = new Vector3(pc.getPos ().x, pc.getPos ().y, transform.position.z);
		transform.position = pos;

		gameObject.SetActive (true);
		//ready = true;

		gm.paintArrows (c);
	}

	public void getReady(bool b) {
		ready = b;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (dead) {
			this.renderer.sprite = sprites [4];
		}
	}

	public bool isIdle() {
		return idle;
	}

	public bool isReady() {
		return ready && !dead;
	}

	void setDirection(Character.Direction d) {
		this.renderer.sprite = sprites [(int)d - 1];
		pc.setDirection (d);
	}

	public void manageTurn(Character.NextOrder move) {
		if (!dead) {
			ready = false;
			pc.setArrows (false);
			setDirection (move.dir);
			pc.manageOrder (move);
			Coord pos = pc.getPos ();
			transform.position = new Vector3 (pos.x, pos.y, transform.position.z);
			//ready = true;
		}
	}

	public int attack() {
		int atk = pc.makeAttack ();
		if (atk > 0)
			shake.shakeDuration = 0.3f;

		return atk;
	}

	public bool defend(int hit) {
		bool res = pc.makeDefense (hit);
		dead = (pc.life <= 0);
		if(dead) 
			AudioManager.instance.Play (Sound.SoundType.DEATH);
		return res;
	}

	public void setArrows(bool b) {
		pc.setArrows (b);
	}

	public bool isDead() {
		return dead;
	}
}

