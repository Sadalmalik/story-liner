using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Self.Story;
using UnityEditor;
using UnityEngine;

public class PlainTextChapterExporter
{
    [MenuItem("StoryEditor/Export chapter to plain text")]
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
                    for (int i=0;i<choice.choices.Count;i++)
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

    [MenuItem("StoryEditor/Fix nodes positions")]
    private static void Fix()
    {
        var chapter = Selection.activeObject as Chapter;

        if (chapter == null) return;

        var offset = -chapter.NodesByID[chapter.startNodeID].position;
        
        foreach (var node in chapter.nodes)
        {
            var pos = node.position + offset;
            pos.x = Mathf.Floor(pos.x / 500) * 500;
            EditorUtility.SetDirty(node);
        }
        
        EditorUtility.SetDirty(chapter);
        AssetDatabase.SaveAssets();
    }
    
}
