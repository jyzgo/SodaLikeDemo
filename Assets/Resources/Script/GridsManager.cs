using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GridsManager  {



	List<List<Grid>> cellGrids; 

	public void DropCell()
	{
		for (int row = 0; row < Constants.MAX_ROWS; ++row) 
		{
			for (int col = 0 ; col < Constants.MAX_COLS; ++col) 
			{
				if (cellGrids[row][col].isEmpty()) 
				{
					int nextRow = row + 1;
					while (nextRow < Constants.MAX_ROWS) 
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

	public void DropNewCells() {
		for (int col = 0 ; col < Constants.MAX_COLS; ++col)
		{
			for (int row = 0; row < Constants.MAX_ROWS; ++row)
			{
				if (cellGrids[row][col].isEmpty()) 
				{
					int num = Constants.MAX_ROWS - row;
					for (int i=0; i<num; i++) {
						var prefab = MainManager.instance.CreateCell();
						var cell = prefab.GetComponent<CellScript>();
						SetCell(cell, row + i, col);
						AdjustPos(row + i, col);
						cell.transform.position += new Vector3(0, (Constants.MAX_ROWS - row + i) * Constants.CELL_SIDE, 0);
						cell.SetTargetPos(row + i, col);
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
			l.Cell.SetTargetPos(l.Row,l.Col);
		}

		if (r.Cell != null) 
		{
			r.Cell.SetTargetPos(r.Row,r.Col);
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

	public List<Grid> this[int row]
	{
		get
		{
			return cellGrids[row];

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

	public GridsManager()
	{//初始化棋盘
		cellGrids = new List<List<Grid>>(Constants.MAX_ROWS);

		GameObject grid = Resources.Load("Prefabs/Grid",typeof(GameObject)) as GameObject;

		for (int row = 0; row < Constants.MAX_ROWS; ++row) 
		{
			var colList = new List<Grid>(Constants.MAX_COLS);
			cellGrids.Add(colList);
			for (int col = 0 ; col < Constants.MAX_COLS; ++col) 
			{
				GameObject curGrid = Object.Instantiate(grid);
				curGrid.transform.position = getPos(row,col,Zorder.grid);
				var curScrit = curGrid.GetComponent<Grid>();
				curScrit.init(row,col);
				colList.Add(curScrit);
				
			}
			
		}

		grid.SetActive (false);


	}


}



