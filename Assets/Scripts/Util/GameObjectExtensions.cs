using UnityEngine;

/// <summary>
/// GameObject 型の拡張メソッドを管理するクラス
/// </summary>
public static partial class GameObjectExtensions
{
	//--------------------------------------------------------------------------------
	// 指定したオブジェクトにぶら下がってる子オブジェクトを全削除
	//--------------------------------------------------------------------------------
	public static void DestroyChildObject(Transform parent_trans)
	{
		for(int i = 0; i < parent_trans.childCount; ++i)
        {
			GameObject.Destroy(parent_trans.GetChild(i).gameObject);
		}
	}
}