using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Word{

	public string word;
	private int typeIndex;
    public int score = 0;

	WordDisplay display;
	public Word (string _word, WordDisplay _display)
	{
		word = _word;
		typeIndex = 0;
        score = word.Length;
		display = _display;
		display.SetWord(word);

	}

	public char GetNextLetter ()
	{
        try
        {
            return word[typeIndex];
        }
        catch (IndexOutOfRangeException)
        {
            throw;
        }
		
	}

	public void TypeLetter ()
	{
		typeIndex++;
		display.RemoveLetter();
	}

	public bool WordTyped ()
	{
		bool wordTyped = (typeIndex >= word.Length);
		if (wordTyped)
		{
			display.RemoveWord();
		}
		return wordTyped;
	}

}
