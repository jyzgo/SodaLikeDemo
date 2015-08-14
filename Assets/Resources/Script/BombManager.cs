using UnityEngine;
using System.Collections;
using MTUnityLib;

public class BombManager : MonoBehaviour {

	int _activeMinRow;
	int _activeMaxRow;

	int _activeMinCol;
	int _activeMaxCol;
	void Awake()
	{



	}

	void UpdateBorder()
	{
		_activeMinRow = MainManager.instance.ActiveMinRow;
		_activeMaxRow = MainManager.instance.ActiveMaxRow;
		_activeMinCol = MainManager.instance.ActiveMinCol;
		_activeMaxCol = MainManager.instance.ActiveMaxCol;
	}

	// Use this for initialization
	void Start () {
	
	}

	void BombGridsWithDir(int originRow,int originCol,int offsetRow,int offsetCol,int distance = 100)
	{
		int curRow = originRow + offsetRow;
		int curCol = originCol + offsetCol;
		int curDistance = 0;
		while (MainManager.instance.IsInBorder(curRow,curCol) && curDistance < distance) 
		{
			var curGrid = GridsManager.instance[curRow,curCol];
			if (curGrid) 
			{
				curGrid.DestroyCell(Constants.CELL_ELIM_TIME,true);
			}

			curRow += offsetRow;
			curCol += offsetCol;
			curDistance +=1;
			
		}

	}


	public void triggerBomb(Grid bombGrid)
	{

		if (!bombGrid ) 
		{
			return;
		}
		var bombCell = bombGrid.Cell;
		if (!bombCell || bombCell.cellBombType == BombType.None) {
			return;
		}
		UpdateBorder();

		int originRow = bombGrid.Row;
		int originCol = bombGrid.Col;

		if(bombCell.cellBombType == BombType.Horizontal)
		{
			BombGridsWithDir(originRow,originCol,0,1);
			BombGridsWithDir(originRow,originCol,0,-1);
		}
		else if(bombCell.cellBombType == BombType.Vertical)
		{
			BombGridsWithDir(originRow,originCol,1,0);
			BombGridsWithDir(originRow,originCol,-1,0);
		}
		else if(bombCell.cellBombType == BombType.Square)
		{
			BombGridsWithDir(originRow,originCol, 0,-1,1);
			BombGridsWithDir(originRow,originCol, 1,-1,1);
			BombGridsWithDir(originRow,originCol, 1,0,1);
			BombGridsWithDir(originRow,originCol, 1,1,1);
			BombGridsWithDir(originRow,originCol, 0,1,1);
			BombGridsWithDir(originRow,originCol, -1,1,1);
			BombGridsWithDir(originRow,originCol, -1,0,1);
			BombGridsWithDir(originRow,originCol, -1,-1,1);
			
		}
		else if(bombCell.cellBombType == BombType.Fish)
		{}
		else if(bombCell.cellBombType == BombType.Color)
		{}
		else if(bombCell.cellBombType == BombType.Coloring)
		{}




	}

	static BombManager _instance;
	static public BombManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(BombManager)) as BombManager;

			}
			return _instance;
		}
	}

}
