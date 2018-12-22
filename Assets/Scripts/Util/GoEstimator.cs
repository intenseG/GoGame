using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using Const;

public class GoEstimator : MonoBehaviour
{
    // 通信状態管理番号リスト
    public enum RequestState
    {
        Empty = 0,
        Success = 1,
        Error = 2
    }

    // private string host = "35.221.127.216";
    // private int port = 22;
    // private string userName = "tsumegonomori15_gmail_com";
    // private string password = "";

    //private ObservableSSHMonoBehaviour ssh;
    //public Queue<string> commands = new Queue<string>();

    public TextAsset sgfFile;

    // ワーキングディレクトリ(サーバーサイド)
    //string working_dir = "https://igo-creative.com/Gnugo/";
    // URL
    string url = "https://igo-creative.com/Gnugo/call_gnugo9.php";
    // サーバへリクエストするデータ
    //string sgfFileName = "test_sgf_1.sgf";
    // 計算結果(文字列)
    public static string result = "";
    // 通信状態番号
    public static int stateNum = (int)RequestState.Empty;
    // タイムアウト時間
    float timeoutsec = 60f;

    public void GetResult()
    {
        stateNum = (int)RequestState.Empty;
        result = "";
        //sgfFileName = fileName;

        // サーバーに置いてあるgnugoと通信して地合計算結果を取得する
        ConnectServer();

        // 結果の文字列が取得できてるかどうかチェック
        int count = 0;
        while (true)
        {
            if (result.Length > 1 && stateNum != (int)RequestState.Empty) break;
            count++;
            if (count > 10000)
            {
                //Debug.Log("強制的にループを抜けます");
                break;
            }
        }

　      //return result;
    }

    private void ConnectServer()
    {
        //SGFの中身を取得
        //string textLines = sgfFile.text; //テキスト全体をstring型で入れる変数を用意して入れる
        //string sgf = textLines;
        string sgf = GoUtil.sgfData;
        Debug.Log(sgf);

        // サーバへPOSTするデータを設定
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("sgf", sgf);
        //dic.Add("move", "est");
        dic.Add("komi", "7");

        // サーバーへPOSTするコルーチンを呼び出す
        StartCoroutine(HttpPost(url, dic));  // POST
        //StartCoroutine(HttpPost());  // POST
    }

    // HTTP POST リクエスト
    IEnumerator HttpPost(string url, Dictionary<string, string> post)
    {
        Debug.Log("1");

        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        Debug.Log("2");
        WWW www = new WWW(url, form);
        Debug.Log("3");

        //CheckTimeOut()の終了を待つ。60秒を過ぎればタイムアウト
        yield return StartCoroutine(CheckTimeOut(www, timeoutsec));

        Debug.Log("4");

        if (www.error != null)
        {
            Debug.LogWarning("HttpPost NG: " + www.error);
            stateNum = (int)RequestState.Error;
        }
        else if (www.isDone)
        {
            // サーバからのレスポンスを整形
            //string resultStr = GoUtil.FormatFinalScore(www.text, stateNum);

            Debug.Log("HttpPost OK: " + www.text);
            // サーバーからのレスポンスを受け取る
            result = www.text;
            stateNum = (int)RequestState.Success;

            // 整形済みの対局結果をstatic変数に代入
            //result = resultStr;
            //Debug.Log("対局結果: " + result);
        }
        else
        {
            result = "Black wins by 0.0 points";
            stateNum = (int)RequestState.Success;
        }
    }

    // HTTP GET リクエスト
    // IEnumerator HttpGet(string url)
    // {
    //     WWW www = new WWW(url);

    //     // CheckTimeOut()の終了を待つ。10秒を過ぎればタイムアウト
    //     yield return StartCoroutine(CheckTimeOut(www, timeoutsec));

    //     if (www.error != null)
    //     {
    //         Debug.LogWarning("HttpGet NG: " + www.error);
    //         stateNum = (int)RequestState.TimeOut;
    //     }
    //     else if (www.isDone)
    //     {
    //         // サーバからのレスポンスを表示
    //         result = www.text;
    //         Debug.Log("HttpGet OK: " + www.text);
    //         stateNum = (int)RequestState.Success;
    //     }
    // }

    // HTTPリクエストのタイムアウト処理
    IEnumerator CheckTimeOut(WWW www, float timeout)
    {
        float requestTime = Time.time;

        while (!www.isDone)
        {
            if (Time.time - requestTime < timeout)
                yield return null;
            else
            {
                Debug.LogError("TimeOut");  //タイムアウト
                break;
            }
        }
        yield return null;
    }
}