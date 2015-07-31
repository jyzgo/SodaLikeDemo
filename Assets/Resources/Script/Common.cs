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

public enum DragDir
{
	None,
	Up,
	Down,
	Right,
	Left
}

public enum CellType
{
	None,
	Brick,
	Bomb
}

public static class Zorder
{
	public const float grid = 10f;
	public const float cell = 0;
	


}

public static class Constants
{
	public const int MAX_ROWS = 8;
	public const int MAX_COLS = 8;
	public const float CELL_SIDE = 1.4f;
	public const float CELLS_LEFT = -3.15f;
	public const float CELLS_BOTTOM = -3.2f;
}


public static class ResPath
{
	public const string CellPrefab = "Prefabs/";
	public const string SpriteRoot = "Sprite/";


}


