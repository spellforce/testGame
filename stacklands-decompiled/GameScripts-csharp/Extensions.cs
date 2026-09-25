using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
	private static Vector3[] corners = (Vector3[])(object)new Vector3[4];

	public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence)
	{
		if (sequence == null)
		{
			yield break;
		}
		List<T> list = sequence.ToList();
		if (!list.Any())
		{
			yield return Enumerable.Empty<T>();
			yield break;
		}
		int startingElementIndex = 0;
		foreach (T startingElement in list)
		{
			int index = startingElementIndex;
			IEnumerable<T> sequence2 = list.Where((T e, int i) => i != index);
			foreach (IEnumerable<T> item in sequence2.Permute())
			{
				yield return item.Prepend(startingElement);
			}
			startingElementIndex++;
		}
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = Random.Range(0, num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static void SetParentClean(this Transform t, Transform parent)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		t.SetParent(parent);
		t.localScale = Vector3.one;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
	}

	public static Bounds TransformBoundsTo(this RectTransform source, Transform target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default(Bounds);
		if ((Object)(object)source != (Object)null)
		{
			source.GetWorldCorners(corners);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(float.MinValue, float.MinValue, float.MinValue);
			Matrix4x4 worldToLocalMatrix = target.worldToLocalMatrix;
			for (int i = 0; i < 4; i++)
			{
				Vector3 val3 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint3x4(corners[i]);
				val = Vector3.Min(val3, val);
				val2 = Vector3.Max(val3, val2);
			}
			((Bounds)(ref result))._002Ector(val, Vector3.zero);
			((Bounds)(ref result)).Encapsulate(val2);
		}
		return result;
	}

	public static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		RectTransform viewport = scrollRect.viewport;
		RectTransform val = (((Object)(object)viewport != (Object)null) ? viewport : ((Component)scrollRect).GetComponent<RectTransform>());
		Rect rect = val.rect;
		Vector3 val2 = Vector2.op_Implicit(((Rect)(ref rect)).center);
		rect = val.rect;
		Bounds val3 = default(Bounds);
		((Bounds)(ref val3))._002Ector(val2, Vector2.op_Implicit(((Rect)(ref rect)).size));
		RectTransform content = scrollRect.content;
		Bounds val4 = (Bounds)(((Object)(object)content != (Object)null) ? content.TransformBoundsTo((Transform)(object)val) : default(Bounds));
		Vector3 size = ((Bounds)(ref val4)).size;
		float num = ((Vector3)(ref size))[axis];
		size = ((Bounds)(ref val3)).size;
		float num2 = num - ((Vector3)(ref size))[axis];
		return distance / num2;
	}

	public static void ScrollToCenter(this ScrollRect scrollRect, RectTransform target, bool clampVerticalPos = true)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		RectTransform val = (((Object)(object)scrollRect.viewport != (Object)null) ? scrollRect.viewport : ((Component)scrollRect).GetComponent<RectTransform>());
		Rect rect = val.rect;
		Bounds val2 = target.TransformBoundsTo((Transform)(object)val);
		float distance = ((Rect)(ref rect)).center.y - ((Bounds)(ref val2)).center.y;
		float num = scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, distance);
		if (clampVerticalPos)
		{
			scrollRect.verticalNormalizedPosition = Mathf.Clamp(num, 0f, 1f);
		}
		else
		{
			scrollRect.verticalNormalizedPosition = num;
		}
	}

	public static LocParam LocParam_Action(string actionName)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return LocParam.Create("action_" + actionName, InputController.instance.GetActionDisplayString(actionName));
	}

	public static string TranslateEnum<T>(this T value) where T : struct, IConvertible
	{
		return SokLoc.Translate($"{value.GetType().ToString().ToLower()}_{value}");
	}

	public static List<T> AsList<T>(this T value)
	{
		return new List<T> { value };
	}

	public static T Choose<T>(this List<T> list)
	{
		return list[Random.Range(0, list.Count)];
	}

	public static T Choose<T>(this T[] arr)
	{
		return arr[Random.Range(0, arr.Length)];
	}

	public static Vector3 Perlin(float t)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mathf.PerlinNoise(t, 0f), 0f, Mathf.PerlinNoise(t, 0.5f));
	}

	public static Vector3 PerlinNormalized(float t)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(PerlinNoise2(t, 0f), 0f, PerlinNoise2(t, 0.5f));
	}

	private static float PerlinNoise2(float x, float y)
	{
		return Mathf.PerlinNoise(x, y) * 2f - 1f;
	}

	public static List<string> FindMatches(string str, string[] arr)
	{
		List<string> list = new List<string>();
		foreach (string text in arr)
		{
			if (text.Contains(str))
			{
				list.Add(text);
			}
		}
		return list;
	}
}
