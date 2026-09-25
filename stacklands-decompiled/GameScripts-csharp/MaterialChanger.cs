using System;
using System.Collections;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
	private MeshRenderer myRenderer;

	private Material[] startMaterials;

	private Material[] currentMaterials;

	public void Init()
	{
		if (!((Object)(object)myRenderer != (Object)null))
		{
			myRenderer = ((Component)this).GetComponent<MeshRenderer>();
			if ((Object)(object)myRenderer == (Object)null)
			{
				Debug.Log((object)(((Object)((Component)this).gameObject).name + " does not have a MeshRenderer"));
			}
			startMaterials = ((Renderer)myRenderer).sharedMaterials;
			currentMaterials = ((Renderer)myRenderer).sharedMaterials;
		}
	}

	private void Awake()
	{
		Init();
	}

	public void SetMaterial(Material mat)
	{
		if ((Object)(object)myRenderer == (Object)null)
		{
			return;
		}
		for (int i = 0; i < currentMaterials.Length; i++)
		{
			if (!(((Object)currentMaterials[i]).name == "Invisible"))
			{
				currentMaterials[i] = mat;
			}
		}
		((Renderer)myRenderer).sharedMaterials = currentMaterials;
	}

	public void SetMaterialForTime(Material mat, float time, Action afterAction = null)
	{
		if (((Component)this).gameObject.activeInHierarchy)
		{
			SetMaterial(mat);
			((MonoBehaviour)this).StartCoroutine(ResetAfter(time, afterAction));
		}
	}

	private IEnumerator ResetAfter(float t, Action action)
	{
		yield return (object)new WaitForSeconds(t);
		ResetMaterials();
		action?.Invoke();
	}

	public void ResetMaterials()
	{
		if (!((Object)(object)myRenderer == (Object)null))
		{
			((Renderer)myRenderer).sharedMaterials = startMaterials;
		}
	}
}
