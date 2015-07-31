using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour {

	public static MainManager instance;



	GameObject _cellHolder;

	public GridsManager mainGrids;

	CellColor genCellColor()
	{
		CellColor curColor = (CellColor)Random.Range (1, 7);

		return curColor;
	}

	CellType genCellType()
	{
		CellType curType = (CellType)0;//(int)Random.Range (0, 1);

		return curType;
	}

	
	CellScript m_beSelectedCell;

	public void SelectCell(CellScript cs)
	{
		if (cs != null) 
		{
			m_beSelectedCell = cs;
			
		}
	
	}

	public void UnSelectCell()
	{
		m_beSelectedCell = null;
	}

	public void MoveCellToDir(DragDir dir)
	{
		
	}


	public GameObject CreateCell()
	{


		var curColor = genCellColor ();
		var curType = genCellType ();
		string cellPath = ResPath.CellPrefab + curType.ToString () + curColor.ToString ();

		GameObject cell = (GameObject)Instantiate(Resources.Load(cellPath,typeof(GameObject)));
		var sc = cell.GetComponent<CellScript> ();
		sc.cellColor = curColor;
		cell.transform.parent = _cellHolder.transform;
		
		return cell;
	}

	void Start()
	{
		mainGrids = new GridsManager();

		_cellHolder = GameObject.Find ("CellHolder");

		for (int row = 0; row < Constants.MAX_ROWS; ++row) 
		{
			for (int col = 0 ; col < Constants.MAX_COLS; ++col) 
			{
				
				var curPrefab = CreateCell();
				CellScript sc = curPrefab.GetComponent<CellScript>();
				mainGrids.SetCell(sc,row,col);
				mainGrids.AdjustPos(row,col);

			}
			
		}
	}







	void Awake()
	{	
	 	if (instance != null && instance != this.gameObject) 
		{
			Destroy(instance);
			
		}
		instance = this;
		DontDestroyOnLoad(instance.gameObject);

	}



}
