﻿using UnityEngine;
using System.Collections;


public enum CellColor {
	None = 0,
	Red  = 1,
	Blue = 2,
	Green = 3,
	Brown = 4,
	Purple = 5,
	Yellow = 6
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

public static class Zorder
{
	public const float grid = 10f;
	public const float cell = 0;

}



public static class Constants
{
	public const int MAX_ROWS = 8;
	public const int MAX_COLS = 13;
	public const float CELL_SIDE = 1.4f;
	public const float CELLS_LEFT = -5.65f;
	public const float CELLS_BOTTOM = -4.7f;
	public const float SWAP_TIME = 0.3f;
	public const float FORM_TIME = 0.13f;
}


public static class ResPath
{
	public const string CellPrefab = "Prefabs/";
	public const string SpriteRoot = "Sprite/";


}


