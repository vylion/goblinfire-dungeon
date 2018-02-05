using UnityEngine;
using System.Collections;

[System.Serializable]
public class Coord {
	public int x, y;

	public Coord(int coordX, int coordY) {
		x = coordX;
		y = coordY;
	}

	public Coord() : this(0,0) {}

	public static Coord operator+(Coord a, Coord b)
	{
		return new Coord(a.x+b.x, a.y+b.y);
	}

	public static Coord operator-(Coord a, Coord b)
	{
		return new Coord(a.x-b.x, a.y-b.y);
	}

	public static Coord operator/(Coord a, int b)
	{
		return new Coord(a.x/b, a.y/b);
	}

	public static Coord operator*(Coord a, int b)
	{
		return new Coord(a.x*b, a.y*b);
	}

	public override string ToString() {
		return "(" + x.ToString () + ", " + y.ToString () + ")";
	}
}

