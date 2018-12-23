using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GoEstimator : MonoBehaviour
{
    // 通信状態管理番号リスト
    public enum RequestState
    {
        Empty = 0,
        Processing = 1,
        Success = 2,
        Error = 3
    }

    // gnugoURL
    string url = "https://igo-creative.com/Gnugo/call_gnugo9.php";
    // 計算結果(文字列)
    public static string result = "";
    // 通信状態番号
    public static int stateNum = (int)RequestState.Empty;
    // タイムアウト時間
    float timeoutsec = 60f;

    public void GetResult()
    {
        stateNum = (int)RequestState.Processing;
        result = "";

        // サーバーに置いてあるgnugoと通信して地合計算結果を取得する
        ConnectServer();

        // 結果の文字列が取得できてるかどうかチェック
        int count = 0;
        while (true)
        {
            if (result.Length > 1 && stateNum == (int) RequestState.Success) break;
            count++;
            if (count > 10000)
            {
                //Debug.Log("強制的にループを抜けます");
                break;
            }
        }
    }

    private void ConnectServer()
    {
        //SGFの中身を取得
        string sgf = GoUtil.sgfData;
        Debug.Log(sgf);

        // サーバへPOSTするデータを設定
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("sgf", sgf);
        dic.Add("komi", "7");

        // サーバーへPOSTするコルーチンを呼び出す
        StartCoroutine(HttpPost(url, dic));  // POST
    }

    // HTTP POST リクエスト
    IEnumerator HttpPost(string url, Dictionary<string, string> post)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(url, form);

        //CheckTimeOut()の終了を待つ。60秒を過ぎればタイムアウト
        yield return StartCoroutine(CheckTimeOut(www, timeoutsec));

        if (www.error != null)
        {
            Debug.LogWarning("HttpPost NG: " + www.error);
            stateNum = (int)RequestState.Error;
        }
        else if (www.isDone)
        {
            Debug.Log("HttpPost OK: " + www.text);
            // サーバーからのレスポンスを受け取る
            result = www.text;
            stateNum = (int)RequestState.Success;
        }
        else
        {
            result = "Black wins by 0.0 points";
            stateNum = (int)RequestState.Success;
        }
    }

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