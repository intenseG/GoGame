using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Move
{
    [SerializeField]
    public int color;
    [SerializeField]
    public int putIndex;
    [SerializeField]
    public int moveCount;
    [SerializeField]
    public List<int> prisonerList;
    [SerializeField]
    public bool isKo;
    [SerializeField]
    public int koIndex;

    public Move() {}
    
    public Move(int color, int putIndex, int moveCount, List<int> prisonerList, bool isKo, int koIndex)
    {
        this.color = color;
        this.putIndex = putIndex;
        this.prisonerList = prisonerList;
        this.moveCount = moveCount;
        this.isKo = isKo;
        this.koIndex = koIndex;
    }
}
