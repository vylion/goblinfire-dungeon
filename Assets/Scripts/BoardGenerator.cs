using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour {

	public enum FloorType
	{
		EMPTY = 0,
		FLOOR_OUT = 1,
		FLOOR_IN = 2,
		WALL = 3,
		DOOR = 4
	}

	public int splitIterations = 6;
	public int areaWidth = 64;
	public int unsplittableArea = 36;
	//public float paddingWeight = 0.5f;

	//public float minWRatio = 0.3f;
	//public float minHRatio = 0.3f;

	//public GameObject tilemapManager;
	public string seed;

	private RoomTree tree;
	private Room[] rooms;
	private int[,] tilemap;
	private TilemapManager map;

	// Use this for initialization
	void Start () {
		map = GetComponent<TilemapManager> ();

		if (seed != null && seed.CompareTo ("") != 0) {
			Random.InitState (seed.GetHashCode ());
		}

		Room r = new Room (new Coord (0, 0), new Coord (areaWidth, areaWidth));
		tree = RoomTree.splitArea (r, splitIterations, unsplittableArea);
		rooms = tree.ToArray ();

		debugTree ();

		//Coord minArea = tree.getMaxCoord () - tree.getMinCoord ();
		Coord minArea = new Coord (areaWidth+3, areaWidth+3);
		tilemap = new int[minArea.x,minArea.y];

		foreach (Room room in rooms) {
			r = new Room(room.getBotLeft() + new Coord(1,1), room.getTopRight() + new Coord(1,1));
			setRoomTiles (r);
			setRoomWalls (r);
		}

		map.paintMap (tilemap);
	}
	
	// Update is called once per frame
	void Update () {
		
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
				tilemap [i, j] = (int) FloorType.FLOOR_IN;
			}
		}
	}

	void setRoomWalls(Room r) {
		int x0 = r.getLeft () - 1;
		int xf = r.getRight () + 1;
		int y0 = r.getBot () - 1;
		int yf = r.getTop () + 1;

		for (int i = x0; i <= xf; i++) {
			if (tilemap [i, y0] == (int)FloorType.EMPTY)
				tilemap [i, y0] = (int) FloorType.WALL;
			if (tilemap [i, yf] == (int)FloorType.EMPTY)
				tilemap [i, yf] = (int) FloorType.WALL;
		}

		for (int j = y0 + 1; j < yf; j++) {
			if (tilemap [x0, j] == (int)FloorType.EMPTY)
				tilemap [x0, j] = (int) FloorType.WALL;
			if (tilemap [xf, j] == (int)FloorType.EMPTY)
				tilemap [xf, j] = (int) FloorType.WALL;
		}
	}
}
