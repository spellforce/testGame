using System;
using UnityEngine;

[Serializable]
public class VariableFilterCurseActive : VariableFilter
{
	public CurseType Curse;

	public override bool IsMet()
	{
		if ((Object)(object)WorldManager.instance == (Object)null)
		{
			return true;
		}
		return WorldManager.instance.CurseIsActive(Curse);
	}
}
