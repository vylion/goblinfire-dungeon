using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room {

	public Coord botLeft, topRight;

	private List<Coord> doors;

	public Room(Coord bl, Coord tr) {
		botLeft = bl;
		topRight = tr;

		doors = new List<Coord> ();
	}

	public Room() : this(new Coord(0,0), new Coord(0,0)) {}

	public Coord getBotLeft() {
		return botLeft;
	}

	public Coord getTopRight() {
		return topRight;
	}

	public int getLeft() {
		return botLeft.x;
	}

	public int getRight() {
		return topRight.x;
	}

	public int getWidth() {
		return topRight.x - botLeft.x;
	}

	public int getTop() {
		return topRight.y;
	}

	public int getBot() {
		return botLeft.y;
	}

	public int getHeight() {
		return topRight.y - botLeft.y;
	}

	public Coord getCenter() {
		int cx = (int)Mathf.Floor ((topRight.x - botLeft.x) / 2) + botLeft.x;
		int cy = (int) Mathf.Floor((topRight.y - botLeft.y) / 2) + botLeft.y;

		return new Coord (cx, cy);
	}

	public Coord getDimensions() {
		return new Coord (topRight.x - botLeft.x, topRight.y - botLeft.y);
	}

	public int getArea() {
		return (topRight.x - botLeft.x) * (topRight.y - botLeft.y);
	}

	public string stringCoordinates() {
		return "[ " + botLeft.ToString () + "; " + topRight.ToString () + " ]";
	}

	public bool contains(Coord p) {
		return !(p.x < botLeft.x || p.x > topRight.x ||
				 p.y < botLeft.y || p.y > topRight.y);
	}

	public bool overlaps(Room r) {
		if (this.contains (r.botLeft) || this.contains (r.topRight))
			return true;

		Coord topLeft = new Coord (r.getTop (), r.getLeft ());
		Coord botRight = new Coord (r.getBot (), r.getRight ());

		return this.contains (topLeft) || this.contains (botRight);
	}

	public Coord getDistance(Room r) {
		int distx = r.getLeft () - this.getRight ();
		if (distx <= 0 && -distx <= this.getWidth ()) {
			return new Coord (0, 0);
		}
		int distx_2 = this.getLeft () - r.getRight ();
		if (distx_2 <= 0 && -distx_2 <= this.getWidth ()) {
			return new Coord (0, 0);
		}
		distx = Mathf.Min (distx, distx_2);

		int disty = r.getTop () - this.getBot ();
		if (disty <= 0 && -disty <= this.getHeight ()) {
			return new Coord (0, 0);
		}
		int disty_2 = this.getTop () - r.getBot ();
		if (disty_2 <= 0 && -disty_2 <= this.getHeight ()) {
			return new Coord (0, 0);
		}
		disty = Mathf.Min (disty, disty_2);

		return new Coord (distx, disty);
	}

	public bool overlaps2(Room r) {
		int dist = r.getLeft () - this.getRight ();
		if (dist <= 0 && -dist <= this.getWidth ())
			return true;
		dist = this.getLeft () - r.getRight ();
		if (dist <= 0 && -dist <= this.getWidth ())
			return true;
		
		dist = r.getTop () - this.getBot ();
		if (dist <= 0 && -dist <= this.getHeight ())
			return true;
		dist = this.getTop () - r.getBot ();
		if (dist <= 0 && -dist <= this.getHeight ())
			return true;

		return false;
	}

	public Coord randomPos() {
		int x = (int)Random.Range (botLeft.x, topRight.x + 1);
		int y = (int)Random.Range (botLeft.y, topRight.y + 1);

		return new Coord (x, y);
	}
}
