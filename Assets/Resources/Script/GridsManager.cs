using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GridsManager:MonoBehaviour  {


	List<List<Grid>> cellGrids; 


	public void DropCell(float delayTime = 0f)
	{
		StartCoroutine(DoDropCell(delayTime));
	}

	IEnumerator DoDropCell(float delayTime = 0f)
	{
		yield return new WaitForSeconds(delayTime);

		for (int row = 0; row < _curMaxRow; ++row) 
		{
			for (int col = 0 ; col < _curMaxCol; ++col) 
			{
				if (cellGrids[row][col].isEmpty()) 
				{
					int nextRow = row + 1;
					while (nextRow < _curMaxRow) 
					{
						if (!cellGrids[nextRow][col].isEmpty()) 
						{
							SwapCell(cellGrids[row][col],cellGrids[nextRow][col]);
							break;
						}
						nextRow++;
						
					}
					
				}
			}
			
		}
		// AdjustAllPos();
	}

	public void DropNewCells(float delayTime = 0f)
	{
		StartCoroutine(DoDropNewCells(delayTime));
	}

	IEnumerator DoDropNewCells(float delayTime = 0f) {
		yield return new WaitForSeconds(delayTime);

		for (int col = 0 ; col < _curMaxCol; ++col)
		{
			for (int row = 0; row < _curMaxRow; ++row)
			{
				if (cellGrids[row][col].isEmpty()) 
				{
					int num = _curMaxRow - row;
					for (int i=0; i<num; i++) {
						var prefab = MainManager.instance.CreateCell();
						var cell = prefab.GetComponent<CellScript>();
						SetCell(cell, row + i, col);
						AdjustPos(row + i, col);
						cell.transform.position += new Vector3(0, (_curMaxRow - row + i) * Constants.CELL_SIDE, 0);
						cell.MoveTo(row + i, col);
					}
				}
			}
			
		}
	}

	public void AdjustAllPos()
	{
		
		foreach (var curList in cellGrids) 
		{
			foreach (var curGrid in curList) 
			{
				if (curGrid.Cell != null) 
				{
					AdjustPos(curGrid.Row,curGrid.Col);
					
				}
				
			}
			
		}
	}

	public void SwapCell(Grid l,Grid r)
	{
		var temp = l.Cell;
		l.Cell = r.Cell;
		r.Cell = temp;

		if (l.Cell != null) 
		{
			l.Cell.MoveTo(l.Row,l.Col);
		}

		if (r.Cell != null) 
		{
			r.Cell.MoveTo(r.Row,r.Col);
		}

	}

	public Vector3 getPos (int row, int col, float z)
	{
		return  new Vector3(Constants.CELLS_LEFT + col * Constants.CELL_SIDE,
		                    Constants.CELLS_BOTTOM + row * Constants.CELL_SIDE,
		                    z);
	}

	public void AdjustPos(GameObject curObj,int row,int col,float z = Zorder.cell)
	{
		curObj.transform.position = getPos (row, col,z);
	}

	public void AdjustPos(int row,int col)
	{
		var curCell = cellGrids[row][col].Cell;
		var obj = curCell.gameObject;
		AdjustPos (obj,row,col,0f);
	}

	public bool SetCell(CellScript sc ,int row,int col)
	{

		
		var grid = cellGrids[row][col];
		if (grid.Cell != null) 
		{
			return false;
		}
		grid.Cell = sc;
		grid.Cell.SetPos(row,col);
		return true;

	}

	public Grid this[int row,int col]
	{
		get
		{
			
			if (row < 0 || row >= cellGrids.Count) 
			{
				return null;
			}
			var curList = cellGrids[row];

			if (col< 0 || col >= curList.Count) 
			{
				return null;
				
			}

			return curList[col];

		}

	}


    

	public bool Destroy(int row,int col)
	{
		var grid = cellGrids[row][col];
		if (grid.Cell == null) 
		{
			return false;
		}
		grid.DestroyCell();
		return true;

	}

	GameObject _gridHolder;

	int _curMaxRow ;
	int _curMaxCol ;



	void Awake()
	{
		
		_curMaxRow = MainManager.instance.LevelMaxRow;
		_curMaxCol = MainManager.instance.LevelMaxCol;



		cellGrids = new List<List<Grid>>(_curMaxRow);
		_gridHolder = new GameObject("GridHolder");//GameObject.Find("GridHolder");


		GameObject grid = Resources.Load("Prefabs/Grid",typeof(GameObject)) as GameObject;

		for (int row = 0; row < _curMaxRow; ++row) 
		{
			var colList = new List<Grid>(_curMaxCol);
			cellGrids.Add(colList);
			for (int col = 0 ; col < _curMaxCol; ++col) 
			{
				GameObject curGrid = Object.Instantiate(grid);
				curGrid.transform.position = getPos(row,col,Zorder.grid);
				curGrid.transform.parent = _gridHolder.transform;
				var curScrit = curGrid.GetComponent<Grid>();
				curScrit.init(row,col);
				colList.Add(curScrit);

			}

		}
	}




}



