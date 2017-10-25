using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 

public static class FactCSVImporter 
{
	static string csvData; 
	static public void readFile(string path)
	{
		csvData = System.IO.File.ReadAllText (path);
		string[,] grid = exportFactDatabase(csvData);
		printDatabase(grid); 
		Debug.Log("size X: " + (1+ grid.GetUpperBound(0)) + " Y: " + (1 + grid.GetUpperBound(1))); 
	}
	static public List<Dictionary<string,string>> importFile(string path) {
		csvData = System.IO.File.ReadAllText (path);
		List<Dictionary<string,string>> database = parseBase(csvData);
		return database;
	}
	static public List<Dictionary<string,string>> parseBase(string csvText) {
		string[] lines = csvText.Split("\n"[0]); 

		int width = 0; 
		for (int i = 0; i < lines.Length; i++)
		{
			string[] row = regexSplit( lines[i] ); 
			width = Mathf.Max(width, row.Length); 
		}
		Dictionary<int,string> header = new Dictionary<int,string>();
		string[] headerRow = regexSplit(lines[0]);
		for (int x = 0; x < headerRow.Length; x++ ) {
			header.Add(x,headerRow[x]);	
		}
		
		// creates new 2D string grid to output to
		string[,] outputGrid = new string[width + 1, lines.Length + 1]; 
		List<Dictionary<string,string>> entList = new List<Dictionary<string,string>> ();
		for (int y = 1; y < lines.Length; y++)
		{
			if (lines [y].Length > 1) {
				string[] row = regexSplit (lines [y]); 
				Dictionary<string,string> entry = new Dictionary<string,string> ();
				for (int x = 0; x < row.Length; x++) {
					entry.Add (headerRow [x], row [x]);
				}
				entList.Add (entry);
			}
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

	static public List<string> splitStringRow(string row) {
		string lastWord = "";
		List<string> strList = new List<string> ();
		foreach (char lastC in row) {
			if (lastC == ';') {
				strList.Add (lastWord);
				lastWord = "";
			} else if (lastC != ' ') {
				lastWord += lastC;
			}
		}
		if (lastWord.Count() > 0) {
			strList.Add (lastWord);
		}
		return strList;
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