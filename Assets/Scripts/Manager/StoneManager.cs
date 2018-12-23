using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoneManager : MonoBehaviour
{

    public enum State
    {
        PASS = -2,
        NONE = -1,
        BLACK = 0,
        WHITE = 1,
        WALL = 5
    }

    const int BOARD_SIZE = 11;     //碁盤サイズ

    public Image stoneImg;
    public Sprite[] stoneSprites = new Sprite[3];
    public Button confirmButton;
    public Text stoneNum;

    int[,] board = new int[BOARD_SIZE, BOARD_SIZE];    //碁盤配列

    int move = 0;     //手数
    int ko_x;         //コウの位置X
    int ko_y;         //コウの位置Y
    int ko_num;       //劫が発生した手数
    bool isKo;
    int koIndex;
    
    bool[,] checkBoard = new bool[BOARD_SIZE, BOARD_SIZE];  //合法手かどうか調べるのに使う

    List<int> prisonerList = new List<int>();     //TsumegoManagerクラスに返す取石のリスト

    //碁石を描画するメソッド(記録あり、戻り値あり)
    public int[,] PutStone(int[,] board2, int putIndex, int color, int moveCount)
    {
        //引数の碁盤配列をこのクラスの碁盤配列にコピー
        board = board2;

        int[] indexArray = GoUtil.TapIndexToXY(putIndex);
        int x = indexArray[0];
        int y = indexArray[1];

        //Debug.Log("XY座標: " + x + "  " + y);
        //Debug.Log("board[x, y]: " + board[x, y]);

        if (color == (int)State.NONE)
        {
            //石なしの設定
            board[x, y] = (int)State.NONE;
            stoneImg.sprite = null;
            stoneImg.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(true);
            //stoneNum.text = "";
        }
        else if (color == (int)State.BLACK)
        {
            //黒石の設定
            board[x, y] = (int)State.BLACK;
            stoneImg.gameObject.SetActive(true);
            stoneImg.sprite = stoneSprites[1];
            confirmButton.gameObject.SetActive(false);
            //stoneNum.text = moveCount.ToString();
            //stoneNum.color = Color.white;
        }
        else if (color == (int)State.WHITE)
        {
            //白石の設定
            board[x, y] = (int)State.WHITE;
            stoneImg.gameObject.SetActive(true);
            stoneImg.sprite = stoneSprites[2];
            confirmButton.gameObject.SetActive(false);
            //stoneNum.text = moveCount.ToString();
            //stoneNum.color = Color.black;
        }

        //取れる石があれば取石リストに追加
        CheckPrisoner(color, x, y, board);
        //Debug.Log("取石リスト要素数: " + prisonerList.Count);

        //コウが発生したインデックスを保存
        if (isKo)
        {
            koIndex = GoUtil.XYToTapIndex(ko_x, ko_y);
            //Debug.Log("KoIndex: " + koIndex + "  XY: " + (ko_x  - 1)+ "-" + (ko_y - 1));
        }
        else
        {
            koIndex = -1;
        }

        //着手を記録
        RecordMove(color, putIndex, moveCount, prisonerList, isKo, koIndex);

        //現在の盤面をログ出力
        //GoUtil.PrintBoard(board, BOARD_SIZE);

        return board;
    }

    //碁石を描画するメソッド(記録なし、戻り値なし)
    public void InitStone(int index, int color)
    {
        int[] indexArray = GoUtil.TapIndexToXY(index);
        int x = indexArray[0];
        int y = indexArray[1];

        if (color == (int)State.NONE)
        {
            //石なしの設定
            board[x, y] = (int)State.NONE;
            stoneImg.sprite = null;
            stoneImg.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(true);
            //stoneNum.text = "";
        }
        else if (color == (int)State.BLACK)
        {
            //黒石の設定
            board[x, y] = (int)State.BLACK;
            stoneImg.gameObject.SetActive(true);
            stoneImg.sprite = stoneSprites[1];
            confirmButton.gameObject.SetActive(false);
            //stoneNum.text = "";
            //stoneNum.color = Color.white;
        }
        else if (color == (int)State.WHITE)
        {
            //白石の設定
            board[x, y] = (int)State.WHITE;
            stoneImg.gameObject.SetActive(true);
            stoneImg.sprite = stoneSprites[2];
            confirmButton.gameObject.SetActive(false);
            //stoneNum.text = "";
            //stoneNum.color = Color.black;
        }
    }
    
    //碁石を描画するメソッド(記録なし、戻り値あり)
    public int[,] InitStone(int[,] board2, int index, int color)
    {
        int[] indexArray = GoUtil.TapIndexToXY(index);
        int x = indexArray[0];
        int y = indexArray[1];

        if (color == (int)State.NONE)
        {
            //石なしの設定
            board2[x, y] = (int)State.NONE;
            stoneImg.sprite = null;
            stoneImg.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(true);
            //stoneNum.text = "";
        }
        else if (color == (int)State.BLACK)
        {
            //黒石の設定
            board2[x, y] = (int)State.BLACK;
            stoneImg.gameObject.SetActive(true);
            stoneImg.sprite = stoneSprites[1];
            confirmButton.gameObject.SetActive(false);
            //stoneNum.text = "";
            //stoneNum.color = Color.white;
        }
        else if (color == (int)State.WHITE)
        {
            //白石の設定
            board2[x, y] = (int)State.WHITE;
            stoneImg.gameObject.SetActive(true);
            stoneImg.sprite = stoneSprites[2];
            confirmButton.gameObject.SetActive(false);
            //stoneNum.text = "";
            //stoneNum.color = Color.black;
        }

        return board2;
    }

    //取石を盤上から取り除くメソッド
    public int[,] SetPrisoner(int[,] board2, int prisoner, int color)
    {
        //Debug.Log("prisoner: " + prisoner);

        //引数の碁盤配列をこのクラスの碁盤配列にコピー
        board = board2;

        int[] indexArray = GoUtil.TapIndexToXY(prisoner);
        int priX = indexArray[0];
        int priY = indexArray[1];

        //Debug.LogWarning("取石座標 -> " + indexArray[0] + "-" + indexArray[1] + "  board[priX, priY] -> " + board[priX, priY]);

        //石なしの設定
        if (color == (int)State.NONE)
        {
            board[priX, priY] = (int)State.NONE;
            stoneImg.sprite = null;
            stoneImg.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(true);
            //stoneNum.text = "";
        }
        else if (color == (int)State.BLACK)
        {
            board[priX, priY] = (int)State.WHITE;
            stoneImg.gameObject.SetActive(true);
            stoneImg.sprite = stoneSprites[1];
            confirmButton.gameObject.SetActive(false);
        }
        else if (color == (int)State.WHITE)
        {
            board[priX, priY] = (int)State.BLACK;
            stoneImg.gameObject.SetActive(true);
            stoneImg.sprite = stoneSprites[2];
            confirmButton.gameObject.SetActive(false);
        }

        //現在の盤面をログ出力
        //GoUtil.PrintBoard(board, BOARD_SIZE);

        return board;
    }

    //碁石を描画するメソッド(記録なし、戻り値あり)
    public int[,] InitPrevStone(int[,] board2, int index, int color)
    {
        int[] indexArray = GoUtil.TapIndexToXY(index);
        int x = indexArray[0];
        int y = indexArray[1];

        if (color == (int)State.NONE)
        {
            //石なしの設定
            board2[x, y] = (int)State.NONE;
        }
        else if (color == (int)State.BLACK)
        {
            //黒石の設定
            board2[x, y] = (int)State.BLACK;
        }
        else if (color == (int)State.WHITE)
        {
            //白石の設定
            board2[x, y] = (int)State.WHITE;
        }

        return board2;
    }

    //取石を制御するメソッド
    public int[,] SetPrevPrisoner(int[,] board2, int prisoner, int color)
    {
        int[] indexArray = GoUtil.TapIndexToXY(prisoner);
        int priX = indexArray[0];
        int priY = indexArray[1];

        //石なしの設定
        if (color == (int)State.NONE)
        {
            board2[priX, priY] = (int)State.NONE;
        }
        else if (color == (int)State.BLACK)
        {
            board2[priX, priY] = (int)State.WHITE;
        }
        else if (color == (int)State.WHITE)
        {
            board2[priX, priY] = (int)State.BLACK;
        }

        return board;
    }

    public void ClearPrisonerList()
    {
        prisonerList.Clear();
    }

    public void ClearStoneNum()
    {
        stoneNum.text = "";
    }

    public void RecordMove(int color, int index, int moveCount, List<int> prisonerList, bool isKo, int koIndex)
    {
        GoArea.moveList.Add(new Move(color, index, moveCount, prisonerList, isKo, koIndex));
    }

    public bool CheckLegalIsExist(int[,] board2, int i, int j, int color)
    {
        bool result;
        result = CheckLegal(color, i, j, board2);
       
        return result;
    }

    public void RemoveAtMove(int moveCount)
    {
        GoArea.moveList.RemoveAt(moveCount);
    }

    public int[,] InitBoard()
    {
        int[,] rtnBoard = new int[BOARD_SIZE, BOARD_SIZE];
        int x, y;
        for (x = 1; x < (BOARD_SIZE - 1); x++)
        {
            for (y = 1; y < (BOARD_SIZE - 1); y++)
            {
                rtnBoard[x, y] = (int)State.NONE;
            }
        }

        int i, j;
        for (i = 0; i < BOARD_SIZE; i++)
        {
            rtnBoard[i, 0] = (int)State.WALL;
            rtnBoard[i, BOARD_SIZE - 1] = (int)State.WALL;
        }
        for (j = 0; j < BOARD_SIZE; j++)
        {
            rtnBoard[0, j] = (int)State.WALL;
            rtnBoard[BOARD_SIZE - 1, j] = (int)State.WALL;
        }

        return rtnBoard;
    }
    
    public bool CheckLegal(int color, int x, int y, int[,] board2)
    {
        //引数の碁盤配列をこのクラスの碁盤配列にコピー
        board = board2;

        //空点じゃないと置けません
        if (board[x, y] != (int)State.NONE)
        {
            //Debug.LogError("空点じゃないよ！");
            //Debug.LogWarning("X -> " + x + "  Y -> " + y + "  Color: " + color);
            return false;
        }

        /* 一手前に劫を取られていたら置けません */
        if (GoArea.moveList.Count > 0)
        {
            Move prevMoveData = GoArea.moveList[GoArea.moveList.Count - 1];
            if (prevMoveData.koIndex != -1)
            {
                int index = GoUtil.XYToTapIndex(x, y);
                if (index == prevMoveData.koIndex)
                {
                    //Debug.LogError("コウだよ！");
                    //Debug.LogWarning((x - 1) + "-" + (y - 1) + "  Color: " + color);
                    return false;
                }
            }
        }

        /* 自殺手なら置けません */
        if (CheckSuicide(color, x, y))
        {
            //Debug.LogError("自殺手だよ！");
            //Debug.LogWarning((x - 1) + "-" + (y - 1) + "  Color: " + color);
            return false;
        }
       
        board[x, y] = (int)State.NONE;

        /* 以上のチェックをすべてクリアできたので置けます */
        return true;
    }

    public bool CheckSuicide(int color, int x, int y)
    {
        bool rtnVal;
        int opponent;  //相手の色

        /* 仮に石を置く */
        board[x, y] = color;

        /* マークのクリア */
        ClearCheckBoard();

        /* その石は相手に囲まれているか調べる */
        rtnVal = DoCheckRemoveStone(color, x, y);

        /* 囲まれているならば自殺手の可能性あり */
        if (rtnVal == true)
        {
            /* 相手の色を求める */
            opponent = ReverseColor(color);

            /* その石を置いたことにより、隣の相手の石が取れるなら自殺手ではない */
            if (y > 1)
            {
                /* 隣は相手？ */
                if (board[x, y - 1] == opponent)
                {
                    /* マークのクリア */
                    ClearCheckBoard();
                    /* 相手の石は囲まれているか？ */
                    rtnVal = DoCheckRemoveStone(opponent, x, y - 1);
                    /* 相手の石を取れるので自殺手ではない */
                    if (rtnVal == true)
                    {
                        //石なしの設定
                        board[x, y] = (int)State.NONE;
                        return false;
                    }
                }
            }
            if (x > 1)
            {
                /* 隣は相手？ */
                if (board[x - 1, y] == opponent)
                {
                    /* マークのクリア */
                    ClearCheckBoard();
                    /* 相手の石は囲まれているか？ */
                    rtnVal = DoCheckRemoveStone(opponent, x - 1, y);
                    /* 相手の石を取れるので自殺手ではない */
                    if (rtnVal == true)
                    {
                        //石なしの設定
                        board[x, y] = (int)State.NONE;
                        return false;
                    }
                }
            }
            if (y < (BOARD_SIZE - 2))
            {
                /* 隣は相手？ */
                if (board[x, y + 1] == opponent)
                {
                    /* マークのクリア */
                    ClearCheckBoard();
                    /* 相手の石は囲まれているか？ */
                    rtnVal = DoCheckRemoveStone(opponent, x, y + 1);
                    /* 相手の石を取れるので自殺手ではない */
                    if (rtnVal == true)
                    {
                        //石なしの設定
                        board[x, y] = (int)State.NONE;
                        return false;
                    }
                }
            }
            if (x < (BOARD_SIZE - 2))
            {
                /* 隣は相手？ */
                if (board[x + 1, y] == opponent)
                {
                    /* マークのクリア */
                    ClearCheckBoard();
                    /* 相手の石は囲まれているか？ */
                    rtnVal = DoCheckRemoveStone(opponent, x + 1, y);
                    /* 相手の石を取れるので自殺手ではない */
                    if (rtnVal == true)
                    {
                        //石なしの設定
                        board[x, y] = (int)State.NONE;
                        return false;
                    }
                }
            }

            //石なしの設定
            board[x, y] = (int)State.NONE;

            /* 相手の石を取れないなら自殺手 */
            return true;
        }
        else
        {
            //石なしの設定
            board[x, y] = (int)State.NONE;

            /* 囲まれていないので自殺手ではない */
            return false;
        }
    }

    public void ClearCheckBoard()
    {
        int x, y;

        for (x = 1; x < (BOARD_SIZE - 1); x++)
        {
            for (y = 1; y < (BOARD_SIZE - 1); y++)
            {
                checkBoard[x, y] = false;
            }
        }
    }

    public bool DoCheckRemoveStone(int color, int x, int y)
    {
        bool rtn;

        /* その場所は既に調べた点ならおしまい */
        if (checkBoard[x, y] == true)
        {
            //Debug.LogWarning("既に探索済みです！");
            return true;
        }

        /* 調べたことをマークする */
        checkBoard[x, y] = true;

        /* 何も置かれていないならばおしまい */
        if (board[x, y] == (int)State.NONE)
        {
            //Debug.LogError("空点！ DoCheckRemoveStone");
            //Debug.LogWarning("X2: " + x + "  Y2: " + y + "  board[x, y]: " + board[x, y] + "  Index: " + GoUtil.XYToTapIndex(x, y));
            return false;
        }

        /* 同じ色の石ならばその石の隣も調べる */
        if (board[x, y] == color)
        {
            /* その石の左(x-1,y)を調べる */
            if (y > 1)
            {
                rtn = DoCheckRemoveStone(color, x, y - 1);
                if (rtn == false)
                {
                    return false;
                }
                //Debug.LogWarning("探索XL: " + x + " 探索YL: " + (y - 1) + " 色: " + color);
            }
            /* その石の上(x,y-1)を調べる */
            if (x > 1)
            {
                rtn = DoCheckRemoveStone(color, x - 1, y);
                if (rtn == false)
                {
                    return false;
                }
                //Debug.LogWarning("探索XT: " + (x - 1) + " 探索YT: " + y + " 色: " + color);
            }
            /* その石の右(x+1,y)を調べる */
            if (y < (BOARD_SIZE - 2))
            {
                rtn = DoCheckRemoveStone(color, x, y + 1);
                if (rtn == false)
                {
                    return false;
                }
                //Debug.LogWarning("探索XR: " + x + " 探索YR: " + (y + 1) + " 色: " + color);
            }
            /* その石の下(x,y+1)を調べる */
            if (x < (BOARD_SIZE - 2))
            {
                rtn = DoCheckRemoveStone(color, x + 1, y);
                if (rtn == false)
                {
                    return false;
                }
                //Debug.LogWarning("探索XU: " + (x + 1) + " 探索YU: " + y + " 色: " + color);
            }
        }

        /* 相手の色の石があった */
        return true;
    }

    public int RemoveStone(int color, int x, int y)
    {
        int prisoner;  /* 取り除かれた石数 */

        /* 置いた石と同じ色なら取らない */
        if (board[x, y] == color)
        {
            //Debug.Log("置いた石と同じ色！ RemoveStone");
            //Debug.Log("board[x, y]: " + board[x, y] + "X: " + x + " Y: " + y);
            return 0;
        }

        /* 空点なら取らない */
        if (board[x, y] == (int)State.NONE)
        {
            //Debug.Log("空点！ RemoveStone");
            //Debug.Log("board[x, y]: " + board[x, y] + "X: " + x + " Y: " + y);
            return 0;
        }

        /* マークのクリア */
        ClearCheckBoard();

        /* 囲まれているなら取る */
        if (DoCheckRemoveStone(board[x, y], x, y))
        {
            prisoner = DoRemoveStone(board[x, y], x, y, 0);
            return prisoner;
        }

        return 0;
    }

    public int DoRemoveStone(int color, int x, int y, int prisoner)
    {
        /* 取り除かれる石と同じ色ならば石を取る */
        if (board[x, y] == color)
        {
            //Debug.LogWarning("取れたよ！わーい！  XY -> " + x + "-" + y + "  Index -> " + GoUtil.XYToTapIndex(x, y));
            /* 取った石の数を１つ増やす */
            prisoner++;
            //取れる石のindexのList
            prisonerList.Add(GoUtil.XYToTapIndex(x, y));

            /* その座標に空点を置く */
            board[x, y] = (int)State.NONE;

            /* 左を調べる */
            if (y > 1)
            {
                prisoner = DoRemoveStone(color, x, y - 1, prisoner);
            }
            /* 上を調べる */
            if (x > 1)
            {
                prisoner = DoRemoveStone(color, x - 1, y, prisoner);
            }
            /* 右を調べる */
            if (y < (BOARD_SIZE - 2))
            {
                prisoner = DoRemoveStone(color, x, y + 1, prisoner);
            }
            /* 下を調べる */
            if (x < (BOARD_SIZE - 2))
            {
                prisoner = DoRemoveStone(color, x + 1, y, prisoner);
            }
        }
        /* 取った石の数を返す */
        return prisoner;
    }

    public int CheckPrisoner(int color, int x, int y, int[,] board2)
    {
        int prisonerN;    /* 取り除かれた石の数（上） */
        int prisonerE;    /* 取り除かれた石の数（右） */
        int prisonerS;    /* 取り除かれた石の数（下） */
        int prisonerW;    /* 取り除かれた石の数（左） */
        int prisonerAll;  /* 取り除かれた石の総数 */
        //bool koFlag;       /* 劫かどうか */

        /* 座標(x,y)に石を置く */
        board[x, y] = color;

        /* 置いた石の隣に同じ色の石はあるか？ */

//        Debug.LogWarning("XY座標:  " + x + "-" + y);
//        Debug.LogError("上: " + board[x + 1, y]);
//        Debug.LogError("下: " + board[x - 1, y]);
//        Debug.LogError("右: " + board[x, y + 1]);
//        Debug.LogError("左: " + board[x, y - 1]);

        if (board[x + 1, y] != color &&
            board[x - 1, y] != color &&
            board[x, y + 1] != color &&
            board[x, y - 1] != color)
        {
            /* 同じ色の石がないならば劫かもしれない */
            isKo = true;
        }
        else
        {
            /* 同じ色の石があるならば劫ではない */
            isKo = false;
        }

        /* 取り除かれた石の数 */
        prisonerN = 0;
        prisonerE = 0;
        prisonerS = 0;
        prisonerW = 0;

        /* 置いた石の周囲の相手の石が死んでいれば碁盤から取り除く */
        if (y > 1)
        {
            prisonerN = RemoveStone(color, x, y - 1);
        }
        if (x > 1)
        {
            prisonerW = RemoveStone(color, x - 1, y);
        }
        if (y < (BOARD_SIZE - 2))
        {
            prisonerS = RemoveStone(color, x, y + 1);
        }
        if (x < (BOARD_SIZE - 2))
        {
            prisonerE = RemoveStone(color, x + 1, y);
        }

        /* 取り除かれた石の総数 */
        prisonerAll = prisonerN + prisonerE + prisonerS + prisonerW;

        /* 置いた石の隣に同じ色の石がなく、取り除かれた石も１つならば劫 */
        if (isKo && prisonerAll == 1)
        {
            //Debug.LogWarning("コウだよ！");

            /* 劫の発生した手数を覚える */
            ko_num = GoArea.moveList.Count + 1;
            /* 劫の座標を覚える */
            if (prisonerE == 1)
            {
                /* 取り除かれた石が右 */
                ko_x = x + 1;
                ko_y = y;
            }
            else if (prisonerS == 1)
            {
                /* 取り除かれた石が下 */
                ko_x = x;
                ko_y = y + 1;
            }
            else if (prisonerW == 1)
            {
                /* 取り除かれた石が左 */
                ko_x = x - 1;
                ko_y = y;
            }
            else if (prisonerN == 1)
            {
                /* 取り除かれた石が上 */
                ko_x = x;
                ko_y = y - 1;
            }
        }
        else
        {
            isKo = false;
        }

        //Debug.LogWarning("取石総数: " + prisonerAll);

        return prisonerAll;
    }

    public int ReverseColor(int color)
    {
        if (color == (int)State.BLACK)
        {
            return (int)State.WHITE;
        }
        else if (color == (int)State.WHITE)
        {
            return (int)State.BLACK;
        }

        return (int)State.NONE;
    }
}
