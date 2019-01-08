using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingEffect : MonoBehaviour {

    //public List<string> exampleTexts;

    Text textComponent;

    int typeNumber;

    public float textSpeed = 0.03f;

	// Use this for initialization
	void Awake () 
    {
        textComponent = gameObject.GetComponent<Text>();
	}

    public void NextText(string text)
    {
        StopAllCoroutines();

        StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {

        for (int i = 0; i < (text.Length + 1) ; i++)
        {
            textComponent.text = text.Substring(0, i); 
            yield return new WaitForSeconds(textSpeed);
        }

    }
}
