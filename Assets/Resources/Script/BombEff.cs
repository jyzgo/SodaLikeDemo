using UnityEngine;
using System.Collections;

public class BombEff : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!_isMoving) 
		{
			return;
		}

		float distCoverd = (Time.time - _startMoveTime)* _speed;
		float fracJourney = distCoverd/_moveLength;

		transform.position=Vector3.Lerp(transform.position,_targetPos,fracJourney);//移动到指定位置
		
		if (transform.position == _targetPos) 
		{
			_isMoving = false;
			Destroy(gameObject);
			
		}
	
	}

	Vector3 _targetPos;
	float _moveTime;
	float _startMoveTime;
	float _speed;
	float _moveLength;

	bool _isMoving = false;
	public void MoveTo(Vector3 tarPos,float t)
	{
		if (_isMoving) 
		{
			return;
		}

		_startMoveTime = Time.time;
		_targetPos = new Vector3(tarPos.x,tarPos.y,-5);
		_moveTime = t;

		_moveLength = Vector3.Distance(transform.position, _targetPos);
		_speed = _moveLength/t; 
		_isMoving = true;
		transform.localScale = new Vector3(0.6f,0/6f,0.6f);
		// GetComponent<SpriteRenderer>().sprite

	}

	public void init(CellType _cellType,BombType _cellBombType,CellColor _cellColor)
	{
		string spritePath = _cellType.ToString() +_cellBombType.ToString()+ _cellColor.ToString();

		Sprite newSprite = Resources.Load("Sprite/Cells/"+spritePath,typeof(Sprite)) as Sprite;
		GetComponent<SpriteRenderer>().sprite = newSprite;
	}
}
