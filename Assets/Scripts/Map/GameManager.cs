using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public GameObject playerObject;
	public GameObject goblinPrefab;
	public GameObject uiTilemap;
	public TileBase arrows;
	public GameObject winMessage;
	public GameObject loseMessage;
	public GameObject startScreen;

	private int[,] characterIDs;
	private PlayerManager player;
	private GoblinManager[] goblins;
	private Tilemap tUI;
	private Coord arrowPos;
	private GameObject enemyList;
	private BoardManager bm;
	private bool finishedGame;
	private bool gameStart;

	void Awake() {
		player = playerObject.GetComponent<PlayerManager> ();
		arrowPos = null;
		tUI = uiTilemap.GetComponent<Tilemap> ();
		finishedGame = false;
		gameStart = false;
	}

	void Update() {
		if (startScreen.activeSelf) {
			if (Input.anyKeyDown) {
				startScreen.SetActive (false);

			}
		} else if (!gameStart) {
			player.getReady (true);
			gameStart = true;
		} else if (!player.isReady ()) {
			foreach (GoblinManager g in goblins) {
				if (g != null && !g.isDead()) {
					g.manageTurn ();
				}
			}

			player.getReady (true);
		}
		if (finishedGame) {
			if (Input.GetKeyDown(KeyCode.Z)) {
				SceneManager.LoadScene (0);
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}

	bool checkEndgame() {
		if (player.isDead ()) {
			loseMessage.SetActive(true);
			return true;
		} else if(allGoblinsDead()) {
			Debug.Log ("All goblins are dead");
			winMessage.SetActive(true);
			return true;
		}

		return false;
	}

	bool allGoblinsDead() {
		foreach (GoblinManager goblin in goblins) {
			if (goblin != null && !goblin.isDead ())
				return false;
		}

		return true;
	}

	public void initPlayer(Coord pos) {
		player.init (pos, this);
		characterIDs [pos.x, pos.y] = 1;
	}

	public void initGrid(int areaWidth, BoardManager manager) {
		characterIDs = new int[areaWidth,areaWidth];
		bm = manager;
	}

	public void prepareGoblins(int n) {
		goblins = new GoblinManager[n];

		enemyList = new GameObject ();
		enemyList.name = "EnemyList";
		enemyList.transform.parent = transform.parent;
	}

	public bool initGoblin(Coord pos) {
		if (characterIDs [pos.x, pos.y] != 0)
			return false;

		GameObject newGoblin = Instantiate (goblinPrefab);
		int charId = addGoblin (newGoblin);
		if (charId >= 0) {
			newGoblin.GetComponent<GoblinManager> ().init (pos, this, charId + 2);
			characterIDs [pos.x, pos.y] = charId+2;
			newGoblin.transform.parent = enemyList.transform;
			return true;
		}

		return false;
	}

	int addGoblin(GameObject goblin) {
		int i = 0;
		while (i < goblins.Length && goblins [i] != null) {
			i++;
		}

		if (i < goblins.Length) {
			goblins [i] = goblin.GetComponent<GoblinManager> ();
			return i;
		}

		return -1;
	}

	public void paintArrows(Coord c) {
		if (arrowPos != null)
			tUI.SetTile (new Vector3Int (arrowPos.x, arrowPos.y, 0), null);
		arrowPos = c;
		tUI.SetTile (new Vector3Int (arrowPos.x, arrowPos.y, 0), arrows);
	}

	public void clearArrows(Coord c) {
		tUI.SetTile (new Vector3Int (c.x, c.y, 0), null);
		arrowPos = null;
	}

	public Coord manageOrder(Coord pos, Character.NextOrder o) {
		Coord nextPos = new Coord (pos.x, pos.y);

		switch (o.dir) {
		case Character.Direction.UP:
			nextPos += new Coord (0, 1);
			break;
		case Character.Direction.DOWN:
			nextPos += new Coord (0,-1);
			break;
		case Character.Direction.LEFT:
			nextPos += new Coord (-1,0);
			break;
		case Character.Direction.RIGHT:
			nextPos += new Coord (1, 0);
			break;
		}

		//Stop from walking outside the board or into walls
		if (outOfBounds (nextPos))
			return pos;
		
		if (!bm.isWalkable (nextPos)) {
			if (characterIDs [pos.x, pos.y] == 1)
				AudioManager.instance.Play (Sound.SoundType.HIT_WALL);
			return pos;
		}

		if(o.order == Character.OrderType.MOVE) {
			//Debug.Log ("Next pos: " + nextPos.ToString ());
			if (characterIDs [nextPos.x, nextPos.y] == 0) {
				//Character moves
				characterIDs [nextPos.x, nextPos.y] = characterIDs [pos.x, pos.y];
				characterIDs [pos.x, pos.y] = 0;
				return nextPos;
			} else {
				o.order = Character.OrderType.ATTACK;
			}
		}
		if (o.order == Character.OrderType.ATTACK) {
			if (characterIDs [nextPos.x, nextPos.y] == 1) {
				//Player gets attacked
				int cid = characterIDs [pos.x, pos.y];
				bool miss = player.defend (goblins [cid-2].attack ());
				if (!miss)
					AudioManager.instance.Play (Sound.SoundType.HIT_PLAYER);
				else
					AudioManager.instance.Play (Sound.SoundType.MISS_ATTACK);
			} else if (characterIDs [nextPos.x, nextPos.y] >= 2) {
				//Goblin gets attacked
				int attacker = characterIDs [pos.x, pos.y];
				int defender = characterIDs [nextPos.x, nextPos.y];

				bool miss = false;
				//Attacker is player
				if (attacker == 1) {
					miss = goblins [defender - 2].defend (player.attack ());
					if (!miss)
						AudioManager.instance.Play (Sound.SoundType.HIT);
					else
						AudioManager.instance.Play (Sound.SoundType.MISS_ATTACK);
				}
				//Attacker is another goblin
				else {
					miss = goblins [defender - 2].defend (goblins [attacker - 2].attack ());
					if (!miss)
						AudioManager.instance.Play (Sound.SoundType.HIT);
				}
				//Clear dead character
				if (goblins [defender - 2].isDead ()) {
					characterIDs [nextPos.x, nextPos.y] = 0;
					finishedGame = checkEndgame ();
				}
			}
		}

		return pos;
	}

	public bool outOfBounds(Coord c) {
		return c.x < 0 || c.x > characterIDs.Length || c.y < 0 || c.y > characterIDs.Length;
	}
}

