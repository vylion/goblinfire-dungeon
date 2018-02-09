using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardManager : MonoBehaviour {

	public enum FloorType
	{
		EMPTY = 0,
		FLOOR_OUT = 1,
		FLOOR_IN = 2,
		WALL = 3,
		DOOR = 4
	}

	Coord[] adjacencies = {
		new Coord (-1, 0),
		new Coord (0, 1),
		new Coord (1, 0),
		new Coord (0, -1)
	};

	Coord[] diagonals = {
		new Coord (-1, -1),
		new Coord (-1, 1),
		new Coord (1, 1),
		new Coord (1, -1)
	};

	public int splitIterations = 6;
	public int areaWidth = 64;
	public int unsplittableArea = 36;
	//public float paddingWeight = 0.5f;

	//public float minWRatio = 0.3f;
	//public float minHRatio = 0.3f;

	//public GameObject tilemapManager;
	public string seed;
	public IntRangeRand goblinsPerRoom;

	private Room area;
	private RoomTree tree;
	private Room[] rooms;
	private FloorType[,] tilemap;
	private TilemapManager map;
	private GameManager board;

	void Awake() {
		map = GetComponent<TilemapManager> ();
		board = GetComponent<GameManager> ();
	}

	// Use this for initialization
	void Start () {
		initTilemap ();
		initCorridors ();
		initWalls ();
		initBoard ();

		map.paintMap (tilemap);
	}
	
	// Update is called once per frame
	public bool isWalkable(Coord pos) {
		return tilemap [pos.x, pos.y] != FloorType.WALL && tilemap [pos.x, pos.y] != FloorType.EMPTY;
	}

	void initBoard() {
		int startRoom = UnityEngine.Random.Range (0, rooms.Length);

		board.initGrid (tilemap.GetLength(0), this);
		board.initPlayer (rooms [startRoom].randomPos ());
		board.prepareGoblins (goblinsPerRoom.max * (rooms.Length - 1));

		int nGoblins;
		for (int i = 0; i < rooms.Length; i++) {
			if (i != startRoom) {
				nGoblins = goblinsPerRoom.getRand ();
				for (int j = 0; j < nGoblins; j++) {
					bool success = false;
					Coord goblinPos = rooms [i].randomPos ();
					for(int tries = 0; tries < 5 && !success && isWalkable(goblinPos); tries++) {
						success = board.initGoblin (goblinPos);
						if(!success) goblinPos = rooms [i].randomPos ();
					}
				}
			}
		}
	}

	void initCorridors() {
		List<Room> corridors = tree.makeCorridors (new List<Room> ());

		for (int i = 0; i < corridors.Count; i++) {
			initCorridor (corridors [i]);
		}
	}

	bool isAdjacent(Coord c, FloorType t) {
		Coord neighbor;

		for (int i = 0; i < adjacencies.Length; i++) {
			neighbor = c + adjacencies [i];
			if (area.contains (neighbor)) {
				//Debug.Log ("neighbor " + neighbor.ToString ());
				try {
					if (tilemap [neighbor.x, neighbor.y] == t)
						return true;
				} catch(Exception e) {
					continue;
				}
			}
		}

		return false;
	}

	bool isAdjacent(int x, int y, FloorType t) {
		return isAdjacent (new Coord (x, y), t);
	}

	bool isDiagonallyAdjacent(Coord c, FloorType t) {
		Coord neighbor;

		for (int i = 0; i < adjacencies.Length; i++) {
			neighbor = c + diagonals [i];
			if (area.contains (neighbor)) {
				//Debug.Log ("neighbor " + neighbor.ToString ());
				try {
				if (tilemap [neighbor.x, neighbor.y] == t)
					return true;
				} catch(Exception e) {
					continue;
				}
			}
		}

		return false;
	}

	bool isDiagonallyAdjacent(int x, int y, FloorType t) {
		return isDiagonallyAdjacent (new Coord (x, y), t);
	}

	void initCorridor(Room corridor) {
		if (corridor.getBotLeft ().y == corridor.getTopRight ().y) {
			//Horizontal corridor
			//-------------------

			int x = corridor.getBotLeft ().x;
			int y = corridor.getBotLeft ().y;

			int i = 0;
			/*while (i < corridor.getWidth () && tilemap [x + i, y] == FloorType.FLOOR_IN) {
				i++;
			}*/


			int j = corridor.getWidth ();
			/*while (j > 0 && tilemap [x + j, y] == FloorType.FLOOR_IN) {
				j--;
			}

			if (j - i > 2) {
				tilemap [x + i, y] = FloorType.DOOR;
				tilemap [x + j, y] = FloorType.DOOR;
			} else if (j - i == 1) {
				int door = UnityEngine.Random.Range (0, 1);
				tilemap [corridor.getLeft () + door, y] = FloorType.DOOR;
				tilemap [corridor.getLeft () + 1 - door, y] = FloorType.FLOOR_IN;

				return;
			} else if (j - i == 0) {
				tilemap [x + i, y] = FloorType.DOOR;
				return;
			}*/

			for (int k = i; k <= j; k++) {
				tilemap [x + k, y] = FloorType.FLOOR_IN;
			}
		} else {
			//Vertical corridor
			//-----------------

			int x = corridor.getBotLeft ().x;
			int y = corridor.getBotLeft ().y;

			int i = 0;
			/*while (i < corridor.getHeight () && tilemap [x, y + i] == FloorType.FLOOR_IN) {
				i++;
			}*/


			int j = corridor.getHeight ();
			/*while (j > 0 && tilemap [x, y + j] == FloorType.FLOOR_IN) {
				j--;
			}

			if (j - i > 2) {
				tilemap [x, y + i] = FloorType.DOOR;
				tilemap [x, y + j] = FloorType.DOOR;
			} else if (j - i == 1) {
				int door = UnityEngine.Random.Range (0, 1);
				tilemap [x, corridor.getBot () + door] = FloorType.DOOR;
				tilemap [x, corridor.getBot () + 1 - door] = FloorType.FLOOR_IN;

				return;
			} else if (j - i == 0) {
				tilemap [x, y + i] = FloorType.DOOR;
				return;
			}*/

			for (int k = i; k <= j; k++) {
				tilemap [x, y + k] = FloorType.FLOOR_IN;
			}
		}
	}

	void initTilemap() {
		area = new Room (new Coord (1, 1), new Coord (areaWidth+1, areaWidth+1));

		if (seed != null && seed.CompareTo ("") != 0) {
			UnityEngine.Random.InitState (seed.GetHashCode ());
		}

		tree = RoomTree.splitArea (area, splitIterations, unsplittableArea);
		rooms = tree.ToArray ();

		debugTree ();

		//Coord minArea = tree.getMaxCoord () - tree.getMinCoord ();
		Coord minArea = new Coord (areaWidth+3, areaWidth+3);
		tilemap = new FloorType[minArea.x,minArea.y];

		Room r;
		foreach (Room room in rooms) {
			//r = new Room(room.getBotLeft()+1, room.getTopRight()+1);
			setRoomTiles (room);
			//setRoomWalls (r);
		}
	}

	void debugTree() {
		int i = 0;
		foreach (Room room in rooms) {
			i++;
			Debug.Log ("Generated room #" + i.ToString () + ": " + room.stringCoordinates ());
		}
		Debug.Log ("Generated " + rooms.Length.ToString () + " rooms out of " + Mathf.Pow(2f, splitIterations));

		Debug.Log("Dimensions are: " + ( new Room(tree.getMinCoord(), tree.getMaxCoord()).stringCoordinates() ) );
	}

	void setRoomTiles(Room r) {
		for (int i = r.getLeft(); i <= r.getRight(); i++) {
			for (int j = r.getBot(); j <= r.getTop(); j++) {
				tilemap [i, j] = FloorType.FLOOR_IN;
			}
		}
	}

	void setRoomWalls(Room r) {
		int x0 = r.getLeft () - 1;
		int xf = r.getRight () + 1;
		int y0 = r.getBot () - 1;
		int yf = r.getTop () + 1;

		for (int i = x0; i <= xf; i++) {
			if (tilemap [i, y0] == FloorType.EMPTY)
				tilemap [i, y0] =  FloorType.WALL;
			if (tilemap [i, yf] == FloorType.EMPTY)
				tilemap [i, yf] =  FloorType.WALL;
		}

		for (int j = y0 + 1; j < yf; j++) {
			if (tilemap [x0, j] == FloorType.EMPTY)
				tilemap [x0, j] =  FloorType.WALL;
			if (tilemap [xf, j] == FloorType.EMPTY)
				tilemap [xf, j] =  FloorType.WALL;
		}
	}

	void initWalls() {
		for (int i = 0; i < tilemap.GetLength(0); i++) {
			for (int j = 0; j < tilemap.GetLength(1); j++) {
				bool adjacentToRoom = (isAdjacent (i, j, FloorType.FLOOR_IN) || isDiagonallyAdjacent (i, j, FloorType.FLOOR_IN));
				if(tilemap[i,j] == FloorType.EMPTY && adjacentToRoom) {
					tilemap [i, j] = FloorType.WALL;
				}
			}
		}
	}
}
