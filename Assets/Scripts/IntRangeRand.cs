using UnityEngine;
using System.Collections;

[System.Serializable]
public class IntRangeRand {
	public int min, max;

	public IntRangeRand(int i, int j) {
		if (i > j) {
			min = j;
			max = i;
		} else {
			min = i;
			max = j;
		}
	}

	public int getRand() {
		return Random.Range (min, max+1);
	}

	public int getWeighted2Min(float weight) {
		int l = max;

		int until = (min + max) / 2 + 1;
		while (Random.value > weight && l > until)
			l--;

		return (new IntRangeRand (min, l)).getRand();
	}

	public int getWeighted2Max(float weight) {
		int f = min;

		int until = (min + max) / 2 - 1;
		while (Random.value > weight && f < until)
			f++;

		return (new IntRangeRand (f, max)).getRand();
	}
}

