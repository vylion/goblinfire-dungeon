using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room {

	public Coord topLeft, botRight;

	private List<Coord> doors;

	public Room(Coord tl, Coord br) {
		topLeft = tl;
		botRight = br;

		doors = new List<Coord> ();
	}

	public Room() : this(new Coord(0,0), new Coord(0,0)) {}

	public Coord getTopLeft() {
		return topLeft;
	}

	public Coord getBotRight() {
		return botRight;
	}

	public int getLeft() {
		return topLeft.x;
	}

	public int getRight() {
		return botRight.x;
	}

	public int getWidth() {
		return botRight.x - topLeft.x;
	}

	public int getTop() {
		return topLeft.y;
	}

	public int getBot() {
		return botRight.y;
	}

	public int getHeight() {
		return botRight.y - topLeft.y;
	}

	public Coord getSize() {
		return new Coord (Mathf.Abs(topLeft.x - botRight.x), Mathf.Abs(topLeft.y - botRight.y));
	}

	public int getArea() {
		Coord size = new Coord (Mathf.Abs(topLeft.x - botRight.x), Mathf.Abs(topLeft.y - botRight.y));

		return size.x * size.y;
	}

	public string DimensionString() {
		return "[ " + topLeft.ToString () + "; " + botRight.ToString () + " ]";
	}

	public bool contains(Coord p) {
		return !(p.x < topLeft.x || p.x > botRight.x ||
				 p.y < topLeft.y || p.y > botRight.y);
	}
}
