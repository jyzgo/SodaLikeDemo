﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

public class MainManager : MonoBehaviour {


	
	GameObject _cellHolder;

	public GridsManager mainGrids;

	CellColor genCellColor()
	{
		CellColor curColor = (CellColor)Random.Range (1, Constants.CORLOR_NUM);

		return curColor;
	}

	CellType genCellType()
	{
		CellType curType = CellType.Brick;//(int)Random.Range (0, 1);

		return curType;
	}

	
	int m_selRow = -1;
	int m_selCol = -1;

	public void SelectGrid(int row,int col)
	{
		m_selRow = row;
		m_selCol = col;
	
	}

	public void UnselGrid()
	{
		m_selCol = -1;
		m_selRow = -1;

	}

	Grid m_curGrid;
	Grid m_targetGrid;
	public bool MoveCellToDir(int offRow,int offCol)
	{


		int nextRow = m_selRow + offRow;
		int nextCol = m_selCol + offCol;



		if ((offRow == 0 && offCol == 0) || (offRow != 0 && offCol != 0))
		{
			return false;
		}


		if (!IsInBorder(nextRow,nextCol)) 
		{
			return false;
			
		}


		var nextGrid = mainGrids[nextRow,nextCol];
		if (!nextGrid.isAllowMove()) 
		{
			return false;
		}

		m_curGrid = mainGrids [m_selRow,m_selCol];
		m_targetGrid = mainGrids [nextRow,nextCol];

		PlaySwapCellAction(m_curGrid,m_targetGrid,true);
		
		return true;
		
	}


	
	public bool isControlAble()
	{
		return _state == GameState.Normal && IsCellsStable();
	}

	public GameState _state {
		private set;
		get;
	}


	
	void PlaySwapCellAction(Grid curGrid,Grid targetGrid,bool isMove)
	{
		if (curGrid == null || targetGrid == null) 
		{
			return;
		}

		if (curGrid.Cell == null || targetGrid.Cell == null) 
		{
			return;
		}

		int startRow = curGrid.Row;
		int startCol = curGrid.Col;
		int tarRow = targetGrid.Row;
		int tarCol = targetGrid.Col;

		curGrid.Cell.MoveTo(tarRow,tarCol,Constants.SWAP_TIME);
		targetGrid.Cell.MoveTo (startRow, startCol, Constants.SWAP_TIME);

		mainGrids.SwapCell (curGrid, targetGrid);


		_state = GameState.Moving;



		StartCoroutine (ResetMovingStateAndCheckElim (curGrid, targetGrid,isMove));


	}

	IEnumerator ResetMovingStateAndCheckElim(Grid l,Grid r,bool isMove)
	{
		yield return new WaitForSeconds(Constants.SWAP_TIME);
		_state = GameState.Normal;
		if (isMove) {
			CheckMatch (l, r);
		} else {
			
		
		}
	}

	void PlayBackAction()
	{
		PlaySwapCellAction (m_targetGrid, m_curGrid,false);

	}



	void CheckMatch(Grid l ,Grid r)
	{

		if (BombManager.instance.MatchBomb(l,r)) 
		{
			mainGrids.DropCell (Constants.FORM_TIME + 0.1f);
			mainGrids.DropNewCells (Constants.FORM_TIME + 0.1f);
			return;
		}



		if (l.isMatchColor (r)) {
			//same color no need check ,just back
			PlayBackAction();
		} else {
			List<Grid> leftList;
			BombType leftBombType;
			bool leftElim = TryElim(l,out leftList, out leftBombType) ;
			if (leftElim) {
				ElimGridListAndGenBomb (leftList, l,leftBombType);
			}
			List<Grid> rightList;
			BombType rightBombType;
			bool rightElim = TryElim(r,out rightList,out rightBombType);
			if (rightElim) {
				ElimGridListAndGenBomb (rightList, r,rightBombType);
			}
			bool isElimAble = leftElim || rightElim;
			if(isElimAble)
			{
				mainGrids.DropCell (Constants.FORM_TIME + 0.1f);
				mainGrids.DropNewCells (Constants.FORM_TIME + 0.1f);


			}else
			{
				PlayBackAction();
			}

		}

	}



	bool TryElim(Grid g,out List<Grid> finalList,out BombType curBombType)
	{

		finalList = new List<Grid>();
		curBombType = BombType.None;

		List<Grid> leftList = new List<Grid>();
		CheckSameColorAndAdd(g,0,-1,leftList);

		List<Grid> rightList = new List<Grid>();
		CheckSameColorAndAdd(g,0,1,rightList);

		List<Grid> upList = new List<Grid>();
		CheckSameColorAndAdd(g,1,0,upList);

		List<Grid> downList = new List<Grid>();
		CheckSameColorAndAdd(g,-1,0,downList);



		int horCount = leftList.Count + rightList.Count + 1;
		int verCount  = upList.Count + downList.Count + 1;

		var leftUpGrid = mainGrids [g.Row + 1, g.Col - 1];
		bool isMatchLeftUp  = g.isMatchColor(leftUpGrid);

		var upRightGrid = mainGrids [g.Row + 1, g.Col + 1];
		bool isMatchUpRight = g.isMatchColor(upRightGrid);

		var rightDownGrid = mainGrids [g.Row - 1, g.Col + 1];
		bool isMatchRightDown = g.isMatchColor(rightDownGrid);

		var downLeftGrid = mainGrids [g.Row - 1, g.Col - 1];
		bool isMatchDownLeft = g.isMatchColor(downLeftGrid);

		//striped candy
		// List<Grid> finalList = new List<Grid>();
		if (verCount >= 5 && horCount >= 2) {
			//Coloring candy
			Debug.Log("Coloring candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			curBombType = BombType.Coloring;
			// ElimGridListAndGenBomb (finalList, g,BombType.Coloring);
			
		} else if (horCount >= 5 && verCount >= 2) {
			//Coloring candy
			Debug.Log("Coloring candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			// ElimGridListAndGenBomb (finalList, g,BombType.Coloring);
			curBombType = BombType.Coloring;
		} else if (verCount >= 5 && horCount == 1) {
			//color candy	
			Debug.Log("Color candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			// ElimGridListAndGenBomb (finalList, g,BombType.Color);
			curBombType = BombType.Color;
		} else if (horCount >= 5 && verCount == 1) {
			//color candy
			Debug.Log("Color candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			curBombType = BombType.Color;
		} else if (leftList.Count > 0 && upList.Count > 0 && isMatchLeftUp) {
			//fish candy
			Debug.Log("fish1 candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			finalList.Add (leftUpGrid);
			curBombType = BombType.Fish;
			
		} else if (upList.Count > 0 && rightList.Count > 0 && isMatchUpRight) {
			//fish candy
			Debug.Log("fish2 candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			finalList.Add (upRightGrid);
			curBombType = BombType.Fish;
			
		} else if (rightList.Count > 0 && downList.Count > 0 && isMatchRightDown) {
			//fish candy
			Debug.Log("fish3 candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			finalList.Add (rightDownGrid);
			curBombType = BombType.Fish;
			
		} else if (downList.Count > 0 && leftList.Count > 0 && isMatchDownLeft) {
			//fish candy
			Debug.Log("fish4 candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			finalList.Add (downLeftGrid);
			curBombType = BombType.Fish;
			
		} else if (horCount >= 3 && horCount < 5 && verCount >= 3 && verCount < 5) {
			//square bomb
			Debug.Log("square candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			curBombType = BombType.Square;
		} else if (verCount == 4 && horCount < 3) {
			//form hor
			Debug.Log("hor candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			curBombType = BombType.Horizontal;
		} else if (horCount == 4 && verCount < 3) {
			//form verBomb
			Debug.Log("ver candy");
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			curBombType = BombType.Vertical;
		} else if (verCount == 3 && horCount < 3) {
			//vertical elim
			Debug.Log("verelim candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			ElimGridListAndGenBomb (finalList, g);
		} else if (verCount < 3 && horCount == 3) {
			//horizon elim
			Debug.Log("horelim candy");
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			ElimGridListAndGenBomb (finalList, g);
		} else {
//			Debug.Log("no elim");
			// _state = GameState.Normal;
			return false;
		}
			

		// mainGrids.DropCell (Constants.FORM_TIME + 0.1f);
		// mainGrids.DropNewCells (Constants.FORM_TIME + 0.1f);
		
		return true;


	}


	void ElimGridListAndGenBomb(List<Grid> lst,Grid g,BombType genBomb = BombType.None)
	{
		if (lst.Count == 0) 
		{
			return;
		}

		if (genBomb == BombType.None) {
			g.DestroyCell (Constants.CELL_ELIM_TIME,true,g);
			for (int i = 0; i < lst.Count; ++i) {
				Grid curGrid = lst [i];
				curGrid. DestroyCell(Constants.CELL_ELIM_TIME,true,g);

			}

		} else {
			if (g.bombType != BombType.None) 
			{
				BombManager.instance.triggerBomb(g,g);
			}


			if (!g.Cell) {
				return;
			}
			g.Cell.cellType = CellType.Bomb;
			g.Cell.cellBombType = genBomb;

			if (g.bombType == BombType.Color) 
			{
				g.Cell.cellColor = CellColor.All;
				
			}

			g.Cell.highZorder();
			g.Cell.updateCell (Constants.FORM_TIME);
			for (int i = 0; i < lst.Count; ++i) {
				Grid curGrid = lst [i];
				curGrid. MoveToAndElim(g,Constants.FORM_TIME,g);

			}
		}


	}

	public bool IsInBorder(int curRow,int curCol)
	{
		if(curRow >= ActiveMaxRow || curCol >= ActiveMaxCol || curRow <ActiveMinRow ||curCol < ActiveMinCol)
		{
			return false;
		}
		return true;
	}

	void CheckSameColorAndAdd(Grid g,int offRow,int offCol,List<Grid> curList)
	{

		int curRow = g.Row;
		int curCol = g.Col;


		while (true) {

			int nextRow = curRow + offRow;
			int nextCol = curCol + offCol;

//			Debug.Log ("nextRow "+ nextRow + "nextCol " +nextCol);

			if(!IsInBorder(nextRow,nextCol))
			{
				return;
			}
			Grid nextGrid = mainGrids[nextRow,nextCol];
			curRow = nextRow;
			curCol = nextCol;
			if(g.isMatchColor(nextGrid) && g.Cell && nextGrid.Cell.cellBombType != BombType.Coloring)
			{
				curList.Add(nextGrid);

			}else
			{
				return;
			}
		}

	}




	// public GameObject cell
	public GameObject CreateCell()
	{

		var curColor = genCellColor ();
		var curType = genCellType ();
		string cellPath = ResPath.CellPrefab+"Cell";

		GameObject cell = (GameObject)Instantiate(Resources.Load(cellPath,typeof(GameObject)));
		var sc = cell.GetComponent<CellScript> ();
		sc.init (curColor, curType);

		cell.transform.parent = _cellHolder.transform;
		
		return cell;
	}

	public int ActiveMinRow
	{
		private set;
		get;
	}

	public int ActiveMinCol
	{
		private set;
		get;
	}

	public int ActiveMaxRow
	{
		private set;
		get;
	}

	public int ActiveMaxCol
	{
		private set;
		get;
	}

	public int LevelState
	{
		private set;
		get;
	}

	public int MaxLength()
	{

		return LevelMaxRow > LevelMaxCol ? LevelMaxRow : LevelMaxCol;
		
	}

	public int LevelMaxRow
	{
		private set;
		get;
	}

	public int LevelMaxCol
	{
		private set;
		get;
	}

	void UpdateActiveArea()
	{
		if (LevelState == 0) 
		{
			//根据关卡配置
			ActiveMaxRow = Constants.MAX_ROWS;
			ActiveMaxCol = Constants.MAX_COLS;

			ActiveMinRow = 0;
			ActiveMinCol = 0;
		}

		for (int row = ActiveMinRow; row < ActiveMaxRow; ++row) 
		{
			for (int col = ActiveMinCol ; col < ActiveMaxCol; ++col) 
			{
				
				var curPrefab = CreateCell();
				CellScript sc = curPrefab.GetComponent<CellScript>();
				mainGrids.SetCell(sc,row,col);
				mainGrids.AdjustPos(row,col);

			}
			
		}

		CheckCollapse ();


	}

	public void OnCellDroppedAndAdded()
	{
		CheckCollapse();
	}

	

	List<List<Grid>> getMatchList(bool isHorizon)
	{
		const int countMin = 2;
		var groupList = new List<List<Grid>> ();

		int outterMin;
		int outterMax;
		int innerMin;
		int innerMax;

		if (isHorizon) {
			outterMin = ActiveMinRow;
			outterMax = ActiveMaxRow;

			innerMin = ActiveMinCol;
			innerMax = ActiveMaxCol;

		} else {
			outterMin = ActiveMinCol;
			outterMax = ActiveMaxCol;

			innerMin = ActiveMinRow;
			innerMax = ActiveMaxRow;
		}

		for (int outterValue = outterMin; outterValue < outterMax; outterValue++) {

			var curList = new List<Grid> ();

			for (int innerValue = innerMin; innerValue < innerMax; innerValue++) {
				Grid curGrid;
				if (isHorizon) {
					curGrid = mainGrids [outterValue, innerValue];
				} else {
					curGrid = mainGrids [innerValue, outterValue];
				}
				
				if (curList.Count == 0) {
					if (curGrid.isBombable ()) {
						curList.Add (curGrid);
						
					}
				} else {

					if (curList [0].isMatchColor (curGrid) && curGrid.isBombable ()) {
						curList.Add (curGrid);
						
					} else {
						if (curList.Count >= countMin) 
						{
							for(int i  = 0 ; i < curList.Count; ++i)
							{
								if (isHorizon) 
								{
									curList[i].horCount = curList.Count;
								}else
								{
									curList[i].verCount = curList.Count;
								}
								
							}

							groupList.Add (curList);
							
						}
					
						curList = new List<Grid> ();
						if (curGrid.isBombable ()) {
							curList.Add (curGrid);
						}
						
						
					}
						
				}
					

			}
						
			if (curList.Count >= countMin) {
				for(int i  = 0 ; i < curList.Count; ++i)
				{
					if (isHorizon) 
					{
						curList[i].horCount = curList.Count;
					}else
					{
						curList[i].verCount = curList.Count;
					}
					
				}
				groupList.Add (curList);
			
			}
		}



		return groupList;
	}



	void FindKeyGrids(List<List<Grid>> candidateList,List<Grid>finalList)
	{
		for(int i = 0; i <candidateList.Count; i ++)
		{

			int maxValueIndex = 0;
			int curMaxValue = 0;
			for(int j = 0 ; j < candidateList[i].Count; j++)
			{

				int curValue = candidateList[i][j].SumCount();
				if (curValue > curMaxValue) 
				{
					curMaxValue = curValue;
					maxValueIndex = j;
					
				}
			}

			finalList.Add(candidateList[i][maxValueIndex]);
		}
	}

	void CheckCollapse()
	{
		if (!isControlAble()) 
		{
			return;
		}

		// _state = GameState.Collapsing;

		//横向遍历 找到所有匹配项
		List<List<Grid>> horizontalList = getMatchList(true);
		List<List<Grid>> verticalList 	= getMatchList(false);

		List<Grid> finalKeyList = new List<Grid>();

		FindKeyGrids(horizontalList,finalKeyList);
		FindKeyGrids(verticalList,finalKeyList);

		finalKeyList = finalKeyList.Distinct().ToList();

		bool isCollapse = false;
		for(int i = 0 ; i < finalKeyList.Count; i ++)
		{
			var curGrid = finalKeyList[i];

			List<Grid> elimList;
			BombType genBombType;

			bool isElim = TryElim(curGrid,out elimList,out genBombType);
			if (isElim) 
			{
				isCollapse = true;
				ElimGridListAndGenBomb(elimList,curGrid,genBombType);
			}


		}


		if (isCollapse) {
			mainGrids.DropCell (Constants.FORM_TIME + 0.1f);
			mainGrids.DropNewCells (Constants.FORM_TIME + 0.1f);
		}

		ResetAllGridCount();

		

	}

	void ResetAllGridCount()
	{
		for(int curRow = ActiveMinRow; curRow < ActiveMaxRow ; ++curRow)
		{
			for(int curCol = ActiveMinCol; curCol < ActiveMaxCol ; ++ curCol)
			{
				mainGrids[curRow,curCol].ResetCount();
			}
		}
	}



	bool HasSameGrid(List<Grid> horizonList, List<Grid> verticalList)
	{
		for(int i = 0; i < horizonList.Count ;++i)
		{
			for(int j = 0 ; j < verticalList.Count; ++j)
			{
				if (horizonList[i] == verticalList[j]) 
				{
					return true;
					
				}
			}
		}
		return false;


	}



	bool mergeMatchPatten(List<Grid> horizonList ,List<Grid> verticalList,List<List<Grid>> finalKeyList)
	{
		bool isMergeable = false;
		for(int i = 0; i < horizonList.Count ;++i)
		{
			for(int j = 0 ; j < verticalList.Count; ++j)
			{
				if (horizonList[i] == verticalList[j]) 
				{
					var curList = new List<Grid>();
					curList.AddRange(horizonList);
					curList.AddRange(verticalList);
					finalKeyList.Add(curList);

					isMergeable =  true;
					break;
				}
			}
		}

		if (!isMergeable) 
		{
			finalKeyList.Add(horizonList);
			finalKeyList.Add(verticalList);
		}

		return isMergeable;
	}

	GameObject gridManager;

	GameObject bombManager;

	void Start()
	{
		LevelMaxRow = Constants.MAX_ROWS;
		LevelMaxCol = Constants.MAX_COLS;
		gridManager = new GameObject("GridsManager");
		gridManager.AddComponent<GridsManager>();

		bombManager = new GameObject("BombManager");
		bombManager.AddComponent<BombManager>();


		mainGrids = gridManager.GetComponent<GridsManager>();

		_cellHolder = new GameObject("CellHolder");

		LevelState = 0;

		UpdateActiveArea();

	}

	HashSet<CellScript> _movingCells;

	public void AddMovingCell(CellScript curCell)
	{
		if (curCell) 
		{
			_movingCells.Add(curCell);
		}
	}

	public bool IsCellsStable()
	{
		return _movingCells.Count == 0;
	}

	public void RemoveMovingCell(CellScript curCell)
	{


		if (curCell && MainManager.instance) 
		{
			_movingCells.Remove(curCell);	

			if (_movingCells.Count == 0) 
			{
				MainManager.instance.OnCellDroppedAndAdded();
			}		
		}
	}

	static readonly IList<GridDir> _dirs = new ReadOnlyCollection<GridDir>
	(
		new[]{
			new GridDir(1,0),
			new GridDir(-1,0),
			new GridDir(0,-1),
			new GridDir(0,1)
		}
	);

	void Awake()
	{	
		_movingCells = new HashSet<CellScript>();



	}

	//----

	static MainManager _instance;
	
	static public bool isActive { 
		get { 
			return _instance != null; 
		} 
	}

	
	static public MainManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(MainManager)) as MainManager;
	
			}
			return _instance;
		}
	}

	void OnGUI()
	{
		DebugGUI ();
	}




	public DebugTools _debugTool = DebugTools.None;

	void DebugGUI()
	{
		if (!Constants.DEBUG_MODE) 
		{
			return;
		}

		if (GUILayout.Button ("spoon")) 
		{
			
			if (_debugTool != DebugTools.Spoon) 
			{
				_debugTool = DebugTools.Spoon;
				Debug.Log ("spoon On");
			}else
			{
				_debugTool = DebugTools.None;
				Debug.Log ("spoon Off");
			}
			
		}
		if (GUILayout.Button ("FormBomb")) 
		{
			
			if (_debugTool != DebugTools.FormBomb) 
			{
				_debugTool = DebugTools.FormBomb;
				Debug.Log ("FormBomb On");
			}else
			{
				_debugTool = DebugTools.None;
				Debug.Log ("FormBomb Off");
			}
			
		}

	}



}
