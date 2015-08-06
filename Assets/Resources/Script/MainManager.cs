using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainManager : MonoBehaviour {

	public static MainManager instance;
	
	GameObject _cellHolder;

	public GridsManager mainGrids;

	CellColor genCellColor()
	{
		CellColor curColor = (CellColor)Random.Range (1, 5);

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


	
	public bool isNormal()
	{
		return _state == GameState.Normal;
	
	}

	public GameState _state {
		private set;
		get;
	}
	
	void PlaySwapCellAction(Grid curGrid,Grid targetGrid,bool isMove)
	{
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
		if (isMove) {
			CheckMatch (l, r);
		} else {
			_state = GameState.Normal;
		
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

		Debug.Log ("ver " + verCount + "hor " + horCount);

		_state = GameState.Normal;

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
			Debug.Log("no elim");
			return false;
		}
			

		mainGrids.DropCell (Constants.FORM_TIME);
		mainGrids.DropNewCells (Constants.FORM_TIME);
		
		return true;


	}

	bool TryGenFish(Grid g)
	{
		return false;
	}

	void ElimGridListAndGenBomb(List<Grid> lst,Grid g,BombType genBomb = BombType.None)
	{
		if (genBomb == BombType.None) {
			g.DestroyCell ();
			for (int i = 0; i < lst.Count; ++i) {
				Grid curGrid = lst [i];
				curGrid. DestroyCell();

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
