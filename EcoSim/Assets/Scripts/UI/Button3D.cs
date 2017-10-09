using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button3D : MonoBehaviour 
{
	[SerializeField] GameObject SelectEffect, buttonTriggerText;
	[SerializeField] Color buttonColorActive, buttonColorInactive;
	[SerializeField] Text buttonText;
	Material buttonMaterial;
	bool selected;

	void Start()
	{
		buttonMaterial = gameObject.GetComponent<Renderer>().material;
		RemoveSelectionEffects();
		selected = false;
	}

	public void OnHandEnter (GameObject hand)	
	{
		//MainGameManager.instance.selectedList.Add(this.gameObject);
		//if (MainGameManager.instance.selectedList[0] == this.gameObject)
		AddSelectionEffects();
		selected = true;
	}												

	public void OnHandExit(GameObject hand)
	{
		RemoveSelectionEffects();
		//MainGameManager.instance.selectedList.Clear();
		selected = false;
	}

	/*void OnCollisionStay(Collision collision)
	{
		if(MainGameManager.instance.selectedList.Count == 0)
		{
			MainGameManager.instance.selectedList.Add(this.gameObject);
			AddSelectionEffects();
		}
	}*/

	void AddSelectionEffects()
	{
		buttonTriggerText.SetActive (true);
		SelectEffect.SetActive (true);	//if unlocked
		buttonMaterial.SetColor ("_EmissionColor", buttonColorActive);
	}

	void RemoveSelectionEffects()
	{
		buttonTriggerText.SetActive (false);
		SelectEffect.SetActive (false);	
		buttonMaterial.SetColor ("_EmissionColor", buttonColorInactive);
	}

	public void OnClick()
	{
		if (!selected)
			return;

		ButtonManager.instance.GetButton(gameObject);
	}
}



