using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Self.Architecture.CSV;
using Self.Story;
using UnityEditor;
using UnityEngine;

public class PlainTextChapterExporter
{
	public static readonly Vector2 scale = new Vector3(600f, 400f);

	[MenuItem("StoryEditor/Fix nodes positions", priority = 100)]
	private static void FixPositions()
	{
		var chapter = Selection.activeObject as Chapter;

		if (chapter == null) return;

		//var grid   = new Dictionary<Vector2Int, BaseNode>();
		var offset = -chapter.NodesByID[chapter.startNodeID].position;

		foreach (var node in chapter.nodes)
		{
			var pos = node.position + offset;

			pos.x = Mathf.FloorToInt(pos.x / scale.x + 0.5f) * scale.x + 10f;
			pos.y = Mathf.FloorToInt(pos.y / scale.y + 0.5f) * scale.y + 10f;

			node.position = pos;
			EditorUtility.SetDirty(node);
		}

		EditorUtility.SetDirty(chapter);
		AssetDatabase.SaveAssets();
	}

	[MenuItem("StoryEditor/Export chapter to plain text", priority = 101)]
	private static void Export()
	{
		var chapter = Selection.activeObject as Chapter;

		if (chapter == null) return;

		var sb = new StringBuilder();

		int counter  = 0;
		var branches = new Queue<string>();
		var reached  = new HashSet<BaseNode>();
		branches.Enqueue(chapter.startNodeID);
		while (branches.Count > 0)
		{
			var id   = branches.Dequeue();
			var node = chapter.NodesByID[id];
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine();
			while (node != null)
			{
				counter++;
				Debug.Log($"Node {counter} : {node.position.x}");
				reached.Add(node);
				if (node is ChoiceNode choice)
				{
					sb.AppendLine($"Node: {counter}");
					sb.AppendLine($"{choice.character.characterName}:");
					sb.AppendLine($"  - {choice.localized}");
					sb.AppendLine($"Choices:");
					for (int i = 0; i < choice.choices.Count; i++)
					{
						var c = choice.choices[i];
						sb.AppendLine($"  - {c.localizedText}");
						branches.Enqueue(choice.nextNodes[i]);
					}

					sb.AppendLine();
					break;
				}

				if (node is ReplicaNode replica)
				{
					sb.AppendLine($"Node: {counter}");
					sb.AppendLine($"{replica.character.characterName}:");
					sb.AppendLine($"  - {replica.localized}");
					sb.AppendLine();
				}

				id = node.NextNode;
				if (!chapter.NodesByID.ContainsKey(id))
				{
					break;
				}

				node = chapter.NodesByID[id];
				if (reached.Contains(node))
					node = null;
			}
		}

		var path = AssetDatabase.GetAssetPath(chapter);
		var file = Path.Combine(Path.GetDirectoryName(path), $"{chapter.name}_plain_text.txt");
		File.WriteAllText(file, sb.ToString());
	}

	class TextNode
	{
		public string character;
		public string text;
	}

	[MenuItem("StoryEditor/Export chapter to csv", priority = 102)]
	private static void ExportToCSV()
	{
		var chapter = Selection.activeObject as Chapter;

		if (chapter == null) return;

		var choices = new Dictionary<int, List<TextNode>>();

		var lines = new List<List<TextNode>>(chapter.nodes.Count);
		for (int i = 0; i < chapter.nodes.Count; i++)
		{
			lines.Add(new List<TextNode>());
		}

		var nodes = new List<BaseNode>(chapter.nodes);
		nodes.Sort((l, r) => l.position.y.CompareTo(r.position.y));

		foreach (var node in nodes)
		{
			var row = Mathf.FloorToInt(node.position.x / scale.x);
			//var col = Mathf.FloorToInt(node.position.x / scale.x);

			if (node is ChoiceNode choice)
			{
				lines[row].Add(new TextNode
				{
					character = choice.character.characterName,
					text      = choice.localized
				});

				if (!choices.ContainsKey(row))
					choices[row] = new List<TextNode>();

				var count = choice.choices.Count;
				for (int i = 0; i < count; i++)
				{
					choices[row].Add(new TextNode
					{
						character = $"Choine {count - i}:",
						text      = choice.choices[i].localizedText
					});
				}
			}
			else if (node is ReplicaNode replica)
			{
				lines[row].Add(new TextNode
				{
					character = replica.character.characterName,
					text      = replica.localized
				});
			}
		}

		for (int k = chapter.nodes.Count; k >= 0; k--)
		{
			if (!choices.TryGetValue(k, out var clist))
				continue;
			lines.Insert(k + 1, clist);
		}

		var table = new List<string[]>();
		foreach (var line in lines)
		{
			var row = new List<string>();

			line.Reverse();
			foreach (var node in line)
			{
				row.Add(node.character);
				row.Add(node.text);
			}

			table.Add(row.ToArray());
		}

		var path = AssetDatabase.GetAssetPath(chapter);
		var file = Path.Combine(Path.GetDirectoryName(path), $"{chapter.name}_table.csv");
		var csv  = CSVUtils.ToString(table);
		File.WriteAllText(file, csv);
	}
	
	
	
	[MenuItem("StoryEditor/Export chapter to csv V2", priority = 102)]
	private static void ExportToCSV_V2()
	{
		var chapter = Selection.activeObject as Chapter;

		if (chapter == null) return;

		var choices = new Dictionary<int, List<TextNode>>();

		
		var sx   = 30;
		var sy   = chapter.nodes.Count*2;
		var grid = new TextNode[sx, sy];

		foreach (var node in chapter.nodes)
		{
			var gridPos = new Vector2Int(
				Mathf.FloorToInt(node.position.x / scale.x + 0.5f),
				-Mathf.FloorToInt(node.position.y / scale.y + 0.5f)
			);

			if (node is ReplicaNode replica)
			{
				grid[gridPos.y, gridPos.x] = new TextNode
				{
					character = replica.character.characterName,
					text      = replica.localized
				};
			}

			if (node is ChoiceNode choice)
			{
				var row = gridPos.x;
				if (!choices.ContainsKey(row))
					choices[row] = new List<TextNode>();
				var count = choice.choices.Count;
				for (int i = 0; i < count; i++)
				{
					choices[row].Add(new TextNode
					{
						character = $"Choine {count - i}:",
						text      = choice.choices[i].localizedText
					});
				}
			}
		}
		
		
		var table = new List<string[]>();
		for (int y = 0; y < sy; y++)
		{
			var row = new List<string>();

			for (int x=0;x<sx;x++)
			{
				var node = grid[x, y];
				if (node == null)
				{
					row.Add("");
					row.Add("");
				}
				else
				{
					row.Add(node.character);
					row.Add(node.text);
				}
			}

			table.Add(row.ToArray());
		}
		for (int k = chapter.nodes.Count; k >= 0; k--)
		{
			if (!choices.TryGetValue(k, out var choiceLine))
				continue;
			var line = new List<string>();
			choiceLine.Reverse();
			foreach (var c in choiceLine)
			{
				line.Add(c.character);
				line.Add(c.text);
			}
			table.Insert(k + 1, line.ToArray());
		}

		
		var path = AssetDatabase.GetAssetPath(chapter);
		var file = Path.Combine(Path.GetDirectoryName(path), $"{chapter.name}_table.csv");
		var csv  = CSVUtils.ToString(table);
		File.WriteAllText(file, csv);
	}
}