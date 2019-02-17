using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordManager : MonoBehaviour {

	public List<Word> words;

	public WordSpawner wordSpawner;
    private int TotalScore = 0;
	private bool hasActiveWord;
	private Word activeWord;
    public void failed(bool active)
    {
        if (active)
        {
            hasActiveWord = false;
            activeWord = null;
        }
    }
	public void AddWord ()
	{
		Word word = new Word(WordGenerator.GetRandomWord(), wordSpawner.SpawnWord());
		Debug.Log(word.word);

		words.Add(word);
	}

	public void TypeLetter (char letter)
	{
		if (hasActiveWord && activeWord!=null && GameObject.FindGameObjectWithTag("Player").GetComponent<DataCapture>().Locked)
		{
			if (activeWord.GetNextLetter() == letter)
			{
				activeWord.TypeLetter();
			}
		} else
		{
			foreach(Word word in words)
			{
                try
                {
                    if (word.GetNextLetter() == letter)
                    {
                        activeWord = word;
                        hasActiveWord = true;
                        word.TypeLetter();
                        break;
                    }
                }
                catch (System.Exception)
                {
                    hasActiveWord = false;
                    continue;
                }
				
			}
		}

		if (hasActiveWord && activeWord.WordTyped())
		{
            updateScore(activeWord.score);
            hasActiveWord = false;
			words.Remove(activeWord);
            
		}
	}
    public void updateScore(int score)
    {
        TotalScore += score;
        GameObject.FindGameObjectWithTag("Score").GetComponent<Text>().text = "Score = " + TotalScore;
    }
}
