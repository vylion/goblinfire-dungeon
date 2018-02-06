using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour {

	public enum Tile
	{
		EMPTY = 0,
		FLOOR = 1,
		WALL = 2,
		DOOR = 3
	}

	public int splitIterations = 6;
	public int areaWidth = 64;
	public int unsplittableArea = 36;
	//public float paddingWeight = 0.5f;

	//public float minWRatio = 0.3f;
	//public float minHRatio = 0.3f;

	public GameObject tilemapManager;

	private RoomTree tree;
	private Room[] rooms;
	private TilemapManager map;

	// Use this for initialization
	void Start () {
		map = tilemapManager.GetComponent<TilemapManager> ();

		Random.InitState ("vylion".GetHashCode ());

		Room r = new Room (new Coord (0, 0), new Coord (areaWidth, areaWidth));
		tree = RoomTree.splitArea (r, splitIterations, unsplittableArea);
		rooms = tree.ToArray ();

		debugTree ();

		foreach (Room room in rooms) {
			map.paintRoomBorder (room);
			map.paintRoom (room);
		}
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


}
