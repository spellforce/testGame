using System.Collections.Generic;
using UnityEngine;

public class WorkerTransformHolder : MonoBehaviour
{
	private List<GameObject> workers = new List<GameObject>();

	public GameObject WorkerPositionPrefab;

	public GameObject WorkerPositionMiddle;

	public float WorkerAmountOffset = 0.1f;

	private void Update()
	{
	}

	public Transform GetTransformAtIndex(int index)
	{
		return workers[Mathf.Clamp(index, 0, workers.Count - 1)].transform;
	}

	public void UpdateWorkerAmount(int workerAmount)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		foreach (GameObject worker in workers)
		{
			Object.Destroy((Object)(object)worker.gameObject);
		}
		workers.Clear();
		for (int i = 0; i < workerAmount; i++)
		{
			Vector3 val = WorkerPositionMiddle.transform.position + new Vector3((float)i * WorkerAmountOffset - (float)(workerAmount / 2) * WorkerAmountOffset + WorkerAmountOffset / 2f * ((workerAmount % 2 == 0) ? 1f : 0f), 0f, 0f);
			GameObject item = Object.Instantiate<GameObject>(WorkerPositionPrefab, val, WorkerPositionMiddle.transform.rotation, ((Component)this).transform);
			workers.Add(item);
		}
	}
}
