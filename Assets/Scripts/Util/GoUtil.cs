using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GoUtil : MonoBehaviour {
    const string LETTERS = "@abcdefghijklmnopqrstuvxyz";
    public const string SGF_GAME_COUNT = "SGF_GAME_COUNT";

    public static string fileName = "";
    public static string sgfData = "";
    public static float diffNum = 0.0f;
    public static bool isFloat;

    public static bool SaveGamestate (List<Move> moveList, int yokoSize, int tateSize, double komi = 7,
        string result = "",
        string blackPlayerName = "Black", string whitePlayerName = "White") {
        List<string> strList = new List<string> ();

        //対局設定
        strList.Add ("(;GM[1]FF[4]CA[UTF-8]");

        if (yokoSize == tateSize) {
            strList.Add ("SZ[" + yokoSize + "]");
        } else {
            strList.Add ("SZ[" + yokoSize + ":" + tateSize + "]");
        }

        strList.Add ("KM[" + komi + "]");
        strList.Add ("PB[" + blackPlayerName + "]");
        strList.Add ("PW[" + whitePlayerName + "]");

        if (result != "") strList.Add ("RE[" + result + "]");
        string cycle_string = "BW";

        //着手リスト
        for (int i = 0; i < moveList.Count; i++) {
            //石の色を判定して文字列リストに追加
            int color = i % 2;
            if (color == (int) StoneManager.State.BLACK) {
                //黒
                strList.Add (";" + cycle_string[0]);
            } else if (color == (int) StoneManager.State.WHITE) {
                //白
                strList.Add (";" + cycle_string[1]);
            }

            //着手情報を文字列リストに追加
            if (moveList[i].putIndex < 0) {
                strList.Add ("[tt]");
            } else {
                int[] twoindex = TapIndexToXY (moveList[i].putIndex);
                int x = twoindex[1];
                int y = twoindex[0];
                strList.Add ("[" + LETTERS[x] + "" + LETTERS[y] + "]");
            }
        }

        strList.Add (")");

        //goguiでの勝敗判定用にSGFデータをstaticで保持しておく
        sgfData = string.Join ("", strList);

        return true;
    }

    public static string SaveSGF (string result, string blackPlayerName = "Black", string whitePlayerName = "White") {
        int gameCount = 0;
        if (PlayerPrefs.HasKey (SGF_GAME_COUNT)) {
            gameCount = PlayerPrefs.GetInt (SGF_GAME_COUNT, 0);
        }

        //対局結果を追記する
        string resultStr = "RE[" + result + "]";
        string markStr = "CA[UTF-8]";
        int index = sgfData.IndexOf (markStr) + markStr.Length;
        string sgf = sgfData.Insert (index, resultStr);

        //文字列をSGFファイルとして書き出す
        string fileName = blackPlayerName + " vs " + whitePlayerName + "-" + gameCount + ".sgf";
        string dirPath = Path.Combine (Application.dataPath, "SGF");
        DirectoryUtils.SafeCreateDirectory (dirPath);
        //string filePath = "C:\\Users\\inten\\Desktop\\ML-Agent\\SGF\\" + fileName;
        string filePath = Path.Combine (dirPath, fileName);
        StreamWriter sw = new StreamWriter (filePath, false, Encoding.GetEncoding ("utf-8")); //true=追記 false=上書き
        sw.WriteLine (sgf);
        sw.Flush ();
        sw.Close ();

        sgfData = "";

        //ゲーム数を1加算して保存
        PlayerPrefs.SetInt (SGF_GAME_COUNT, gameCount + 1);
        PlayerPrefs.Save ();
        //Debug.Log("SGFファイル保存先 -> " + filePath);

        return filePath;
    }

    public static string SaveResult (string result) {
        int gameCount = 0;
        if (PlayerPrefs.HasKey (SGF_GAME_COUNT)) {
            gameCount = PlayerPrefs.GetInt (SGF_GAME_COUNT, 0);
        }

        //結果だけをtxtファイルとして書き出す
        fileName = "game_result " + gameCount + ".txt";
        string dirPath = Path.Combine (Application.dataPath, "SGF");
        DirectoryUtils.SafeCreateDirectory (dirPath);
        //string filePath = "C:\\Users\\inten\\Desktop\\ML-Agent\\SGF\\" + fileName;
        string filePath = Path.Combine (dirPath, fileName);
        StreamWriter sw = new StreamWriter (filePath, false, Encoding.GetEncoding ("utf-8")); //true=追記 false=上書き
        sw.WriteLine (result);
        sw.Flush ();
        sw.Close ();

        return filePath;
    }

    public static int[] TapIndexToXY (int pushIndex) {
        int boardSize = GoArea.BOARD_SIZE - 2;
        int[] rtnArray = new int[2];
        int x, y = 0;

        if ((pushIndex + 1) % boardSize == 0) {
            x = boardSize;
            if (pushIndex == 0) {
                y = 1;
            } else {
                y = (pushIndex + 1) / boardSize;
            }

            rtnArray[0] = y;
            rtnArray[1] = x;
        } else if ((pushIndex + 1) % boardSize > 0) {
            //x = (pushIndex + 1) % (GoArea.BOARD_SIZE - 2) - 1;
            x = (pushIndex + 1) % boardSize;
            if ((pushIndex + 1) < boardSize) {
                y = 1;
            } else {
                y = (pushIndex + 1) / boardSize + 1;
                //y = (pushIndex + 1) / (GoArea.BOARD_SIZE - 2);
            }

            rtnArray[0] = y;
            rtnArray[1] = x;
        }

        return rtnArray;
    }

    public static int XYToTapIndex (int x, int y) {
        return (x - 1) * (GoArea.BOARD_SIZE - 2) + (y - 1);
    }

    //int型の二次元配列をfloat型配列に変換
    public static float[] IntArrToFloatArr (int[, ] intArr) {
        float[] arr = new float[82];
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                int index = XYToTapIndex (i, j);
                arr[index] = intArr[i + 1, j + 1];
            }
        }

        arr[81] = (float) StoneManager.State.PASS;

        return arr;
    }

    public static string FormatFinalScore (string result) {
        string shapeStr = "";
        int stateNum = GoEstimator.stateNum;

        // 結果の文字列が取得できてるかどうかチェック
        int count = 0;
        while (true) {
            if (result.Length > 1 && stateNum == (int) GoEstimator.RequestState.Success) break;
            count++;
            if (count > 10000) {
                //Debug.Log("強制的にループを抜けます");
                break;
            }
        }

        //陣地の差を計算(isFloatがここで切り替わる)
        string diff = CalcDiff (result, stateNum);
        //勝者の色を取得する
        string color = GetWinnerColor (result, stateNum);

        if (isFloat) {
            string diff2 = diff.Substring (0, diff.Length - 2);
            shapeStr = diff2 + "目";
        } else {
            if (result.Contains ("Jigo")) {
                shapeStr = "引き分け";
            } else if (result.Contains ("resign")) {
                shapeStr = "中押し";
            } else if (result.Contains ("time")) {
                shapeStr = "時間切れ";
            } else {
                Debug.LogError ("対応外の対局結果フォーマットです。");
            }
        }

        return color + shapeStr + "勝ち";
    }

    public static string GetWinnerColor (string resultStr, int stateNum) {
        // 結果の文字列が取得できてるかどうかチェック
        int count = 0;
        while (true) {
            if (resultStr.Length > 1 && stateNum == (int) GoEstimator.RequestState.Success) break;
            count++;
            if (count > 10000) {
                //Debug.Log("強制的にループを抜けます");
                break;
            }
        }

        if (resultStr.Contains ("Black")) {
            return "黒";
        } else if (resultStr.Contains ("White")) {
            return "白";
        } else {
            //Debug.LogError("resultの中身 -> " + resultStr);
            return "";
        }
    }

    public static string CalcDiff (string resultStr, int stateNum) {
        string rtn = "";
        //float diff = 0.0f;

        // 結果の文字列が取得できてるかどうかチェック
        int count = 0;
        while (true) {
            if (resultStr.Length > 1 && stateNum == (int) GoEstimator.RequestState.Success) break;
            count++;
            if (count > 10000) {
                //Debug.Log("強制的にループを抜けます");
                break;
            }
        }

        // if (resultStr.Length > 1)
        // {
        string[] strArray = resultStr.Split (' ');
        if (strArray.Length >= 4) {
            if (float.TryParse (strArray[3], out diffNum)) {
                rtn = strArray[3];
                isFloat = true;
                //Debug.LogWarning("isFloat true");
            } else {
                rtn = strArray[3];
                isFloat = false;
                //Debug.LogWarning("isFloat false");
            }
        } else {
            //Debug.LogError("strArrayの要素数が4未満です。 -> " + strArray.Length);
            isFloat = false;
        }
        //}
        //else
        //{
        //Debug.LogError("取得した計算結果が不正な文字列です。 -> " + resultStr);
        //isFloat = false;
        //}

        Debug.Log ("diffNum -> " + diffNum);
        return rtn;
    }

    public static void PrintBoard (int[, ] board, int boardSize) {
        string boardText = "\n";
        for (int i = 1; i < boardSize - 1; i++) {
            for (int j = 1; j < boardSize - 1; j++) {
                if (board[i, j] == 0) {
                    boardText += "●";
                } else if (board[i, j] == 1) {
                    boardText += "○";
                } else {
                    boardText += "+";
                }
                boardText += ",";
            }

            boardText += "\n";
        }

        Debug.LogError(boardText);
    }
}