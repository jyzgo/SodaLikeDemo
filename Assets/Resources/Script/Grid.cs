using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	GridMark _markState = GridMark.NoMark;
	public GridMark MarkState{
		set
		{
			_markState = value;
		}
		get
		{
			return _markState;
		}
	}

	public void init(int row,int col)
	{
		Row = row;
		Col = col;

		Material gridMater;

		if ((row + col) %2==0) 
		{

			gridMater = Resources.Load("Material/DarkMaterial",typeof(Material)) as Material;
		}else
		{

			gridMater = Resources.Load("Material/LightMaterial",typeof(Material)) as Material;

		}


		GetComponent<MeshRenderer> ().material = gridMater;

	}

	public CellScript Cell
	{
		set;
		get;
	}
	
	public int Row {
		set;
		get;
	}
	
	public int Col {
		set;
		get;
	}
	public bool isBombable()
	{
		if (isEmpty()) 
		{
			return false;
		}

		return true;
	}

	public GameObject gridBg {
		set;
		get;
	}

	public bool isAllowMove()
	{
		if (isEmpty()) 
		{
			return false;
			
		}
		return true;
	}



	public bool allowElim()
	{
		return Cell && !Cell.IsMoving && !Cell.IsBombing && !Cell.IsUpdating;
	}

	public void MoveToAndElim(Grid g,float moveTime,Grid triggerGrid)
	{

		if (g && Cell && allowElim()) 
		{
			if (Cell.cellBombType == BombType.None) 
			{
				Cell.MoveTo(g.Row,g.Col,moveTime);
				
			}
			DestroyCell (moveTime,false,triggerGrid);
			
			

			
		}
	}

	public BombType bombType
	{

		get{
			if (!Cell) 
			{
				return BombType.None;
			}

			return Cell.cellBombType;
		}
	}
	public bool isMatchColor(Grid oth)
	{
		if (oth == null) 
		{
			return false;
			
		}
		if(Cell == null || oth.Cell == null)
		{
			return false;
		}
		
		if (Cell.cellColor == oth.Cell.cellColor) {
			return true;
		}
		return false;
	}

	int _verCount = 0;
	public int verCount{
		set{
			_verCount = value;
		}
		get
		{
			return _verCount;
		}
	}

	int _horCount = 0;
	public int horCount{
		set{
			_horCount = value;

		}
		get{
			return _horCount;
		}
	}

	public void ResetCount()
	{
		_verCount = 0;
		_horCount = 0;
	}

	public int SumCount()
	{
		return _verCount + _horCount;
	}

	public void ScaleCell(float scale = 1.5f)
	{
		if (Cell) 
		{
			Cell.transform.localScale = new Vector3(scale,scale,scale);

			
		}
	}

	public bool isEmpty()
	{
		return Cell == null ? true: false;
	}
	
	public void DestroyCell(float t = 0f,bool isPlayElim = false,Grid triggerGrid = null)
	{
		if (Cell != null && Cell.gameObject != null) 
		{
			if (isPlayElim) 
			{
				Cell.PlayElimAnim(t);
			}


			if(!Cell.IsTriggering)
			{
				Cell.IsTriggering = true;
				BombManager.instance.triggerBomb(this);
			}
			
			if (Cell && Cell.gameObject) 
			{
				Destroy(Cell.gameObject,t);
				Cell = null;
			}


		}
	}




	Vector3 startPos;
	void OnMouseDown()
	{
		beSelected();
	}
	
	void OnMouseDrag()
	{
		beDrag();
		
	}


	float activeDistance = 5f;

	bool isBeSelected = false;
	void beSelected()
	{

		if (!MainManager.instance.isControlAble()) 
		{
			return;
		}

		if(MainManager.instance._debugTool == DebugTools.Spoon)
		{
			var gridMgr = (GameObject.Find("GridsManager")).GetComponent<GridsManager>();
			DestroyCell(Constants.CELL_ELIM_TIME,true,null);
			gridMgr.DropCell (Constants.FORM_TIME + 0.1f);
			gridMgr.DropNewCells (Constants.FORM_TIME + 0.1f);
			
			return;
		}

		if (MainManager.instance._debugTool == DebugTools.FormBomb) 
		{
			Debug.Log("form ");
			BombType bombType = BombType.None;
			CellColor curColor = CellColor.None; 

			// None = 0,
			// Red  = 1,
			// Blue = 2,
			// Green = 3,
			// Brown = 4,
			// Purple = 5,
			// Yellow = 6

			if (Input.GetKey("1")) 
			{
				curColor = CellColor.Red;
				
			}else if (Input.GetKey("2")) {
				curColor = CellColor.Blue;
				
			}else if (Input.GetKey("3")) {
				curColor = CellColor.Green;
				
			}else if (Input.GetKey("4")) {
				curColor = CellColor.Brown;
				
			}else if (Input.GetKey("5")) {
				curColor = CellColor.Purple;
				
			}else if (Input.GetKey("6")) {
				curColor = CellColor.Yellow;
				
			}


			if (Input.GetKey("q")) 
			{
				bombType = BombType.None;
			}else if (Input.GetKey("w")) {
				bombType = BombType.Horizontal;
			}else if (Input.GetKey("e")) {
				bombType = BombType.Vertical;
			}else if (Input.GetKey("r")) {
				bombType = BombType.Square;
			}else if (Input.GetKey("a")) {
				bombType = BombType.Fish;
			}else if (Input.GetKey("s")) {
				bombType = BombType.Color;
			}else if (Input.GetKey("d")) {
				bombType = BombType.Coloring;
			}


			if (Cell) 
			{
				if (bombType == BombType.None) 
				{
					Cell.cellType = CellType.Brick;
				}else
				{
					Cell.cellType = CellType.Bomb;
				}

				if (curColor != CellColor.None) 
				{
					Cell.cellColor = curColor;
				}
				
				Cell.cellBombType = bombType;

				if (bombType == BombType.Color) 
				{
					Cell.cellColor = CellColor.All;
					
				}

				Cell.updateCell();
			}


			return;
			
		}

		isBeSelected = true;
		startPos = Input.mousePosition;
		MainManager.instance.SelectGrid (Row,Col);
	}

	
	void beDrag()
	{
		if (!isBeSelected) {
			return;
		}


		if (!MainManager.instance.isControlAble()) 
		{
			return;
		}


		var v3 = Input.mousePosition - startPos;
		v3.Normalize();
		var f = Vector3.Dot(v3, Vector3.up);
		if (Vector3.Distance(startPos, Input.mousePosition) < activeDistance) {
			return;
		}
		int offsetRow = -10;
		int offsetCol = - 10;
//		Debug.Log ("be drag row " + Row + " Col " + Col);
		if (f >= 0.5) 
		{
			offsetRow = 1;
			offsetCol = 0;
//			Debug.Log("Up");

		}
		else if (f <= -0.5) 
		{
			offsetRow = -1;
			offsetCol = 0;
//			Debug.Log("Down");
		}
		else 
		{
			f = Vector3.Dot(v3, Vector3.right);
			if (f >= 0.5) {
//				Debug.Log("Right");
				offsetRow = 0;
				offsetCol = 1;
			}
			else {
//				Debug.Log("Left");
				offsetRow = 0;
				offsetCol = -1;
			}
			
		}

		if (Mathf.Abs(offsetRow) + Mathf.Abs(offsetCol) == 1) 
		{

			bool isMove = MainManager.instance.MoveCellToDir (offsetRow, offsetCol);
			if (isMove) 
			{
				isBeSelected = false;
				
			}

		}





		
	}


}
