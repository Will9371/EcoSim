using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour 
{
	//************[Singleton Code]********************
	public static ButtonManager instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}
	//**********************************************

	//All objects that need to be toggled on/off based on game events
	[SerializeField] GameObject nextButton, closeButton;
	[SerializeField] GameObject tutorialParent;
	[SerializeField] GameObject[] textObjs;
	Button3D next, close;
	int tutorialIndex;

	void Start()
	{
		nextButton.SetActive(true);
		tutorialIndex = 0;

		next = nextButton.GetComponent<Button3D>();
		close = closeButton.GetComponent<Button3D>();
	}

	public void OnTriggerPress()
	{
		next.OnClick();
		close.OnClick();
	}

	public void GetButton(GameObject button)
	{
		if(button == nextButton)
			OnNextSelect();
		else if(button == closeButton)
			OnCloseSelect();
	}
		
	void OnNextSelect()
	{
		textObjs[tutorialIndex].SetActive(false);

		if(tutorialIndex + 1 >= textObjs.Length)
			tutorialIndex = 0;
		else
			tutorialIndex++;

		textObjs[tutorialIndex].SetActive(true);
	}

	void OnCloseSelect()
	{
		tutorialParent.SetActive(false);
	}
}
