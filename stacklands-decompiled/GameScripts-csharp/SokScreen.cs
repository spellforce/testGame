using UnityEngine;

public class SokScreen : MonoBehaviour
{
	public RectTransform Rect => ((Component)this).gameObject.GetComponent<RectTransform>();

	public virtual bool IsFrameRateUncapped { get; }
}
