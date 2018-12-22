using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterConverter
{

    public static string ConvertCharacter(string data)
    {
        string encData = null;
        if(!string.IsNullOrEmpty(data))
        {
            encData = EncodeBinaryToString(data);
            Debug.Log(encData);
        }

        return encData;
    }

    /// <summary>
    /// 文字化けした文字列を日本語文字列に変換.
    /// </summary>
    /// <param name="data">コンソール出力文字.</param>
    /// <returns>文字化け文字を日本語変換された文字.</returns>
    private static string EncodeBinaryToString(string data)
    {
        string encResult = null;
        
        List<string> asciiArray = new List<string>();
        List<string> codeArray = new List<string>();

        // ASCIIコードのみを取得するためのパターン.
        string getAsciiPattern = @"(\{U\+)(?<Result>.+?)(\})";

    　  // 文字化け文字列全てを取得するためのパターン.
    　  string getcodePattern = @"(\{.*?\})";

        System.Text.RegularExpressions.Match asciiMatch;
        System.Text.RegularExpressions.Match codeMatch;

        // マッチされない場合はそのまま返す.
        if(!System.Text.RegularExpressions.Regex.IsMatch(data, getcodePattern))
        {
        　　　return data;
        }

        　 // 文字化け文字の取得.
        codeMatch = System.Text.RegularExpressions.Regex.Match(data, getcodePattern);

        　// マッチしている間.
        　while(codeMatch.Success)
        {
            codeArray.Add(codeMatch.Value);

            // 次のマッチングへ.
            codeMatch = codeMatch.NextMatch();
        }

        // ASCIIコードのみを取得.
        asciiMatch = System.Text.RegularExpressions.Regex.Match(data, getAsciiPattern);

        while(asciiMatch.Success)
        {
            asciiArray.Add(asciiMatch.Groups["Result"].Value);
            asciiMatch = asciiMatch.NextMatch();
        }

        List<string> japaneseCharArray = new List<string>();

        // ASCIIコードを日本語文字列に変換する.
        for(int index = 0; index < asciiArray.Count; ++index)　
        {
            // 16進数を基に32bit符号付き変数に変換.
            int intCode16 = Convert.ToInt32(asciiArray[index], 16);

            // char型に変換.
            char conChar = Convert.ToChar(intCode16);

            // string文字列に変換.
            string strChar = conChar.ToString();
            japaneseCharArray.Add(strChar);
        }

        // 元の文字列を日本語文字列に置き換える.
        for(int index = 0; index < japaneseCharArray.Count; ++index)
        {
            data = data.Replace(codeArray[index], japaneseCharArray[index]);
        }

        encResult = data;
        return encResult;
    }
}