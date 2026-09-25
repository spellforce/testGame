using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class RuntimeSokBundle : ISokBundle
{
	private AssetBundle myAssetBundle;

	public bool Load(string id)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		string text = "";
		if ((int)Application.platform == 7 || (int)Application.platform == 2)
		{
			text = Path.Combine(Application.dataPath, "../", id, "PC", id);
		}
		else if ((int)Application.platform == 0 || (int)Application.platform == 1)
		{
			text = Path.Combine(Application.dataPath, "../../", id, "macOS", id);
		}
		myAssetBundle = AssetBundle.LoadFromFile(text);
		if ((Object)(object)myAssetBundle == (Object)null)
		{
			Debug.LogError((object)("No asset bundle found at path " + Path.GetFullPath(text)));
			return false;
		}
		return true;
	}

	public List<T> LoadAssets<T>() where T : Object
	{
		return myAssetBundle.LoadAllAssets<T>().ToList();
	}
}
