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

	
	public bool isEmpty()
	{
		return Cell == null ? true: false;
	}
	
	public void DestroyCell()
	{
		if (Cell != null) 
		{
			Destroy(Cell.gameObject);
			Cell = null;
		}
	}
}
