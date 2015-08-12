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
	int _verticalCount = 0;
	public int verticalCount {
		set{
			_verticalCount = value;
			if (value != 0) 
			{
				Cell.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
			}
		}
		get{
			return _verticalCount;
		}
	}

	int _horzonCount = 0;
	public int horizonCount {
		set{
			_horzonCount = value;
			if (value != 0) 
			{
				Cell.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
			}
		}
		get{
			return _horzonCount;
		}
	}
	public bool isCenter = false;
	public int mPriority = 0;

	public void MoveToAndElim(Grid g,float moveTime)
	{
		if (g && Cell) 
		{
			Cell.MoveTo(g.Row,g.Col,moveTime);
			DestroyCell (moveTime);

			
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

	
	public bool isEmpty()
	{
		return Cell == null ? true: false;
	}
	
	public void DestroyCell(float t = 0f,bool isPlayElim = false)
	{
		if (Cell != null) 
		{
			if (isPlayElim) 
			{
				Cell.PlayElimAnim(t);
			}
			
			Destroy(Cell.gameObject,t);
			Cell = null;

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
			DestroyCell(Constants.CELL_ELIM_TIME,true);
			gridMgr.DropCell (Constants.FORM_TIME + 0.1f);
			gridMgr.DropNewCells (Constants.FORM_TIME + 0.1f);
			
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
