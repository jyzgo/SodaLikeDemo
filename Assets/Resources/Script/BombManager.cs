using UnityEngine;
using System.Collections;
using MTUnityLib;
using System.Collections.Generic;
using System.Linq;

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

	void CreateBombEff(Grid bombGrid,Grid targetGrid)
	{
		if (bombGrid == null || bombGrid.Cell == null) {
			return;
		}
		string effPath = ResPath.CellPrefab+"BombEff";

		GameObject bombEff = (GameObject)Instantiate(Resources.Load(effPath,typeof(GameObject)));

		bombEff.transform.position = new Vector3 (bombGrid.transform.position.x, bombGrid.transform.position.y, -1);

		var bombSc = bombEff.GetComponent<BombEff> ();
		bombSc.init (bombGrid.Cell.cellType, bombGrid.bombType, bombGrid.Cell.cellColor);
		bombSc.MoveTo (targetGrid.transform.position, Constants.CELL_ELIM_TIME + 0.5f);
	}

	void BombGridsWithDir(int originRow,int originCol,int offsetRow,int offsetCol,int distance = 100)
	{

		int curRow = originRow + offsetRow;
		int curCol = originCol + offsetCol;
		var triggerGrid = GridsManager.instance[originRow,originCol];
		int curDistance = 0;
		while (MainManager.instance.IsInBorder(curRow,curCol) && curDistance < distance) 
		{
			var curGrid = GridsManager.instance[curRow,curCol];
			if (curGrid) 
			{
				curGrid.DestroyCell(Constants.CELL_ELIM_TIME,true,triggerGrid);
			}

			curRow += offsetRow;
			curCol += offsetCol;
			curDistance +=1;
			
		}

	}

	void FishBombTrigger(Grid fishGrid,Grid triggerGrid)
	{
		if (fishGrid == null || triggerGrid == null) 
		{
			return;
		}

		UpdateBorder();

		var curGrid = FindMVPGrid();
		if (curGrid) {
			CreateBombEff (fishGrid, curGrid);
			curGrid.DestroyCell(Constants.CELL_ELIM_TIME,true,triggerGrid);

		}
	}

	Grid FindMVPGrid()
	{
		UpdateBorder();

		var candidateSet = new HashSet<Grid>();
		for (int curRow = _activeMinRow ; curRow < _activeMaxRow; ++curRow) 
		{
			for (int curCol = _activeMinCol; curCol < _activeMaxCol; ++curCol) 
			{
				var curGrid = GridsManager.instance[curRow,curCol];
				if (curGrid && curGrid.isBombable()) 
				{
					candidateSet.Add(curGrid);
				}

			}
			
		}

		if (candidateSet.Count > 0 ) 
		{
			
			Grid[] asArray = candidateSet.ToArray();
			int index = Random.Range (0, asArray.Length);
			Grid curGrid =  asArray[index];
			return curGrid;
		}
		return null;

	}

	void ColorBombTrigger(Grid bombGrid,Grid triggerGrid)
	{
		if (bombGrid == null || triggerGrid == null) 
		{
			return;
		}

		UpdateBorder();
		for (int curRow = _activeMinRow ; curRow < _activeMaxRow; ++curRow) 
		{
			for (int curCol = _activeMinCol; curCol < _activeMaxCol; ++curCol) 
			{
				var curGrid = GridsManager.instance[curRow,curCol];
				if (curGrid && curGrid != triggerGrid && curGrid.isBombable() && curGrid.isMatchColor(triggerGrid)) 
				{
					curGrid.DestroyCell(Constants.CELL_ELIM_TIME,true,triggerGrid);
					CreateBombEff(bombGrid,curGrid);
				}
			}
			
		}

		triggerGrid.DestroyCell(Constants.CELL_ELIM_TIME,true,triggerGrid);
		bombGrid.DestroyCell(Constants.CELL_ELIM_TIME,true,triggerGrid);

	}

	List<Grid> FindMostOtherColor(Grid triggerGrid)
	{
		Dictionary<CellColor,List<Grid>> curDict = new Dictionary<CellColor,List<Grid>>();
		UpdateBorder();
		for (int curRow = _activeMinRow ; curRow < _activeMaxRow; ++curRow) 
		{
			for (int curCol = _activeMinCol; curCol < _activeMaxCol; ++curCol) 
			{
				var curGrid = GridsManager.instance[curRow,curCol];
				if (curGrid  && curGrid.isBombable() && !curGrid.isMatchColor(triggerGrid)) 
				{
					var curColor = curGrid.Cell.cellColor;
					if (!curDict.ContainsKey(curColor)) 
					{
						curDict[curColor] = new List<Grid>();
					}

					curDict[curColor].Add(curGrid);
				}
			}
			
		}

		List<Grid> mostList = new List<Grid>();

		foreach (KeyValuePair<CellColor,List<Grid>> kv in curDict) {
			var curList = kv.Value;
			if (curList.Count > mostList.Count) 
			{
				mostList = curList;
			}
		}

		return mostList;

	}



	List<Grid> FindColorGrids(Grid triggerGrid)
	{
		List<Grid> curList = new List<Grid>();
		UpdateBorder();
		for (int curRow = _activeMinRow ; curRow < _activeMaxRow; ++curRow) 
		{
			for (int curCol = _activeMinCol; curCol < _activeMaxCol; ++curCol) 
			{
				var curGrid = GridsManager.instance[curRow,curCol];
				if (curGrid  && curGrid.isBombable() && curGrid.isMatchColor(triggerGrid)) 
				{
					curList.Add(curGrid);
				}
			}
			
		}
		return curList;
	}

	void TintGrids(Grid bombGrid,List<Grid> targetList)
	{
		var curColor = bombGrid.Cell.cellColor;
		for(int i = 0; i < targetList.Count; i ++ )
		{
			var curCell = targetList[i].Cell;
			if (curCell) 
			{
				curCell.cellColor = curColor;
				curCell.updateCell(Constants.CELL_ELIM_TIME);
				CreateBombEff (bombGrid, targetList[i]);

			}
		}
	}

	public void ColoringBombTrigger(Grid bombGrid,Grid triggerGrid)
	{
		if (bombGrid == null || triggerGrid == null) 
		{
			return;
		}

		CellColor targetColor;

		List<Grid> mostColorList;
		if (bombGrid.isMatchColor(triggerGrid)) 
		{
			mostColorList = FindMostOtherColor(triggerGrid);
		}else
		{
			mostColorList  = FindColorGrids(triggerGrid);
		}

		TintGrids(bombGrid,mostColorList);
		bombGrid.DestroyCell(Constants.CELL_ELIM_TIME,true,null);
		// List<Grid>


	}


	public bool MatchBomb(Grid l,Grid r)
	{
		if ( 
			(l.bombType == BombType.Color || l.bombType == BombType.Coloring) && 
			(r.bombType == BombType.Color || r.bombType == BombType.Coloring)
			) 
		{ // clear all 



			return true;
		}



		if (l.bombType == BombType.Color || r.bombType == BombType.Color) 
		{

			if (l.bombType == BombType.None) 
			{
				ColorBombTrigger(r,l);
			}else
			{
				ColorBombTrigger(l,r);
			}

			return true;
			
		}

		if (l.bombType == BombType.Coloring && r.bombType != BombType.Coloring) 
		{
			ColoringBombTrigger(l,r);
		}else if (r.bombType == BombType.Coloring && l.bombType != BombType.Coloring) {
			ColoringBombTrigger(r,l);
		}

		return false;
	}


	public void triggerBomb(Grid bombGrid,Grid triggerGrid)
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

			var leftTar = GridsManager.instance [originRow, _activeMinCol];
			var rightTar = GridsManager.instance [originRow, _activeMaxCol - 1];
			CreateBombEff (bombGrid, leftTar);
			CreateBombEff (bombGrid, rightTar);
		}
		else if(bombCell.cellBombType == BombType.Vertical)
		{
			BombGridsWithDir(originRow,originCol,1,0);
			BombGridsWithDir(originRow,originCol,-1,0);

			var upTar = GridsManager.instance [_activeMaxRow - 1, originCol];
			var downTar = GridsManager.instance [_activeMinRow, originCol];
			CreateBombEff (bombGrid, upTar);
			CreateBombEff (bombGrid, downTar);


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
		{
			FishBombTrigger(bombGrid,triggerGrid);
		}
		else if(bombCell.cellBombType == BombType.Color)
		{
			ColorBombTrigger(bombGrid,triggerGrid);
		}
		else if(bombCell.cellBombType == BombType.Coloring)
		{
			ColoringBombTrigger(bombGrid,triggerGrid);
		}




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
