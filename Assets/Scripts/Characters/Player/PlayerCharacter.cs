using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerCharacter : Character
{
	public PlayerCharacter (Coord p, int l, IntRangeRand a, int e, int dr) : base (p, l, a, e, dr, true) {}

	override public bool manageOrder (NextOrder o)
	{
		Coord nextPos = gm.manageOrder (pos, o);
		if (nextPos.x != pos.x || nextPos.y != pos.y) {
			pos = nextPos;
			return true;
		}

		return false;
	}

	public void setArrows(bool b) {
		if (b)
			gm.paintArrows (pos);
		else
			gm.clearArrows (pos);
	}
}

