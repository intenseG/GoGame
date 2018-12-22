using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class GoAcademy : Academy {

    public bool isPlayMode; //プレイモード: True, 学習モード: False

    //リセット時に呼ばれる
    public override void AcademyReset()
    {
        GoArea.isPlayMode = isPlayMode;
    }

    //ステップ毎に呼ばれる
    public override void AcademyStep()
    {
    }
}
