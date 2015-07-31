using UnityEngine;
using System.Collections;


public class CellScript : MonoBehaviour {

	public CellColor cellColor { 
		set;
		get;

	}

	public int cellRow
	{
		set;
		get;
	}
	public int cellCol {
		set;
		get;
	}




	Transform cellTransform;
	bool rotating = false;
	uint frameSelectAEnd;

	public void Init() {

	}

	public void SetPos(int row,int col)
	{
		cellRow = row;
		cellCol = col;
	}

	int targetRow;
	int targetCol;

	bool canMove = false;

	Vector3 targetPosition;

	void FixedUpdate()
	{
		if(canMove)
		{
		    transform.position=Vector3.MoveTowards(transform.position,targetPosition,Time.deltaTime*8);//移动到指定位置
			
			if (transform.position == targetPosition) 
			{
				canMove = false;
				
			}
		}
	}



	public void SetTargetPos(int row,int col)
	{
		

		cellRow = row;
		cellCol = col;

		targetPosition = new Vector3(Constants.CELLS_LEFT + col * Constants.CELL_SIDE,
		                             Constants.CELLS_BOTTOM + row * Constants.CELL_SIDE,
		                             0f);
		canMove = true;
	}



	// Use this for initialization
	void Start () 
	{
		cellTransform = this.transform;
	
	}
	
	// Update is called once per frame
	void Update () {

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

	void beDrag()
	{
		if (!m_beSel) 
		{
			return;
		}

		var v3 = Input.mousePosition - startPos;
		v3.Normalize();
		var f = Vector3.Dot(v3, Vector3.up);
		if (Vector3.Distance(startPos, Input.mousePosition) < activeDistance) {
				// m_beSel = false;
				return;
			}
			m_beSel = false;
			if (f >= 0.5) 
			{
				Debug.Log("Up");
			}
			else if (f <= -0.5) 
			{
				Debug.Log("Down");
			}
			else 
			{
				f = Vector3.Dot(v3, Vector3.right);
				if (f >= 0.5) {
					 Debug.Log("Right");
				}
				else {
				     Debug.Log("Left");
				}

			}
	    	


	}

	bool m_beSel = false;
	void beSelected()
	{
		m_beSel = true;
		startPos = Input.mousePosition;
		MainManager.instance.SelectCell(this);
	}



	public void SetSelectedEffect() {

	}

	public void UnsetSelectedEffect() {

	}


}
