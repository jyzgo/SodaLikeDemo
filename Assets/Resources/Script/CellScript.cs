using UnityEngine;
using System.Collections;


public class CellScript : MonoBehaviour {

	[SerializeField, SetProperty("CellColor")]
	private CellColor _cellColor;
	public CellColor cellColor { 
		set
		{
			if(_cellColor != value)
			{
				_cellColor = value;

			}

		}
		get
		{
			return _cellColor;
		}

	}

	public void init(CellColor curCorlor,CellType curType)
	{
		cellColor = curCorlor;
		cellType = curType;
		updateCell ();
	}

	private BombType _cellBombType = BombType.None;
	public BombType cellBombType
	{
		set{ 
			_cellBombType = value;
		}
		get{
			return _cellBombType;
		}
	}


	public void updateCell(float delayTime = 0f)
	{
		StartCoroutine(DoUpdateCell(delayTime));

	}

	IEnumerator DoUpdateCell(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		if (_cellColor != CellColor.None && _cellType != CellType.None) {

			string spritePath = _cellType.ToString() +_cellBombType.ToString()+ _cellColor.ToString();

			Sprite newSprite = Resources.Load("Sprite/Cells/"+spritePath,typeof(Sprite)) as Sprite;
			GetComponent<SpriteRenderer>().sprite = newSprite;
			
		} else {

		
		}
	}

	[SerializeField, SetProperty("CellType")]
	private CellType _cellType;
	public CellType cellType {
		set
		{
			if(_cellType != value){
				_cellType = value;

			}
		}
		get
		{
			return _cellType;
		}
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
	float moveToTargetTime;
	float startMoveTime;
	float moveLength;

	const float speed = 10;

	void FixedUpdate()
	{
		if(canMove)
		{
			float distCoverd = (Time.time - startMoveTime)* speed;
			float fracJourney = distCoverd/moveLength;


		    transform.position=Vector3.Lerp(transform.position,targetPosition,fracJourney);//移动到指定位置
			
			if (transform.position == targetPosition) 
			{
				canMove = false;
				MainManager.instance.RemoveMovingCell(this);
				
			}
		}

		if (_canScale) 
		{
			var curScale = transform.localScale;
			var tarScale = new Vector3(curScale.x * elimScale,curScale.y * elimScale ,curScale.z * elimScale);
			transform.localScale = Vector3.Lerp(transform.localScale,tarScale,Time.deltaTime * _scaleSpeed);
		}
		
	}

	const float elimScale = 0.1f;
	float _scaleTime ;
	bool _canScale = false;
	float _startScaleTime =0f;
	float _scaleSpeed = 0f;

	public void PlayElimAnim(float t)
	{
		if (t == 0f) 
		{
			return;
		}

		float oriScale = transform.localScale.x;
		float targetScale = elimScale;

		_scaleSpeed = Mathf.Abs((targetScale - oriScale))/t;

		_scaleTime = t;
		_canScale = true;

	}

	void OnDestroy()
	{
		if (canMove) 
		{
			MainManager.instance.RemoveMovingCell(this);
		}

	}


	public void MoveTo(int row,int col,float moveTime = 0f)
	{
		moveToTargetTime = moveTime;
		startMoveTime = Time.time;	

		cellRow = row;
		cellCol = col;



		targetPosition = new Vector3(Constants.CELLS_LEFT + col * Constants.CELL_SIDE,
		                             Constants.CELLS_BOTTOM + row * Constants.CELL_SIDE,
		                             Zorder.cell);
		moveLength = Vector3.Distance(transform.position, targetPosition);
		canMove = true;

		MainManager.instance.AddMovingCell(this);


	}




}
