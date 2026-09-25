using UnityEngine;

public static class FRILerp
{
	public static Vector3 Spring(Vector3 value, Vector3 target, float spring, float drag, ref Vector3 velo)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		velo = Lerp(velo, (target - value) * spring, drag);
		return value + velo * Time.deltaTime;
	}

	public static float Spring(float value, float target, float spring, float drag, ref float velo)
	{
		velo = Lerp(velo, (target - value) * spring, drag);
		return value + velo * Time.deltaTime;
	}

	public static Vector3 Lerp(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Lerp(from, target, 1f - Mathf.Exp((0f - speed) * (useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime)));
	}

	public static Vector3 PLerp(Vector3 from, Vector3 target, float speed, float dt)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Lerp(from, target, 1f - Mathf.Exp((0f - speed) * dt));
	}

	public static Quaternion PLerp(Quaternion from, Quaternion target, float speed, float dt)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Lerp(from, target, 1f - Mathf.Exp((0f - speed) * dt));
	}

	public static float PLerp(float from, float target, float speed, float dt)
	{
		return Mathf.Lerp(from, target, 1f - Mathf.Exp((0f - speed) * dt));
	}

	public static Vector3 LerpFixed(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Lerp(from, target, 1f - Mathf.Exp((0f - speed) * (useTimeScale ? Time.fixedDeltaTime : Time.unscaledDeltaTime)));
	}

	public static Vector3 LerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.LerpUnclamped(from, target, 1f - Mathf.Exp((0f - speed) * Time.deltaTime));
	}

	public static float Lerp(float from, float target, float speed, bool useTimeScale = true)
	{
		return Mathf.Lerp(from, target, 1f - Mathf.Exp((0f - speed) * (useTimeScale ? Time.fixedDeltaTime : Time.unscaledDeltaTime)));
	}

	public static float LerpUnclamped(float from, float target, float speed)
	{
		return Mathf.LerpUnclamped(from, target, 1f - Mathf.Exp((0f - speed) * Time.deltaTime));
	}

	public static Vector3 Slerp(Vector3 from, Vector3 target, float speed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Slerp(from, target, 1f - Mathf.Exp((0f - speed) * Time.deltaTime));
	}

	public static Vector3 SlerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.SlerpUnclamped(from, target, 1f - Mathf.Exp((0f - speed) * Time.deltaTime));
	}

	public static Quaternion Lerp(Quaternion from, Quaternion target, float speed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Lerp(from, target, 1f - Mathf.Exp((0f - speed) * Time.deltaTime));
	}

	public static Quaternion LerpUnclamped(Quaternion from, Quaternion target, float speed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.LerpUnclamped(from, target, 1f - Mathf.Exp((0f - speed) * Time.deltaTime));
	}
}
