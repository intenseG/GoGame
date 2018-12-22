using System.Linq;
using UnityEngine;

/// <summary>
/// GameObject 型の拡張メソッドを管理するクラス
/// </summary>

public static class GameObjectExtensions {
	/// <summary>
	/// すべての子オブジェクトを返します
	/// </summary>
	/// <param name="self">GameObject 型のインスタンス</param>
	/// <param name="includeInactive">非アクティブなオブジェクトも取得する場合 true</param>
	/// <returns>すべての子オブジェクトを管理する配列</returns>
	public static GameObject[] GetChildren (
		this GameObject self,
		bool includeInactive = false) {
		return self.GetComponentsInChildren<Transform> (includeInactive)
			.Where (c => c != self.transform)
			.Select (c => c.gameObject)
			.ToArray ();
	}

	//--------------------------------------------------------------------------------
	// 指定したオブジェクトにぶら下がってる子オブジェクトを全削除
	//--------------------------------------------------------------------------------
	public static void DestroyChildObject (this Transform parent_trans) {
		foreach (Transform child in parent_trans) {
			GameObject.Destroy (child.gameObject);
		}
	}
}

public static class ComponentExtensions {
	/// <summary>
	/// すべての子オブジェクトを返します
	/// </summary>
	/// <param name="self">Component 型のインスタンス</param>
	/// <param name="includeInactive">非アクティブなオブジェクトも取得する場合 true</param>
	/// <returns>すべての子オブジェクトを管理する配列</returns>
	public static GameObject[] GetChildren (
		this Component self,
		bool includeInactive = false) {
		return self.GetComponentsInChildren<Transform> (includeInactive)
			.Where (c => c != self.transform)
			.Select (c => c.gameObject)
			.ToArray ();
	}
}