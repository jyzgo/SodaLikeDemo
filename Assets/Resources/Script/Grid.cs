using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

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
	
	public void DestroyCell(float t = 0f)
	{
		if (Cell != null) 
		{
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

		if (!MainManager.instance.isNormal()) 
		{
			return;
		}

		StartCoroutine (unSelected ());

		isBeSelected = true;
		startPos = Input.mousePosition;
		MainManager.instance.SelectGrid (Row,Col);
	}

	IEnumerator unSelected()
	{
		yield return new WaitForSeconds (0.5f);
		isBeSelected = false;
	}
	
	void beDrag()
	{
		if (!isBeSelected) {
			return;
		}


		if (!MainManager.instance.isNormal()) 
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
		Debug.Log ("be drag row " + Row + " Col " + Col);
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
		}



		
	}


}
