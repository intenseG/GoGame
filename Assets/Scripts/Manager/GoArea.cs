using System.Collections;
using System.Collections.Generic;
using Const;
using UnityEngine;
using UnityEngine.UI;

//エリア
public class GoArea : MonoBehaviour {
    //碁盤サイズ(盤外含む) 9+2
    public const int BOARD_SIZE = 11;

    //1局の最大手数
    public static readonly int MAX_MOVE_COUNT = 100;

    //着手リスト
    [HideInInspector] public static List<Move> moveList;

    //碁石のプレハブ
    public GameObject stonePrefab;
    public Transform goban;

    //参照
    public GoAgent[] agent = new GoAgent[2]; //Agent

    private StoneManager stoneManager = new StoneManager (); //石を制御するスクリプト
    private List<StoneManager> stoneList = new List<StoneManager> (); //生成した石のスクリプトリスト

    public Text[] winCountObj;
    public static int winCountB;
    public static int winCountW;

    //変数
    private int[, ] points = new int[BOARD_SIZE, BOARD_SIZE]; //セル（0:なし, 1:黒, 2:白, 3:盤外）
    private int[, ] reviewBoard; //状態用の碁盤配列

    private int first; //先攻AgentId

    private bool isCreated; //初期化フラグ
    public static bool isFinish; //終局フラグ
    public static int currentTurn; //現在の手番
    private int passCount; //連続パス回数

    [HideInInspector] public bool inAction;

    //private int prisonerCountB; //黒が取った白石の数
    //private int prisonerCountW; //白が取った黒石の数

    //エリアのリセット時に呼ばれる
    public void AreaReset (int agentId) {
        inAction = false;
        currentTurn = 0;
        isFinish = false;
        moveList = new List<Move> ();
        points = new int[BOARD_SIZE, BOARD_SIZE];
        stoneManager = new StoneManager ();

        //碁石のプレハブを生成済みかどうかの判定フラグを取得
        if (goban.childCount > 0) isCreated = true;
        if (agentId == 0) {
            if (isCreated) {
                goban.DestroyChildObject ();
            } else {
                //先攻Agentをランダムに決める
                //this.first = Random.Range(0, 2);

                //GoAgent0を先番に固定
                this.first = 0;
                Debug.Log ("先攻AgentId -> " + first);
            }
        }

        //碁石のプレハブを生成
        for (int x = 1; x < BOARD_SIZE - 1; x++) {
            for (int y = 1; y < BOARD_SIZE - 1; y++) {
                //インデックスを計算
                int index = GoUtil.XYToTapIndex (x, y);
                //Debug.Log("Index: " + index + "  XY: " + x + "-" + y);

                //碁盤配列を初期化
                points[x, y] = (int) StoneManager.State.NONE;
                if (agentId == 0) {
                    //碁石オブジェクトを碁盤の子に設定
                    GameObject obj = SetChild (goban.gameObject, stonePrefab).gameObject;
                    //碁石オブジェクトをアクティブにする
                    obj.SetActive (true);
                    //碁石オブジェクト名をインデックスに設定
                    obj.name = index.ToString ();
                    //スクリプトを取得してリストに追加
                    stoneManager = obj.GetComponent<StoneManager> ();
                    stoneList.Add (stoneManager);
                    //ボタンの名前をインデックスにする
                    stoneList[index].confirmButton.gameObject.name = index.ToString ();

                    //着手ボタンのクリックイベントを設定
                    stoneList[index].confirmButton.onClick.AddListener (() => this.DecidePlayerAction (1, index));
                    //着手ボタンを有効化
                    stoneList[index].confirmButton.gameObject.SetActive (true);
                    //着手ボタンを無効化
                    //stoneList[index].confirmButton.gameObject.SetActive (false);

                    //碁石オブジェクトを初期化
                    stoneList[index].InitStone (index, (int) StoneManager.State.NONE);
                }
            }
        }

        //盤外を設定
        for (int k = 0; k < BOARD_SIZE; k++) {
            points[k, 0] = (int) StoneManager.State.WALL;
            points[k, BOARD_SIZE - 1] = (int) StoneManager.State.WALL;
        }

        for (int l = 0; l < BOARD_SIZE; l++) {
            points[0, l] = (int) StoneManager.State.WALL;
            points[BOARD_SIZE - 1, l] = (int) StoneManager.State.WALL;
        }

        if (agentId == 0 && !isCreated) {
            gameObject.transform.Find ("stonePrefab").gameObject.SetActive (false);
        }
    }

    //アクションの適用
    public void AreaAction (int agentId, int action, bool isPlayer) {
        Debug.Log ("agentID: " + agentId);
        Debug.Log ("Action: " + action);
        Debug.Log ("GetTurn(): " + GetTurn());
        //Agentのターンでない時は無処理
        if (GetTurn () != agentId) return;

        Debug.Log("手番判定通った！");

        if (action == 81) {
            //パス
            PassMove (agentId);
            //現在の手番を更新
            currentTurn = ChangeTurn (agentId);
        } else {
            //1次元配列形式のインデックスを2次元配列形式に変換
            int[] indexArray = GoUtil.TapIndexToXY (action);
            int x = indexArray[0];
            int y = indexArray[1];
            Debug.LogWarning((GetTurnCount() + 1) + "手目" + "  Action: " + action + "  Index: " + x + "-" + y);

            if (points[x, y] != (int) StoneManager.State.NONE) {
                inAction = false;
                return;
            }

            Debug.Log("空点判定通った！");

            //碁石の色を取得
            int color = -1;
            if (moveList.Count % 2 == 0) {
                color = 0;
            } else {
                color = 1;
            }

            if (action != -1 && stoneList[action].CheckLegal (color, x, y, points)) {
                Debug.Log("CheckLegal通った！");
                int mlCount = moveList.Count;
                if (mlCount == 0 || (mlCount >= 1 && !moveList[mlCount - 1].isKo) ||
                    (moveList[mlCount - 1].isKo && moveList[mlCount - 1].koIndex != action)) {
                    Debug.Log("コウ判定通った！");

                    //碁石を配置
                    points = stoneList[action].PutStone (points, action, color, moveList.Count + 1);

                    //取れる石があったら盤上から取り除く
                    if (moveList.Count > 0 && moveList[moveList.Count - 1].prisonerList.Count > 0) {
                        foreach (int prisoner in moveList[moveList.Count - 1].prisonerList) {
                            //Debug.Log("取石Index -> " + prisoner);
                            points = stoneList[prisoner].SetPrisoner (points, prisoner, (int) StoneManager.State.NONE);
                        }

                        //取石リストをリセット
                        stoneList[action].ClearPrisonerList ();
                    }

                    //現在の手番を更新
                    currentTurn = ChangeTurn (agentId);

                    passCount = 0;
                    inAction = false;
                } else {
                    Debug.LogWarning ("コウのルール違反！");
                    points[x, y] = (int) StoneManager.State.NONE;
                    this.agent[agentId].AddReward (-0.05f);
                    inAction = false;
                }
            }
            //碁石の配置失敗
            else {
                if (isPlayer) {
                    inAction = false;
                    return;
                }

                points[x, y] = (int) StoneManager.State.NONE;

                bool isExist = false;
                for (int i = 1; i < BOARD_SIZE - 1; i++) {
                    for (int j = 1; j < BOARD_SIZE - 1; j++) {
                        if (points[i, j] == (int) StoneManager.State.NONE) {
                            Debug.LogError ("空点: " + i + "-" + j);
                            if (stoneList[action].CheckLegalIsExist (points, i, j, color)) {
                                Debug.LogError ("打てる場所があります！");
                                isFinish = false;
                                this.agent[agentId].AddReward (-0.05f);
                                isExist = true;
                            } else {
                                isFinish = true;
                            }

                            //Debug.LogWarning("LegalIndex: " + index);
                            //Debug.LogWarning("LegalIndex XY: " + i + "-" + j);
                        }

                        if (isExist) break;
                    }

                    if (isExist) break;
                }
            }
        }

        //デバッグ用
        //string debugStr = "";
        //for (int di = 1; di < BOARD_SIZE - 1; di++)
        //{
        //    for (int dj = 1; dj < BOARD_SIZE - 1; dj++)
        //    {
        //        debugStr += points[di, dj].ToString();
        //    }
        //}

        //Debug.LogError("デバッグ用出力 points:\n" + debugStr);

        if (GetTurnCount () >= MAX_MOVE_COUNT || isFinish) {
            if (agentId == 0) {
                //対局結果をサーバーのgnugoで判定する
                StartCoroutine (DetermineResult ());
            }
        } else {
            inAction = false;
        }
    }

    private IEnumerator DetermineResult () {
        float timeOutSec = 30.0f;
        //先攻のagent番号を求める
        int black = first == 0 ? 0 : 1;
        int white = black == 0 ? 1 : 0;

        GoUtil.SaveGamestate (moveList, BOARD_SIZE - 2, BOARD_SIZE - 2, 7, GoEstimator.result,
            agent[black].names[black], agent[white].names[white]);

        //サーバーのgnugoを起動して勝敗判定を行う
        GoEstimator est = GameObject.Find ("GoEstimator").GetComponent<GoEstimator> ();
        est.GetResult ();

        while (true) {
            //対局結果が取得できたら画面に結果を表示する
            if (GoEstimator.result.Length > 1) {
                string finalScore = GoUtil.FormatFinalScore (GoEstimator.result);

                //黒勝ち
                if (finalScore.Contains ("黒")) {
                    Debug.Log ("黒勝ち");
                    this.agent[black].AddReward (1.0f);
                    this.agent[white].AddReward (-1.0f);
                    this.agent[black].winCount++;
                    winCountObj[black].text = this.agent[black].winCount.ToString ();
                }
                //白勝ち
                else if (finalScore.Contains ("白")) {
                    Debug.Log ("白勝ち");
                    this.agent[black].AddReward (-1.0f);
                    this.agent[white].AddReward (1.0f);
                    this.agent[white].winCount++;
                    winCountObj[white].text = this.agent[white].winCount.ToString ();
                }
                //引き分け
                else if (finalScore.Contains ("引き分け")) {
                    Debug.Log ("引き分け");
                    this.agent[black].AddReward (0.5f);
                    this.agent[white].AddReward (0.5f);
                } else {
                    Debug.Log ("想定外の対局結果です。");
                }

                Debug.Log (finalScore);

                //対局結果を含めてSGFを保存
                GoUtil.SaveSGF (GoEstimator.result, agent[black].names[black], agent[white].names[white]);
                //Debug.Log("SGFで保存しました！");
                this.agent[0].Done ();
                this.agent[1].Done ();

                yield break;
            } else {
                if (timeOutSec <= 0f) yield break;

                yield return new WaitForSeconds (0.5f);
                timeOutSec -= 0.5f;
            }
        }
    }

    public void DecidePlayerAction (int agentId, int action) {
        this.agent[agentId].SetSelectAction (action);
    }

    public int[, ] GetPreviousPoints (int[, ] board2, int k) {
        int[, ] prevBoard = new int[BOARD_SIZE, BOARD_SIZE];
        if (GetTurnCount () >= k) {
            prevBoard = GetPoints ();
            List<Move> tempMoveList = moveList;

            //1手目から(最終手 - k)までを再現する
            for (int j = 0; j < tempMoveList.Count - k; j++) {
                int putIndex = tempMoveList[j].putIndex;
                int color = j % 2 == 1 ? (int) StoneManager.State.BLACK : (int) StoneManager.State.WHITE;
                //i手前に戻す
                prevBoard = stoneList[putIndex].InitPrevStone (prevBoard, putIndex, color);

                //k手前に取石があるなら再配置する
                List<int> tempPrisonerList = tempMoveList[j].prisonerList;
                if (tempPrisonerList.Count > 0) {
                    //取石を1つずつ処理する
                    foreach (int prisoner in tempPrisonerList) {
                        prevBoard = stoneList[prisoner].SetPrevPrisoner (prevBoard, prisoner, (int) StoneManager.State.NONE);
                    }
                }
            }
        } else {
            prevBoard = InitBoard ();
        }

        return prevBoard;
    }

    //パス
    public void PassMove (int agentId) {
        Move mv = new Move (agentId, -2, GetTurnCount () + 1, new List<int> (), false, -1);
        if (passCount == 0) {
            //1回目のパス
            passCount = 1;
            moveList.Add (mv);
        } else if (passCount == 1) {
            //2回連続パス(終局)
            passCount = 2;
            moveList.Add (mv);
            isFinish = true;
        }
    }

    public int[, ] InitBoard () {
        int[, ] rtnBoard = new int[BOARD_SIZE, BOARD_SIZE];
        int x, y;
        for (x = 1; x < (BOARD_SIZE - 1); x++) {
            for (y = 1; y < (BOARD_SIZE - 1); y++) {
                rtnBoard[x, y] = (int) StoneManager.State.NONE;
            }
        }

        int i, j;
        for (i = 0; i < BOARD_SIZE; i++) {
            rtnBoard[i, 0] = (int) StoneManager.State.WALL;
            rtnBoard[i, BOARD_SIZE - 1] = (int) StoneManager.State.WALL;
        }

        for (j = 0; j < BOARD_SIZE; j++) {
            rtnBoard[0, j] = (int) StoneManager.State.WALL;
            rtnBoard[BOARD_SIZE - 1, j] = (int) StoneManager.State.WALL;
        }

        return rtnBoard;
    }

    //手番を反転させる
    public int ChangeTurn (int turn) {
        return turn == 0 ? 1 : 0;
    }

    //セル群の取得
    public int[, ] GetPoints () {
        return points;
    }

    //ターン数の取得
    public int GetTurnCount () {
        return moveList.Count;
    }

    //ターンの取得
    public int GetTurn () {
        if (agent[0].IsDone () ||
            agent[1].IsDone () ||
            agent[0].GetStepCount () == 0 ||
            agent[1].GetStepCount () == 0) return -1;
        return (GetTurnCount () + first) % 2;
    }

    //先攻プレイヤーのagentIdを取得
    public int GetFirstAgentId () {
        return first;
    }

    //ボタンのenabledを設定
    private void SetButtonEnabled (bool isEnabled) {
        for (int i = 0; i < (BOARD_SIZE - 2) * (BOARD_SIZE - 2); i++) {
            stoneList[i].confirmButton.gameObject.SetActive (isEnabled);
        }
    }

    private int ReverseAgentId (int agentId) {
        return agentId == 0 ? 1 : 0;
    }

    public Transform SetChild (GameObject parent, GameObject child, string name = null) {
        //プレハブからインスタンスを生成
        GameObject obj = Instantiate (child);

        //作成したオブジェクトを子として登録
        obj.transform.SetParent (parent.transform);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        //作成したオブジェクトの名前に(Clone)がつかないようにプレハブの名前を再付与
        obj.name = (name != null) ? name : child.name;

        return obj.transform;
    }
}