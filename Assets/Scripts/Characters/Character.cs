using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Character {

	public enum OrderType {
		MOVE = 0,
		ATTACK = 1
	}

	public enum Direction {
		NONE = 0,
		UP = 1,
		DOWN = 2,
		LEFT = 3,
		RIGHT = 4
	}

	public struct NextOrder {
		public OrderType order;
		public Direction dir;

		public static NextOrder stall() {
			return new NextOrder (OrderType.MOVE, Direction.NONE);
		}

		public NextOrder(OrderType o, Direction d) {
			order = o;
			dir = d;
		}
	}

	protected Coord pos;
	protected GameManager gm;

	public int life;
	public IntRangeRand attack;
	public int evade;
	public int detectRange;
	public bool detectThroughSight;

	protected Direction facing;

	public Character(Coord p, int l, IntRangeRand a, int e, int dr, bool dts) {
		pos = p;
		life = l;
		attack = a;
		evade = e;
		detectRange = dr;
		detectThroughSight = dts;

		facing = Direction.NONE;
	}

	public void init(Coord c, GameManager g) {
		pos = c;
		gm = g;
	}

	abstract public bool manageOrder (NextOrder o);

	void setPos(Coord c) {
		pos = c;
	}

	public Coord getPos() {
		return pos;
	}

	public void setDirection(Direction d) {
		facing = d;
	}

	public int makeAttack() {
		return attack.getRand ();
	}

	public bool makeDefense(int hit) {
		if (hit <= 0)
			return true;

		bool miss = (Random.value < 0.05 * evade);
		if (!miss) {
			life -= hit;
		}
		return miss;
	}
}
