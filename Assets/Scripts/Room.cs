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
}
