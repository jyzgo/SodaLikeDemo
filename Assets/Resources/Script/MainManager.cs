using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainManager : MonoBehaviour {


	
	GameObject _cellHolder;

	public GridsManager mainGrids;

	CellColor genCellColor()
	{
		CellColor curColor = (CellColor)Random.Range (1, 7);

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


		if (nextRow < ActiveMinRow || nextRow >= ActiveMaxRow || nextCol < ActiveMinCol || nextCol >= ActiveMaxCol) 
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
		if (l.isMatchColor (r)) {
			//same color no need check ,just back
			PlayBackAction();
		} else {
			//
//			Debug.Log("not same");

			bool leftElim = TryElim(l) ;
			if (leftElim) {
				Debug.Log ("lll row " + l.Row + " col " + l.Col);
			}
			bool rightElim = TryElim(r);
			if (rightElim) {
				Debug.Log ("rrr row " + r.Row + " col " + r.Col);
			}
			bool isElimAble = leftElim || rightElim;
			if(isElimAble)
			{

			}else
			{
				PlayBackAction();
			}

		}

	}



	bool TryElim(Grid g)
	{

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
		List<Grid> finalList = new List<Grid>();
		if (verCount >= 5 && horCount >= 2) {
			//Coloring candy
			Debug.Log("Coloring candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			ElimGridListAndGenBomb (finalList, g,BombType.Coloring);
			
		} else if (horCount >= 5 && verCount >= 2) {
			//Coloring candy
			Debug.Log("Coloring candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			ElimGridListAndGenBomb (finalList, g,BombType.Coloring);
		} else if (verCount >= 5 && horCount == 1) {
			//color candy	
			Debug.Log("Color candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			ElimGridListAndGenBomb (finalList, g,BombType.Color);
		} else if (horCount >= 5 && verCount == 1) {
			//color candy
			Debug.Log("Color candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			ElimGridListAndGenBomb (finalList, g,BombType.Color);
		} else if (leftList.Count > 0 && upList.Count > 0 && isMatchLeftUp) {
			//fish candy
			Debug.Log("fish1 candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			finalList.Add (leftUpGrid);
			ElimGridListAndGenBomb (finalList, g,BombType.Fish);
			
		} else if (upList.Count > 0 && rightList.Count > 0 && isMatchUpRight) {
			//fish candy
			Debug.Log("fish2 candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			finalList.Add (upRightGrid);
			ElimGridListAndGenBomb (finalList, g,BombType.Fish);
			
		} else if (rightList.Count > 0 && downList.Count > 0 && isMatchRightDown) {
			//fish candy
			Debug.Log("fish3 candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			finalList.Add (rightDownGrid);
			ElimGridListAndGenBomb (finalList, g,BombType.Fish);
			
		} else if (downList.Count > 0 && leftList.Count > 0 && isMatchDownLeft) {
			//fish candy
			Debug.Log("fish4 candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			finalList.Add (downLeftGrid);
			ElimGridListAndGenBomb (finalList, g,BombType.Fish);
			
		} else if (horCount >= 3 && horCount < 5 && verCount >= 3 && verCount < 5) {
			//square bomb
			Debug.Log("square candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			ElimGridListAndGenBomb (finalList, g,BombType.Square);
		} else if (verCount == 4 && horCount < 3) {
			//form hor
			Debug.Log("hor candy");
			finalList.AddRange (upList);
			finalList.AddRange (downList);
			ElimGridListAndGenBomb (finalList, g,BombType.Horizontal);
		} else if (horCount == 4 && verCount < 3) {
			//form verBomb
			Debug.Log("ver candy");
			finalList.AddRange (rightList);
			finalList.AddRange (leftList);
			ElimGridListAndGenBomb (finalList, g,BombType.Vertical);
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
			

		mainGrids.DropCell (Constants.FORM_TIME + 0.1f);
		mainGrids.DropNewCells (Constants.FORM_TIME + 0.1f);
		
		return true;


	}


	void ElimGridListAndGenBomb(List<Grid> lst,Grid g,BombType genBomb = BombType.None)
	{
		if (genBomb == BombType.None) {
			g.DestroyCell (Constants.CELL_ELIM_TIME,true);
			for (int i = 0; i < lst.Count; ++i) {
				Grid curGrid = lst [i];
				curGrid. DestroyCell(Constants.CELL_ELIM_TIME,true);

			}

		} else {
			g.Cell.cellType = CellType.Bomb;
			g.Cell.cellBombType = genBomb;
			g.Cell.updateCell (Constants.FORM_TIME);
			for (int i = 0; i < lst.Count; ++i) {
				Grid curGrid = lst [i];
				curGrid. MoveToAndElim(g,Constants.FORM_TIME);

			}
		}


	}
	void CheckSameColorAndAdd(Grid g,int offRow,int offCol,List<Grid> curList)
	{

		int curRow = g.Row;
		int curCol = g.Col;


		while (true) {

			int nextRow = curRow + offRow;
			int nextCol = curCol + offCol;

//			Debug.Log ("nextRow "+ nextRow + "nextCol " +nextCol);

			if(nextRow >= ActiveMaxRow || nextCol >= ActiveMaxCol || nextRow <ActiveMinRow ||nextCol < ActiveMinCol)
			{
				return;
			}
			Grid nextGrid = mainGrids[nextRow,nextCol];
			curRow = nextRow;
			curCol = nextCol;
			if(g.isMatchColor(nextGrid))
			{
				curList.Add(nextGrid);

			}else
			{
				return;
			}
		}

	}




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
		Debug.Log("droopped");
		CheckCollapse();
	}

	List<List<Grid>> getHorizontalMatchedList()
	{
		List<List<Grid>> horizontalList = new List<List<Grid>>();
		for (int curRow = ActiveMinRow; curRow < ActiveMaxRow; ++curRow) 
		{
			for (int curCol = ActiveMinCol ; curCol < ActiveMaxCol; ++curCol) 
			{
				var curGrid = mainGrids[curRow,curCol];
				Debug.Log ("horr row " + curRow + "col" + curCol);
				if (!curGrid.isBombable()) 
				{
					continue;
				}
				int nextCol = curCol ;
				List<Grid> matchGrids = new List<Grid>();

				while (curRow < ActiveMaxRow && nextCol < ActiveMaxCol && mainGrids[curRow,nextCol].isBombable() 
					&& mainGrids[curRow,curCol].isMatchColor(mainGrids[curRow,nextCol])) 
				{
					matchGrids.Add(mainGrids[curRow,nextCol]);
					nextCol++;
					
				}

				if (matchGrids.Count >= 2) 
				{
					horizontalList.Add(matchGrids);

					if (nextCol + 1 < ActiveMaxCol) 
					{
						curCol = nextCol + 1;
					}else
					{
						curCol = 0;
						curRow++;
						if (curRow >= ActiveMaxRow) {
							break;
						}
					}


				}

			}
			
		}

		return horizontalList;
	}

	List<List<Grid>> getVerticalMatchedList()
	{
		List<List<Grid>> verticalList = new List<List<Grid>>();
		for (int curCol = ActiveMinCol; curCol < ActiveMaxCol; ++curCol) 
		{
			for (int curRow = ActiveMinRow ; curRow < ActiveMaxRow; ++curRow) 
			{
				Debug.Log ("horr row " + curRow + "col" + curCol);
				var curGrid = mainGrids[curRow,curCol];
				if (!curGrid.isBombable()) 
				{
					continue;
				}

				int nextRow = curRow ;
				List<Grid> matchGrids = new List<Grid>();
				while (curCol < ActiveMaxCol && nextRow < ActiveMaxRow && 
					mainGrids[curRow,curCol].isMatchColor(mainGrids[nextRow,curCol]) &&
				 	mainGrids[nextRow,curCol].isBombable())
		           {
		               matchGrids.Add(mainGrids[nextRow,curCol]);
		               nextRow ++;
		           }
		           
		           if (matchGrids.Count >= 2) {
		               //符合消除条件
					verticalList.Add(matchGrids);
		               //更新坐标
		               if (nextRow + 1 < ActiveMaxRow) {
		                   curRow = nextRow + 1;
		               }else
		               {
		                   curRow = 0;

		                   curCol ++;
						if (curCol >= ActiveMaxCol) {
							break;
						}
		                   
		               }
		           }


			}
			
		}
		return verticalList;

	}

	void CheckCollapse()
	{
		if (!isControlAble()) 
		{
			return;
		}

		// _state = GameState.Collapsing;

		//横向遍历 找到所有匹配项
		List<List<Grid>> horizontalList = getHorizontalMatchedList();
		List<List<Grid>> verticalList 	= getVerticalMatchedList();

		Debug.Log("hor count "+ horizontalList.Count);
		Debug.Log("ver count "+ verticalList.Count);

		List<Grid> finalKeyList = new List<Grid>();


		for(int i = 0; i < horizontalList.Count ;i++)
		{
			for(int j = 0; j < verticalList.Count ; j++)
			{
				if(mergeMatchPatten(horizontalList[i],verticalList[j],finalKeyList))
				{
					verticalList.Remove(verticalList[j]);
					break;
				}

			}
		}

		Debug.Log ("finnnn  cout " + finalKeyList.Count);

		for(int i = 0 ; i < verticalList.Count; i++)
		{
			finalKeyList.Add(verticalList[i][0]);

		}

		for(int i = 0 ; i < finalKeyList.Count ; i ++)
		{
			var grid = finalKeyList[i];
			Debug.Log("fin row " + grid.Row + "col" + grid.Col);
			TryElim(finalKeyList[i]);

		}


		
	}

	bool mergeMatchPatten(List<Grid> horizonList ,List<Grid> verticalList,List<Grid> finalKeyList)
	{
		bool isMergeable = false;
		for(int i = 0; i < horizonList.Count ;++i)
		{
			for(int j = 0 ; j < verticalList.Count; ++j)
			{
				if (horizonList[i] == verticalList[j]) 
				{
					finalKeyList.Add(verticalList[j]);
					isMergeable =  true;
					break;
				}
			}
		}

		if (!isMergeable) 
		{
			finalKeyList.Add(horizonList[0]);
		}

		return isMergeable;
	}

	GameObject gridManager;

	void Start()
	{
		LevelMaxRow = Constants.MAX_ROWS;
		LevelMaxCol = Constants.MAX_COLS;
		gridManager = new GameObject();
		gridManager.AddComponent<GridsManager>();


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


		if (curCell) 
		{
			_movingCells.Remove(curCell);	

			if (_movingCells.Count == 0) 
			{
				MainManager.instance.OnCellDroppedAndAdded();
			}		
		}
	}

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
	
				if (_instance == null)
				{
					GameObject go = new GameObject("_MainManager");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<MainManager>();

				}
			}
			return _instance;
		}
	}



}
