using UnityEngine;
using UnityEngine.UI;

public class ScrollbarFixer : MonoBehaviour
{
	private Scrollbar scrollbar;

	private void Start()
	{
		scrollbar = ((Component)this).GetComponent<Scrollbar>();
	}

	private void Update()
	{
		((Selectable)scrollbar).interactable = !InputController.instance.CurrentSchemeIsController;
	}
}
