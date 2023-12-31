﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LS;

public class WordGameDict: Singleton<WordGameDict>
	{
	// In C# using a HashSet is an O(1) operation. It's a dictionary without the keys!
	//private HashSet<string> words = new HashSet<string>();

	private List<string> words = new List<string>(); 
	private TextAsset dictText;


    /*public WordGameDict(){
		InitializeDictionary("ospd");
	}

	public WordGameDict(string filename){
		InitializeDictionary(filename);
	}*/

    private void Start()
    {
		words = new List<string>();
		InitializeDictionary("ospd");
    }

    protected void InitializeDictionary(string filename){
		dictText = (TextAsset) Resources.Load(filename, typeof (TextAsset));
		var text = dictText.text;
		//print(text);
		foreach (string s in text.Split('\n')){
			words.Add(s);			
		}
		//Debug.Log("clear:" + words.Count);
	}
	
	public bool CheckWord(string word){
		return (words.Contains(word));
		//return true;
	}
}
