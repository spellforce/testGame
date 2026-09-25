using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneBootstrapper : MonoBehaviour
{
	public List<GameBoard> Boards;

	public List<GameObject> ObjectsToInstantiate;

	private void Awake()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		GameObject val = new GameObject("Boards");
		foreach (GameBoard board in Boards)
		{
			GameBoard gameBoard = Object.Instantiate<GameBoard>(board);
			((Component)gameBoard).transform.SetParent(val.transform, true);
			((Object)((Component)gameBoard).gameObject).name = ((Object)((Component)board).gameObject).name;
		}
		GameObject val2 = new GameObject("Managers");
		foreach (GameObject item in ObjectsToInstantiate)
		{
			GameObject val3;
			try
			{
				val3 = Object.Instantiate<GameObject>(item);
			}
			catch (Exception ex)
			{
				Debug.LogError((object)"Exception during scene bootstrapping:");
				Debug.LogException(ex);
				continue;
			}
			if (((Object)item).name.Contains("Manager") || ((Object)item).name.Contains("Controller"))
			{
				val3.transform.SetParent(val2.transform, true);
			}
			((Object)val3).name = ((Object)item).name;
		}
		if (PlatformHelper.HasModdingSupport)
		{
			ModManager.instance.ReadyUpMods();
		}
	}
}
