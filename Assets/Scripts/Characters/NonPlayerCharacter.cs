using UnityEngine;
using System.Collections;

[System.Serializable]
public class NonPlayerCharacter : Character
{
	public float turnTime = 1;

	public Sprite[] sprites = new Sprite[5];

	private float movedTime = 0;

	public NonPlayerCharacter(Coord p, int l, IntRangeRand a, int e, float tt, int dr, bool dts) : base(p, l, a, e, dr, dts) {
		turnTime = tt;
		movedTime = 0;
	}

	override public bool manageOrder (NextOrder o)
	{
		if (o.dir == Direction.NONE) {
			movedTime = 0;
			return false;
		} else if (movedTime <= 0) {
			movedTime += turnTime;
			Coord nextPos = gm.manageOrder (pos, o);
			if (nextPos.x != pos.x || nextPos.y != pos.y) {
				pos = nextPos;
				return true;
			}

			return false;
		} else {
			movedTime -= 1;
			return false;
		}
	}
}

