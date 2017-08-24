﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 

public class FactCSVImporter 
{
	public string csvData; 
	public void readFile(string path)
	{
		csvData = System.IO.File.ReadAllText (path);
		string[,] grid = exportFactDatabase(csvData);
		printDatabase(grid); 
		Debug.Log("size X: " + (1+ grid.GetUpperBound(0)) + " Y: " + (1 + grid.GetUpperBound(1))); 
	}
	public List<DatabaseEntry> importFile(string path) {
		csvData = System.IO.File.ReadAllText (path);
		List<DatabaseEntry> database = createDatabase(csvData);
		return database;
	}
	public List<DatabaseEntry> createDatabase(string csvText) {
		string[] lines = csvText.Split("\n"[0]); 

		// finds the max width of row
		int width = 0; 
		for (int i = 0; i < lines.Length; i++)
		{
			string[] row = regexSplit( lines[i] ); 
			width = Mathf.Max(width, row.Length); 
		}

		// creates new 2D string grid to output to
		string[,] outputGrid = new string[width + 1, lines.Length + 1]; 
		List<DatabaseEntry> entList = new List<DatabaseEntry> ();
		for (int y = 1; y < lines.Length; y++)
		{
			string[] row = regexSplit( lines[y] ); 
			DatabaseEntry newDB = new DatabaseEntry ();
			newDB.factID = row [0] + "--" + row [1];
			newDB.category = row [0];
			newDB.info = row [2];
			newDB.factGroups = new List<string>();
			for (int x = 3; x < row.Length; x++) 
			{
				newDB.factGroups.Add(row[x]);
			}
			entList.Add (newDB);
		}
		return entList; 
	}

	static public string[,] exportFactDatabase(string csvText)
	{
		string[] lines = csvText.Split("\n"[0]); 

		// finds the max width of row
		int width = 0; 
		for (int i = 0; i < lines.Length; i++)
		{
			string[] row = regexSplit( lines[i] ); 
			width = Mathf.Max(width, row.Length); 
		}

		// creates new 2D string grid to output to
		string[,] outputGrid = new string[width + 1, lines.Length + 1]; 
		for (int y = 0; y < lines.Length; y++)
		{
			string[] row = regexSplit( lines[y] ); 
			for (int x = 0; x < row.Length; x++) 
			{
				outputGrid[x,y] = row[x]; 
				outputGrid[x,y] = outputGrid[x,y].Replace("\"\"", "\"");
			}
		}

		return outputGrid; 
	}

	static public void printDatabase(string[,] grid)
	{
		string debugStr = ""; 
		for (int y = 0; y < grid.GetUpperBound(1); y++) {	
			for (int x = 0; x < grid.GetUpperBound(0); x++) {

				debugStr += grid[x,y]; 
				debugStr += "|"; 
			}
			debugStr += "\n"; 
		}
		Debug.Log(debugStr);
	}

	static public string[] regexSplit(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
			@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)", 
			System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
			select m.Groups[1].Value).ToArray();
	}
}