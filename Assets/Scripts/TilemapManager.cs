﻿using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{

	public GameObject floor;
	public GameObject overChar;

	public TileBase dirt;
	public TileBase grass;
	public TileBase wall;
	public TileBase rocks;

	private Coord minCoord = new Coord(0,0);
	private Coord maxCoord = new Coord(0,0);
	private Tilemap tFloor;
	private Tilemap tOverChar;
	private int[,] mapTiles;

	// Use this for initialization
	void Start ()
	{
		tFloor = floor.GetComponent<Tilemap> ();

		//tFloor.BoxFill (new Vector3Int (0, 0, 2), wall, 0, 0, 4, 1);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void setMinCoord(Coord c) {
		minCoord = c;
	}

	public void setMaxCoord(Coord c) {
		maxCoord = c;
	}

	public void setCoordRange(Coord min, Coord max) {
		minCoord = min;
		maxCoord = max;
	}

	public void setMap(int areaWidth) {
		mapTiles = new int[areaWidth, areaWidth];
	}

	public void paintRoom(Room r) {
		paintRect (tFloor, r.getBotLeft (), r.getTopRight (), 0, rocks);
	}

	public void paintRoomBorder(Room r) {
		paintBorder (tFloor, r.getBotLeft (), r.getTopRight (), 0, wall);
	}

	void paintRect(Tilemap map, Coord minCoord, Coord maxCoord, int z, TileBase tile) {
		int x0 = minCoord.x > maxCoord.x ? maxCoord.x : minCoord.x;
		int y0 = minCoord.y > maxCoord.y ? maxCoord.y : minCoord.y;
		int xf = maxCoord.x > minCoord.x ? maxCoord.x : minCoord.x;
		int yf = maxCoord.y > minCoord.y ? maxCoord.y : minCoord.y;

		for (int i = x0; i <= xf; i++) {
			for (int j = y0; j <= yf; j++) {
				map.SetTile (new Vector3Int (i, j, z), tile);
			}
		}
	}

	void paintBorder(Tilemap map, Coord minCoord, Coord maxCoord, int z, TileBase tile) {
		int x0 = minCoord.x > maxCoord.x ? maxCoord.x : minCoord.x;
		int y0 = minCoord.y > maxCoord.y ? maxCoord.y : minCoord.y;
		int xf = maxCoord.x > minCoord.x ? maxCoord.x : minCoord.x;
		int yf = maxCoord.y > minCoord.y ? maxCoord.y : minCoord.y;

		for (int i = x0 - 1; i <= xf + 1; i++) {
			map.SetTile (new Vector3Int (i, y0 - 1, z), tile);
			map.SetTile (new Vector3Int (i, yf + 1, z), tile);
		}

		for (int j = y0; j <= yf; j++) {
			map.SetTile (new Vector3Int (x0 - 1, j, z), tile);
			map.SetTile (new Vector3Int (xf + 1, j, z), tile);
		}
	}
}

