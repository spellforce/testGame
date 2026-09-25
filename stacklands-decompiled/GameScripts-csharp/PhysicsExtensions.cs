using System;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicsExtensions
{
	private class AscendingDistanceComparer : IComparer<RaycastHit>
	{
		public int Compare(RaycastHit h1, RaycastHit h2)
		{
			if (!(((RaycastHit)(ref h1)).distance < ((RaycastHit)(ref h2)).distance))
			{
				if (!(((RaycastHit)(ref h1)).distance > ((RaycastHit)(ref h2)).distance))
				{
					return 0;
				}
				return 1;
			}
			return -1;
		}
	}

	private static AscendingDistanceComparer ascendDistance = new AscendingDistanceComparer();

	public static bool BoxCast(BoxCollider box, Vector3 direction, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.BoxCast(center, halfExtents, direction, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool BoxCast(BoxCollider box, Vector3 direction, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.BoxCast(center, halfExtents, direction, ref hitInfo, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static RaycastHit[] BoxCastAll(BoxCollider box, Vector3 direction, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.BoxCastAll(center, halfExtents, direction, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static int BoxCastNonAlloc(BoxCollider box, Vector3 direction, RaycastHit[] results, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool CheckBox(BoxCollider box, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.CheckBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);
	}

	public static Collider[] OverlapBox(BoxCollider box, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);
	}

	public static int OverlapBoxNonAlloc(BoxCollider box, Collider[] results, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		box.ToWorldSpaceBox(out var center, out var halfExtents, out var orientation);
		return Physics.OverlapBoxNonAlloc(center, halfExtents, results, orientation, layerMask, queryTriggerInteraction);
	}

	public static int OverlapTwoBoxNonAlloc(BoxCollider box, BoxCollider box2, Collider[] results, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = ((Collider)box).bounds;
		Bounds bounds2 = ((Collider)box2).bounds;
		((Bounds)(ref bounds)).Encapsulate(((Bounds)(ref bounds2)).min);
		((Bounds)(ref bounds)).Encapsulate(((Bounds)(ref bounds2)).max);
		return Physics.OverlapBoxNonAlloc(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).extents, results, Quaternion.identity, layerMask, queryTriggerInteraction);
	}

	public static void ToWorldSpaceBox(this BoxCollider box, out Vector3 center, out Vector3 halfExtents, out Quaternion orientation)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		orientation = ((Component)box).transform.rotation;
		center = ((Component)box).transform.TransformPoint(box.center);
		Vector3 val = AbsVec3(((Component)box).transform.lossyScale);
		halfExtents = Vector3.Scale(val, box.size) * 0.5f;
	}

	public static void ToWorldSpaceBox2(this BoxCollider box, out Vector3 halfExtents)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 lossyScale = ((Component)box).transform.lossyScale;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y), Mathf.Abs(lossyScale.z));
		halfExtents = Vector3.Scale(val, box.size) * 0.5f;
	}

	public static bool SphereCast(SphereCollider sphere, Vector3 direction, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.SphereCast(center, radius, direction, ref hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static RaycastHit[] SphereCastAll(SphereCollider sphere, Vector3 direction, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.SphereCastAll(center, radius, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static int SphereCastNonAlloc(SphereCollider sphere, Vector3 direction, RaycastHit[] results, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.SphereCastNonAlloc(center, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool CheckSphere(SphereCollider sphere, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.CheckSphere(center, radius, layerMask, queryTriggerInteraction);
	}

	public static Collider[] OverlapSphere(SphereCollider sphere, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.OverlapSphere(center, radius, layerMask, queryTriggerInteraction);
	}

	public static int OverlapSphereNonAlloc(SphereCollider sphere, Collider[] results, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		sphere.ToWorldSpaceSphere(out var center, out var radius);
		return Physics.OverlapSphereNonAlloc(center, radius, results, layerMask, queryTriggerInteraction);
	}

	public static void ToWorldSpaceSphere(this SphereCollider sphere, out Vector3 center, out float radius)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		center = ((Component)sphere).transform.TransformPoint(sphere.center);
		radius = sphere.radius * MaxVec3(AbsVec3(((Component)sphere).transform.lossyScale));
	}

	public static bool CapsuleCast(CapsuleCollider capsule, Vector3 direction, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.CapsuleCast(point, point2, radius, direction, ref hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static RaycastHit[] CapsuleCastAll(CapsuleCollider capsule, Vector3 direction, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.CapsuleCastAll(point, point2, radius, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static int CapsuleCastNonAlloc(CapsuleCollider capsule, Vector3 direction, RaycastHit[] results, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.CapsuleCastNonAlloc(point, point2, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	public static bool CheckCapsule(CapsuleCollider capsule, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.CheckCapsule(point, point2, radius, layerMask, queryTriggerInteraction);
	}

	public static Collider[] OverlapCapsule(CapsuleCollider capsule, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.OverlapCapsule(point, point2, radius, layerMask, queryTriggerInteraction);
	}

	public static int OverlapCapsuleNonAlloc(CapsuleCollider capsule, Collider[] results, int layerMask = -5, QueryTriggerInteraction queryTriggerInteraction = (QueryTriggerInteraction)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		capsule.ToWorldSpaceCapsule(out var point, out var point2, out var radius);
		return Physics.OverlapCapsuleNonAlloc(point, point2, radius, results, layerMask, queryTriggerInteraction);
	}

	public static void ToWorldSpaceCapsule(this CapsuleCollider capsule, out Vector3 point0, out Vector3 point1, out float radius)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)capsule).transform.TransformPoint(capsule.center);
		radius = 0f;
		float num = 0f;
		Vector3 val2 = AbsVec3(((Component)capsule).transform.lossyScale);
		Vector3 val3 = Vector3.zero;
		switch (capsule.direction)
		{
		case 0:
			radius = Mathf.Max(val2.y, val2.z) * capsule.radius;
			num = val2.x * capsule.height;
			val3 = ((Component)capsule).transform.TransformDirection(Vector3.right);
			break;
		case 1:
			radius = Mathf.Max(val2.x, val2.z) * capsule.radius;
			num = val2.y * capsule.height;
			val3 = ((Component)capsule).transform.TransformDirection(Vector3.up);
			break;
		case 2:
			radius = Mathf.Max(val2.x, val2.y) * capsule.radius;
			num = val2.z * capsule.height;
			val3 = ((Component)capsule).transform.TransformDirection(Vector3.forward);
			break;
		}
		if (num < radius * 2f)
		{
			val3 = Vector3.zero;
		}
		point0 = val + val3 * (num * 0.5f - radius);
		point1 = val - val3 * (num * 0.5f - radius);
	}

	public static void SortClosestToFurthest(RaycastHit[] hits, int hitCount = -1)
	{
		if (hitCount != 0)
		{
			if (hitCount < 0)
			{
				hitCount = hits.Length;
			}
			Array.Sort(hits, 0, hitCount, ascendDistance);
		}
	}

	private static Vector3 AbsVec3(Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
	}

	private static float MaxVec3(Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Max(v.x, Mathf.Max(v.y, v.z));
	}
}
