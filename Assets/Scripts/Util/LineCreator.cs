using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreator : MonoBehaviour 
{
	public int tate = 10;
	public int yoko = 19;

	public Transform basePosi;
	//public Vector3 startVector = new Vector3(0, -374, 1);
	public float tateSize = 68;
	public float yokoSize = 32;
	// public LineRenderer tateLine;
	// public LineRenderer yokoLine;

	void Start () 
	{
		int i, j;
		for(i = 0; i < tate; i++)
		{
			for(j = 0; j < yoko; j++)
			{
				LineRenderer yokoLine = new LineRenderer();
				yokoLine.startWidth = 0.05f;
				yokoLine.endWidth = 0.05f;
				Vector3 yokoPos = new Vector3(basePosi.position.x, basePosi.position.y + tateSize * i, 1);
				yokoLine.SetPosition(j, yokoPos);
				yokoLine.material.SetColor("_Color", Color.black);
			}
			LineRenderer tateLine = new LineRenderer();
			tateLine.startWidth = 0.05f;
			tateLine.endWidth = 0.05f;
			Vector3 tatePos = new Vector3(basePosi.position.x - yokoSize * j, basePosi.position.y + tateSize * i, 1);
			tateLine.SetPosition(i, tatePos);
			tateLine.material.SetColor("_Color", Color.black);
		}
	}
}
