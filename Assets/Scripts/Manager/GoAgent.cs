using System.Collections.Generic;
using MLAgents;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

//Agent
public class GoAgent : Agent {
    //参照
    public GoArea area; //エリア
    public int agentId; //AgentID
    [HideInInspector] public int winCount;
    [HideInInspector] public string[] names;
    public List<float[]> turns;
    [HideInInspector] public int selectedAction = -1; //プレイヤーが選択したアクション(座標)

    //リセット時に呼ばれる
    public override void AgentReset () {
        names = new string[2];
        names[0] = "GoAgent0";
        names[1] = "GoAgent1";

        //Debug.Log ("Agent Reset");
        this.area.AreaReset (agentId);
    }

    //Stateの取得
    public override void CollectObservations () {

        //手番
        if (agentId == area.GetFirstAgentId ()) {
            AddVectorObs (0);
        } else {
            AddVectorObs (1);
        }

        //現在の盤面を取得
        int[, ] board = this.area.GetPoints ();

        //黒石の位置
        float[] boardB = new float[82];
        for (int i = 0; i < 82; i++) {
            if (i == 81) {
                boardB[i] = 0;
                break;
            }

            int[] indexArray = GoUtil.TapIndexToXY (i);
            int x = indexArray[0];
            int y = indexArray[1];

            if (board[x + 1, y + 1] == 0) {
                boardB[i] = 1;
            } else {
                boardB[i] = 0;
            }
        }

        AddVectorObs (boardB);

        //白石の位置
        float[] boardW = new float[82];
        for (int i = 0; i < 82; i++) {
            if (i == 81) {
                boardW[i] = 0;
                break;
            }

            int[] indexArray = GoUtil.TapIndexToXY (i);
            int x = indexArray[0];
            int y = indexArray[1];

            if (board[x, y] == 1) {
                boardW[i] = 1;
            } else {
                boardW[i] = 0;
            }
        }

        AddVectorObs (boardW);
    }

    //ステップ毎に呼ばれる
    public override void AgentAction (float[] vectorAction, string textAction) {
        if (GoArea.currentTurn != agentId) return;

        if (!GoArea.isPlayMode) {
            //学習モード
            this.area.AreaAction (this.agentId, (int) vectorAction[0], false);
        } else {
            //プレイモード
            //Agent0 -> COM
            //Agent1 -> Player
            if (GoArea.currentTurn == 0) {
                //Debug.Log ("COMの行動");
                this.area.AreaAction (this.agentId, (int) vectorAction[0], false);
            } else if (GoArea.currentTurn == 1) {
                //Debug.Log ("Playerの行動");
                this.area.AreaAction (this.agentId, selectedAction, true);
            }
        }
    }

    public void SetSelectAction (int action) {
        //Playerの行動
        this.selectedAction = action;
        area.inAction = true;
        RequestDecision ();
    }

    private void FixedUpdate () {
        if (GoArea.moveList == null) return;
        if (GoArea.currentTurn != agentId) return;
        if (area.GetTurnCount () >= GoArea.MAX_MOVE_COUNT || GoArea.isFinish) return;

        if (!GoArea.isPlayMode) {
            if (!area.inAction) {
                RequestDecision ();
            }
        } else {
            if (!area.inAction && agentId == 0) {
                RequestDecision ();
            }
        }
    }
}