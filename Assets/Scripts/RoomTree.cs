using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class RoomTree {

	public Room node;
	private RoomTree leftChild, rightChild;

	private float minWRatio, minHRatio;
	private Coord minCoord, maxCoord;

	public struct RoomSplit {
		public Room r1;
		public Room r2;

		public RoomSplit(Room r, Room s) {
			r1 = r;
			r2 = s;
		}
	}

	public RoomTree(Room n) {
		node = n;
		leftChild = null;
		rightChild = null;
	}

	public void setLeft(RoomTree t) {
		leftChild = t;
	}

	public void setRight(RoomTree t) {
		rightChild = t;
	}

	public Room[] ToArray() {
		Room[] rooms = { node };

		if (leftChild == null && rightChild == null) {
			return rooms;
		} else if (leftChild == null) {
			Room[] rightRooms = rightChild.ToArray ();

			return rightRooms;

		} else if (rightChild == null) {
			Room[] leftRooms = leftChild.ToArray ();

			return leftRooms;

		} else {
			Room[] rightRooms = rightChild.ToArray ();
			Room[] leftRooms = leftChild.ToArray ();

			Room[] allRooms = leftRooms.Concat (rightRooms).ToArray ();

			return allRooms;
		}
	}

	public bool contains(Coord p) {
		if (node.contains (p)) {
			if (leftChild == null && rightChild == null)
				return true;
			else
				return leftChild.contains (p) || rightChild.contains (p);
		}

		return false;
	}

	public Coord getTopLeft() {
		return node.getTopLeft ();
	}

	public Coord getBotRight() {
		return node.getBotRight ();
	}

	public int getLeft() {
		return node.getLeft ();
	}

	public int getRight() {
		return node.getRight ();
	}

	public int getTop() {
		return node.getTop ();
	}

	public int getBot() {
		return node.getBot ();
	}

	//---------------------------------------------------------------------------------------------------------------------------------
	// Generators

	public static Room generateRoom(Room area) {
		Coord origin = area.getTopLeft ();
		int roomWidth = area.getWidth ();
		int roomHeight = area.getHeight ();

		int x = (int) Random.Range (0, Mathf.Floor (roomWidth*3 / 5));
		int y = (int) Random.Range (0, Mathf.Floor (roomWidth*3 / 5));
		int w = roomWidth - x - (int) Random.Range (0, Mathf.Floor (roomWidth*3 / 5));
		int h = roomHeight - y - (int) Random.Range (0, Mathf.Floor (roomWidth*3 / 5));

		Room r = new Room (origin + new Coord (x, y), origin + new Coord (x + w, y + h));
		//Debug.Log ("Generated Room: " + r.DimensionString());

		return r;
	}

	public static RoomSplit randomSplit(Room r, float minWRatio, float minHRatio) {
		Room r1, r2;
		int mid;

		if (Random.value < (float) r.getWidth () / ((float) r.getHeight () + r.getWidth() )) {
			//Debug.Log ("Doing horizontal split. Ratio is " + (r.getWidth () / ((float) r.getHeight () + r.getWidth())).ToString());

			//Horizontal
			mid = Random.Range (r.getLeft () + 1, r.getRight () - 1);
			r1 = new Room (new Coord (r.getLeft (), r.getTop ()), new Coord (mid, r.getBot()));
			r2 = new Room (new Coord (mid + 1, r.getTop()), new Coord (r.getRight (), r.getBot ()));

			//Debug.Log ("Mid point " + mid.ToString () + " between [" + r.getLeft ().ToString () + ", " + r.getRight ().ToString () + "]");

			if (r1.getWidth () / r1.getHeight () < minWRatio || r2.getWidth () / r2.getHeight () < minWRatio) {
				//Debug.Log ("Width ratio R1: " + ((float)r1.getWidth () / r1.getHeight ()).ToString () + "; width ratio R2: " + ((float)r2.getWidth () / r2.getHeight ()).ToString ());
				//throw new System.Exception("Width/Height ratio smaller than " + minWRatio.ToString ());
				//return randomSplit (r, minWRatio, minHRatio);
				return new RoomSplit(null, null);
			}
		} else {
			//Debug.Log ("Doing vertical split. Ratio is " + (r.getWidth () / ((float) r.getHeight () + r.getWidth())).ToString());

			//Vertical
			mid = Random.Range (r.getTop () + 1, r.getBot () - 1);
			r1 = new Room (new Coord (r.getLeft(), r.getTop()), new Coord (r.getRight(), mid));
			r2 = new Room (new Coord (r.getLeft(), mid + 1), new Coord (r.getRight(), r.getBot()));

			//Debug.Log ("Mid point " + mid.ToString () + " between [" + r.getTop ().ToString () + ", " + r.getBot ().ToString () + "]");

			if (r1.getHeight () / r1.getWidth () < minHRatio || r2.getHeight () / r2.getWidth () < minHRatio) {
				//Debug.Log ("Height ratio R1: " + ((float)r1.getHeight () / r1.getWidth ()).ToString () + "Height ratio R2: " + ((float)r2.getHeight () / r2.getWidth ()).ToString ());
				//throw new System.Exception("Height/Width ratio smaller than " + minHRatio.ToString ());
				//return randomSplit (r, minWRatio, minHRatio);
				return new RoomSplit(null, null);
			}
		}

		//Debug.Log ("Split " + r.DimensionString () + " into " + r1.DimensionString () + " and " + r2.DimensionString ());

		return new RoomSplit (r1, r2);
	}

	public static RoomTree splitArea(Room area, int level, int minArea, float mw, float mh) {
		RoomTree root = new RoomTree (area);

		if (level > 0 && area.getArea() > minArea) {
			Debug.Log ("Iteration #" + level.ToString() + ". Area is " + area.getArea().ToString() + ". Splitting " + area.DimensionString ());
			RoomSplit rs;

			for(int i = 0; i < 10; i++) {
				rs = RoomTree.randomSplit (area, mw, mh);

				if (rs.r1 != null && rs.r2 != null) {
					root.leftChild = splitArea (rs.r1, level - 1, minArea, mw, mh);
					root.rightChild = splitArea (rs.r2, level - 1, minArea, mw, mh);

					return root;
				} else {
					Debug.Log ("Split ratio was too small");
				}
			}
		}

		return root;
	}

	public static RoomTree splitArea(Room area, int level, int minArea) {
		RoomTree root = new RoomTree (area);

		if (level > 0 && area.getArea() > minArea) {
			//Debug.Log ("Iteration #" + level.ToString() + ". Area is " + area.getArea().ToString() + ". Splitting " + area.DimensionString ());

			RoomSplit rs = RoomTree.randomSplit (area, 0, 0);
			root.leftChild = splitArea (rs.r1, level-1, minArea);
			root.rightChild = splitArea (rs.r2, level-1, minArea);

			int minx = root.leftChild.getLeft () <= root.rightChild.getLeft () ? root.leftChild.getLeft () : root.rightChild.getLeft ();
			int maxx = root.rightChild.getRight () >= root.leftChild.getRight () ? root.rightChild.getRight () : root.leftChild.getRight ();
			int miny = root.leftChild.getTop() <= root.rightChild.getTop () ? root.leftChild.getTop () : root.rightChild.getTop ();
			int maxy = root.rightChild.getBot () >= root.leftChild.getBot () ? root.rightChild.getBot () : root.leftChild.getBot ();

			root.minCoord = new Coord (minx, maxx);
			root.maxCoord = new Coord (miny, maxy);
		}

		else {
			root.node = generateRoom (root.node);
			root.minCoord = root.node.getTopLeft ();
			root.maxCoord = root.node.getBotRight ();
		}

		return root;
	}
}

