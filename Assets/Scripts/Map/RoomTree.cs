using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class RoomTree {

	public Room node;
	private RoomTree leftChild, rightChild;
	private bool horizontalSplit;
	private Coord center;

	private float minWRatio, minHRatio;
	private Coord minCoord, maxCoord;

	public struct RoomSplit {
		public Room r1;
		public Room r2;
		public bool horizontalSplit;

		public RoomSplit(Room r, Room s, bool split) {
			r1 = r;
			r2 = s;
			horizontalSplit = split;
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
		/*} else if (leftChild == null) {
			Room[] rightRooms = rightChild.ToArray ();

			return rightRooms;

		} else if (rightChild == null) {
			Room[] leftRooms = leftChild.ToArray ();

			return leftRooms;*/

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

	public void setSplit(bool split) {
		horizontalSplit = split;
	}

	public Coord getMinCoord() {
		return minCoord;
	}

	public Coord getMaxCoord() {
		return maxCoord;
	}

	public int getLeft() {
		return minCoord.x;
	}

	public int getRight() {
		return maxCoord.x;
	}

	public int getTop() {
		return maxCoord.y;
	}

	public int getBot() {
		return minCoord.y;
	}

	public Coord getCenter() {
		return center;
	}

	public bool isLeaf() {
		return leftChild == null && rightChild == null;
	}

	public bool isHorizontal() {
		return horizontalSplit;
	}

	//---------------------------------------------------------------------------------------------------------------------------------
	// Generators

	public static Room generateRoom(Room area) {
		Coord origin = area.getBotLeft ();
		int roomWidth = area.getWidth ();
		int roomHeight = area.getHeight ();

		int x = (int) Random.Range (0, Mathf.Floor (roomWidth*3 / 5) + 1);
		int y = (int) Random.Range (0, Mathf.Floor (roomHeight*3 / 5) + 1);
		int w = roomWidth - x - (int) Random.Range (0, Mathf.Floor (roomWidth*3 / 5) + 1);
		int h = roomHeight - y - (int) Random.Range (0, Mathf.Floor (roomHeight*3 / 5) + 1);

		Room r = new Room (origin + new Coord (x, y), origin + new Coord (x + w, y + h));
		//Debug.Log ("Generated Room: " + r.DimensionString());

		return r;
	}

	public static RoomSplit randomSplit(Room r, float minWRatio, float minHRatio) {
		Room r1, r2;
		int mid;

		bool horizontalSplit = Random.value < (float)r.getWidth () / ((float)r.getHeight () + r.getWidth ());
		if (horizontalSplit) {
			//Debug.Log ("Doing horizontal split. Ratio is " + (r.getWidth () / ((float) r.getHeight () + r.getWidth())).ToString());

			//Horizontal
			mid = Random.Range (r.getLeft () + 1, r.getRight () - 1);
			r1 = new Room (r.getBotLeft(), new Coord (mid, r.getTop()));
			r2 = new Room (new Coord (mid + 1, r.getBot()), r.getTopRight());

			//Debug.Log ("Mid point " + mid.ToString () + " between [" + r.getLeft ().ToString () + ", " + r.getRight ().ToString () + "]");

			if (r1.getWidth () / r1.getHeight () < minWRatio || r2.getWidth () / r2.getHeight () < minWRatio) {
				//Debug.Log ("Width ratio R1: " + ((float)r1.getWidth () / r1.getHeight ()).ToString () + "; width ratio R2: " + ((float)r2.getWidth () / r2.getHeight ()).ToString ());
				//throw new System.Exception("Width/Height ratio smaller than " + minWRatio.ToString ());
				//return randomSplit (r, minWRatio, minHRatio);
				return new RoomSplit(null, null, horizontalSplit);
			}
		} else {
			//Debug.Log ("Doing vertical split. Ratio is " + (r.getWidth () / ((float) r.getHeight () + r.getWidth())).ToString());

			//Vertical
			mid = Random.Range (r.getBot () + 1, r.getTop () - 1);
			r1 = new Room (r.getBotLeft(), new Coord (r.getRight(), mid));
			r2 = new Room (new Coord (r.getLeft(), mid + 1), r.getTopRight());

			//Debug.Log ("Mid point " + mid.ToString () + " between [" + r.getTop ().ToString () + ", " + r.getBot ().ToString () + "]");

			if (r1.getHeight () / r1.getWidth () < minHRatio || r2.getHeight () / r2.getWidth () < minHRatio) {
				//Debug.Log ("Height ratio R1: " + ((float)r1.getHeight () / r1.getWidth ()).ToString () + "Height ratio R2: " + ((float)r2.getHeight () / r2.getWidth ()).ToString ());
				//throw new System.Exception("Height/Width ratio smaller than " + minHRatio.ToString ());
				//return randomSplit (r, minWRatio, minHRatio);
				return new RoomSplit(null, null, horizontalSplit);
			}
		}

		//Debug.Log ("Split " + r.DimensionString () + " into " + r1.DimensionString () + " and " + r2.DimensionString ());

		return new RoomSplit (r1, r2, horizontalSplit);
	}

	public static RoomTree splitArea(Room area, int level, int minArea, float mw, float mh) {
		RoomTree root = new RoomTree (area);
		root.center = area.getCenter ();

		if (level > 0 && area.getArea() > minArea) {
			Debug.Log ("Iteration #" + level.ToString() + ". Area is " + area.getArea().ToString() + ". Splitting " + area.stringCoordinates ());
			RoomSplit rs;

			for(int i = 0; i < 10; i++) {
				rs = RoomTree.randomSplit (area, mw, mh);

				if (rs.r1 != null && rs.r2 != null) {
					root.leftChild = splitArea (rs.r1, level - 1, minArea, mw, mh);
					root.rightChild = splitArea (rs.r2, level - 1, minArea, mw, mh);

					int minx = Mathf.Min(root.leftChild.getLeft (), root.rightChild.getLeft ());
					int maxx = Mathf.Max(root.rightChild.getRight (), root.leftChild.getRight ());
					int miny = Mathf.Min(root.leftChild.getBot (), root.rightChild.getBot ());
					int maxy = Mathf.Max(root.rightChild.getTop (), root.leftChild.getTop ());

					root.minCoord = new Coord (minx, miny);
					root.maxCoord = new Coord (maxx, maxy);

					return root;
				} else {
					Debug.Log ("Split ratio was too small");
				}
			}
		}

		root.node = generateRoom (root.node);
		root.minCoord = root.node.getBotLeft ();
		root.maxCoord = root.node.getTopRight ();

		return root;
	}

	public static RoomTree splitArea(Room area, int level, int minArea) {
		RoomTree root = new RoomTree (area);
		root.center = area.getCenter ();

		if (level > 0 && area.getArea() > minArea) {
			//Debug.Log ("Iteration #" + level.ToString() + ". Area is " + area.getArea().ToString() + ". Splitting " + area.DimensionString ());

			RoomSplit rs = RoomTree.randomSplit (area, 0, 0);
			root.leftChild = splitArea (rs.r1, level-1, minArea);
			root.rightChild = splitArea (rs.r2, level-1, minArea);
			root.setSplit (rs.horizontalSplit);

			int minx = Mathf.Min(root.leftChild.getLeft (), root.rightChild.getLeft ());
			int maxx = Mathf.Max(root.rightChild.getRight (), root.leftChild.getRight ());
			int miny = Mathf.Min(root.leftChild.getBot (), root.rightChild.getBot ());
			int maxy = Mathf.Max(root.rightChild.getTop (), root.leftChild.getTop ());

			root.minCoord = new Coord (minx, miny);
			root.maxCoord = new Coord (maxx, maxy);
		}

		else {
			root.node = generateRoom (root.node);
			root.minCoord = root.node.getBotLeft ();
			root.maxCoord = root.node.getTopRight ();
		}

		return root;
	}

	public List<Room> makeCorridors(List<Room> corridors) {
		
		if (leftChild != null && rightChild != null) {
			List<Room> lc = leftChild.makeCorridors (corridors);
			List<Room> rc = rightChild.makeCorridors (corridors);

			corridors = lc.Concat (rc).ToList ();

			Coord start, end;

			if (leftChild.isLeaf () && rightChild.isLeaf ()) {
				if (horizontalSplit) {
					Debug.Log ("horizontal corridor");
					if (leftChild.getLeft () < rightChild.getLeft ()) {
						start = new Coord (leftChild.getRight (), this.getCenter ().y);
						end = new Coord (rightChild.getLeft (), this.getCenter ().y);
					} else {
						start = new Coord (rightChild.getRight (), this.getCenter ().y);
						end = new Coord (leftChild.getLeft (), this.getCenter ().y);
					}
				} else {
					Debug.Log ("vertical corridor");
					if (leftChild.getBot () < rightChild.getBot ()) {
						start = new Coord (this.getCenter().x, leftChild.getTop ());
						end = new Coord (this.getCenter().x, rightChild.getBot());
					} else {
						start = new Coord (this.getCenter().x, rightChild.getTop ());
						end = new Coord (this.getCenter().x, leftChild.getBot());
					}
				}
			} else {
				Debug.Log ("generic corridor");
				Coord cl, cr;
				cl = leftChild.getCenter ();
				cr = rightChild.getCenter ();
				;

				if (cl.x > cr.x || cl.y > cr.y) {
					start = cr;
					end = cl;
				} else {
					start = cl;
					end = cr;
				}
			}

			Room corridor = new Room (start, end);
			Debug.Log ("Corridor coords: " + corridor.stringCoordinates());
			corridors.Add (corridor);
		}

		return corridors;
	}
}

