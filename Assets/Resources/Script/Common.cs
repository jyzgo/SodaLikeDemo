using UnityEngine;
using System.Collections;


public enum CellColor {
	None = 0,
	Red  = 1,
	Blue = 2,
	Green = 3,
	Brown = 4,
	Purple = 5,
	Yellow = 6,
	All
};

public enum GameState
{
	Normal,
	Moving,
	Backing,
	Collapsing
}

public enum CellType
{
	None,
	Brick,
	Bomb
}

public enum BombType
{
	None,
	Horizontal,
	Vertical,
	Square,
	Fish,
	Color,
	Coloring
}

public enum GridMark
{
	NoMark,
	Marking,
	Marked
}

public struct GridDir 
{
	public int offsetRow;
	public int offsetCol;
	public GridDir(int curRow,int curCol)
	{
		offsetRow = curRow;
		offsetCol = curCol;
	}
}



public static class Zorder
{
	public const float grid = 10f;
	public const float cell = 0f;
	public const float high = -1f;

}



public static class Constants
{
	public const int MAX_ROWS = 7;
	public const int MAX_COLS = 7;
	public const float CELL_SIDE = 1.4f;
	public const float CELLS_LEFT = -5.65f;
	public const float CELLS_BOTTOM = -4.7f;
	public const float SWAP_TIME = 0.3f;
	public const float FORM_TIME = 0.33f;

	public const float CELL_ELIM_TIME = 0.13f;

	public const int CORLOR_NUM = 5;

	public const bool DEBUG_MODE = true;
}


public static class ResPath
{
	public const string CellPrefab = "Prefabs/";
	public const string SpriteRoot = "Sprite/";


}


