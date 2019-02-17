using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordDisplay : MonoBehaviour {
    
	public Text text;
	public float fallSpeed = 1f;
    private bool active = false;
	public void SetWord (string word)
	{
		text.text = word;
	}

	public void RemoveLetter ()
	{
        if (text != null)
        {
            text.text = text.text.Remove(0, 1);
            text.color = Color.red;
            active = true;
        }
	}

	public void RemoveWord ()
	{
        try
        {
            Destroy(gameObject);
        }
        catch (MissingReferenceException)
        {
            
        }
		    
	}

	private void Update()
	{
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<DataCapture>().Locked)
        {
            transform.Translate(0f, -fallSpeed * Time.deltaTime, 0f);
            if (transform.localPosition.y <= -350)
            {
                GameObject.FindGameObjectWithTag("GameController").GetComponent<WordManager>().failed(active);
                Destroy(gameObject);
            }
        }
	}
    

}
