using UnityEngine;

public class ImageEffect : MonoBehaviour
{
	public Material PostProcessingMaterial;

	public float Weight;

	private Material realMaterial;

	private void Awake()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		realMaterial = new Material(PostProcessingMaterial);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		realMaterial.SetFloat("_Weight", Weight);
		Graphics.Blit((Texture)(object)source, destination, realMaterial);
	}
}
