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

	public void highZorder()
	{
		var curPos = transform.position;
		transform.position = new Vector3(curPos.x,curPos.y,Zorder.high);
	}

	public void backZorder()
	{
		var curPos = transform.position;
		transform.position = new Vector3(curPos.x,curPos.y,Zorder.cell);
	}

	bool _updating = false;
	public bool IsUpdating{
		private set
		{
			_updating = value;
		}
		get{
			return _updating;
		}
	}

	bool _isTriggering = false;
	public bool IsTriggering{
		set
		{
			_isTriggering = value;
		}
		get
		{
			return _isTriggering;
		}
	}

	public void updateCell(float delayTime = 0f)
	{
		_updating = true;

		StartCoroutine(DoUpdateCell(delayTime));

	}

	IEnumerator DoUpdateCell(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		_updating = false;
		string spritePath = _cellType.ToString() +_cellBombType.ToString()+ _cellColor.ToString();

		Sprite newSprite = Resources.Load("Sprite/Cells/"+spritePath,typeof(Sprite)) as Sprite;
		GetComponent<SpriteRenderer>().sprite = newSprite;
		if (_cellColor != CellColor.None && _cellType != CellType.None) {


			backZorder();
			
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

	bool _isMoving = false;
	public bool IsMoving {
		private set{ 
			_isMoving = value;
		}
		get
		{
			return _isMoving;
		}
	}

	Vector3 targetPosition;

	float startMoveTime;
	float moveLength;

	const float speed = 10;

	void Update()
	{
		if(_isMoving)
		{
			float distCoverd = (Time.time - startMoveTime)* speed;
			float fracJourney = distCoverd/moveLength;

		    transform.position=Vector3.Lerp(transform.position,targetPosition,fracJourney);//移动到指定位置
			
			if (transform.position == targetPosition) 
			{
				_isMoving = false;
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

	bool _canScale = false;

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


		_canScale = true;

	}

	bool _bombing = false;
	public bool IsBombing
	{
		set{
			_bombing = value;
		}
		
		get{
			return _bombing;
		}
	}

	void OnDestroy()
	{
		if (MainManager.instance) 
		{
			MainManager.instance.RemoveMovingCell(this);
		}

	}




	public void MoveTo(int row,int col,float moveTime = 0f)
	{

		startMoveTime = Time.time;	

		cellRow = row;
		cellCol = col;



		targetPosition = new Vector3(Constants.CELLS_LEFT + col * Constants.CELL_SIDE,
		                             Constants.CELLS_BOTTOM + row * Constants.CELL_SIDE,
		                             Zorder.cell);
		moveLength = Vector3.Distance(transform.position, targetPosition);
		_isMoving = true;

		MainManager.instance.AddMovingCell(this);


	}




}
