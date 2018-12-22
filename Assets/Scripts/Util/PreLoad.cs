using UnityEngine;
using Const;

[CreateAssetMenu()]
public class PreLoad : ScriptableObject {

	private void OnEnable()
	{
		if (PlayerPrefs.HasKey(Key.IS_CREATED))
		{
			PlayerPrefs.DeleteKey(Key.IS_CREATED);
		}

		if (PlayerPrefs.HasKey(Key.SGF_GAME_COUNT))
		{
			PlayerPrefs.DeleteKey(Key.SGF_GAME_COUNT);
		}
	}
}
