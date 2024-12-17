using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Self.Story;
using Self.Story.Editors;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Self.ArticyImporter
{
    public static class ArticyProjectImporter
    {
#region Menu

        [MenuItem("StoryEditor/Import From Articy File")]
        private static void ImportFileFromArticy()
        {
            var file = EditorUtility.OpenFilePanel("Open File", Application.dataPath, "json");

            ImportFromArticy(file);
        }

        [MenuItem("StoryEditor/Import From Articy Selected")]
        private static void ImportSelectedFromArticy()
        {
            var file = Path.GetFullPath(
                AssetDatabase.GetAssetPath(Selection.activeObject)
            ).Replace("\\", "/");

            ImportFromArticy(file);
        }

#endregion


#region Main Import

        private static void ImportFromArticy(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new System.Exception("Could not open file, chapFolder is null or empty");

            //var relativeFilePath = Path.GetRelativePath(Application.dataPath, file);
            var relativeFilePath = $"Assets/{file.Split("Assets/")[1]}";
            var jsonAsset        = AssetDatabase.LoadAssetAtPath<TextAsset>(relativeFilePath);

            if (jsonAsset == null)
                throw new System.Exception($"Error loading asset at chapFolder {relativeFilePath}");

            ArticyStructureTool.ReorderArticyFolders(Path.Combine(relativeFilePath, "Assets"));

            var data = JsonConvert.DeserializeObject<ArticyData>(jsonAsset.text);
            data.InitAssets();

            //var storyNodes = new Dictionary<ulong, StoryNode>();
            //data.Packages[0].Models.ForEach(m => storyNodes.Add(m.Properties.Id.Value, m));

            var creationPath = Path.GetDirectoryName(relativeFilePath);
            var characters   = CreateCharactersFromImport(data, creationPath);

            var book = CreateBookFromImport(data, creationPath);
            book.characters = characters.Values.ToList();

            var chapters = CreateChaptersFromImport(book, data, characters, creationPath);

            foreach (var chapter in chapters)
            {
                chapter.book = book;
                EditorUtility.SetDirty(chapter);
            }

            AssetDatabase.SaveAssets();
        }

        private static Dictionary<HexValue, Character> CreateCharactersFromImport(ArticyData data, string path)
        {
            var entities = data.Hierarchy.Children.First(node => node.Type == "Entities");

            if (entities == null)
                throw new System.Exception("Node 'Entities' not foud in Aticy project hierarchy!");

            HashSet<HexValue> characterIds = entities.Children.Select(node => node.Id).ToHashSet();

            var characters = data.Packages[0].Models.Where(node => characterIds.Contains(node.Properties.Id));

            var charFolder = Path.Combine(path, "Characters");

            if (!Directory.Exists(charFolder))
                Directory.CreateDirectory(charFolder);

            var characterList = new Dictionary<HexValue, Character>();

            foreach (var character in characters)
            {
                var newCharacter = ScriptableUtils.CreateAsset<Character>(
                    charFolder, $"Character_{character.Properties.DisplayName}");
                newCharacter.isMainCharacter = character.Type.Equals("DefaultMainCharacterTemplate");
                newCharacter.characterName   = character.Properties.DisplayName;

                Debug.Log($"Try get asset: {character.Properties.PreviewImage.Asset}");
                if (data.Assets.TryGetValue(character.Properties.PreviewImage.Asset, out var asset))
                {
                    var assetPath = Path.Combine(path, "Assets/Characters", Path.GetFileName(asset.AssetRef));
                    Debug.Log($"Set character: '{asset.AssetRef}' => '{assetPath}'");
                    newCharacter.characterIcon = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                }

                EditorUtility.SetDirty(newCharacter);
                AssetDatabase.SaveAssets();

                characterList.Add(character.Properties.Id, newCharacter);
            }

            return characterList;
        }

#endregion


        private static Book CreateBookFromImport(ArticyData data, string path)
        {
            var newBook            = ScriptableUtils.CreateAsset<Book>(path, $"Book_{data.Project.Name}");
            var variablesContainer = ScriptableUtils.AddToAsset<VariablesContainer>(newBook, ".settings.variables");

            newBook.variables = variablesContainer;
            newBook.bookName  = data.Project.Name;

            foreach (var variableSet in data.GlobalVariables)
            {
                foreach (var variable in variableSet.Variables)
                {
                    var      varName = $".{variableSet.Namespace}.{variable.Variable}";
                    var      varId   = $"{variableSet.Namespace}.{variable.Variable}";
                    Variable newVar  = null;

                    switch (variable.Type)
                    {
                        case "Boolean":
                            var boolVar = ScriptableUtils.AddToAsset<BoolVariable>(newBook, varName);
                            bool.TryParse(variable.Value, out boolVar.value);
                            newVar = boolVar;
                            break;
                        case "String":
                            var stringVar = ScriptableUtils.AddToAsset<StringVariable>(newBook, varName);
                            stringVar.value = variable.Value;
                            newVar          = stringVar;
                            break;
                        case "Integer":
                            var intVar = ScriptableUtils.AddToAsset<IntVariable>(newBook, varName);

                            if (int.TryParse(variable.Value, out int parsedValue))
                            {
                                if (parsedValue > intVar.maxValue)
                                    intVar.maxValue = parsedValue;

                                intVar.value = parsedValue;
                            }

                            newVar = intVar;
                            break;
                        default:
                            break;
                    }

                    newVar.id = varId;
                    variablesContainer.Add(newVar);
                }
            }

            EditorUtility.SetDirty(newBook);
            AssetDatabase.SaveAssets();

            return newBook;
        }

        private static List<Chapter> CreateChaptersFromImport(
            Book                            book,
            ArticyData                      data,
            Dictionary<HexValue, Character> characters,
            string                          path)
        {
            var chapFolder = $"{path}/Chapters/";

            if (!Directory.Exists(chapFolder))
                Directory.CreateDirectory(chapFolder);

            var chapterList = new List<Chapter>();
            var chapters = data.Packages[0].Models.Where(
                m => m.Type.Equals("Dialogue") || m.Type.Equals("FlowFragment"));

            var chaptersDictionary = new Dictionary<HexValue, Chapter>();
            var chapterNodes       = new Dictionary<HexValue, ChapterNode>();
            var chapterJumps       = new Dictionary<HexValue, HexValue>();
            var inputConnections   = new Dictionary<HexValue, Chapter>();

            // create new chapter assets
            foreach (var chapter in chapters)
            {
                
                var chapterName = chapter.Properties.DisplayName;
                var nodesDic    = new Dictionary<HexValue, string>();
                var newChapter  = ScriptableUtils.CreateAsset<Chapter>(chapFolder, $"Chapter_{chapterName}");

                newChapter.chapterName = chapterName;

                Debug.Log($"Create chapter asset '{chapter.Properties.Id}' : {chapter.Properties.DisplayName} -- {newChapter}");
                chaptersDictionary.Add(chapter.Properties.Id, newChapter);

                var entryNode      = ScriptableUtils.AddToAsset<EntryNode>(newChapter);
                var exitNode       = ScriptableUtils.AddToAsset<ExitNode>(newChapter);
                var backgroundNode = (ActiveNode) null;

                entryNode.id = GUID.Generate().ToString();
                exitNode.id  = GUID.Generate().ToString();
                entryNode.UpdateName();
                exitNode.UpdateName();
                newChapter.startNodeID = entryNode.id;

                if (chapter.Properties.Attachments.Length > 0)
                {
                    Debug.Log($"Handle attachment A: {chapter.Properties.Attachments[0]}");
                    var assetId = chapter.Properties.Attachments[0];
                    if (data.Locations.TryGetValue(assetId, out var location))
                    {
                        Debug.Log($"Handle attachment B: {location}");
                        if (data.Assets.TryGetValue(location.Properties.PreviewImage.Asset, out var asset))
                        {
                            Debug.Log($"Handle attachment C: {asset}");

                            var assetPath = Path.Combine(path, "Assets/Locations", Path.GetFileName(asset.AssetRef));
                            Debug.Log($"Handle attachment D: {assetPath}");

                            backgroundNode    = ScriptableUtils.AddToAsset<ActiveNode>(newChapter);
                            backgroundNode.id = GUID.Generate().ToString();
                            var setBackAction = ScriptableUtils.AddToAsset<BackgroundAction>(backgroundNode);
                            setBackAction.sprite   = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                            backgroundNode.actions = new List<BaseAction> {setBackAction};

                            if (setBackAction.sprite == null)
                                throw new System.Exception("You need to copy assets into {json-file-folder-name}/Assets/ folder, you dummy");

                            Debug.Log($"Handle attachment E: {setBackAction.sprite.name}");
                        }
                    }
                }

                var nodes = data.Packages[0].Models.Where(n => n.Properties.Parent.Equals(chapter.Properties.Id));

                // create nodes
                foreach (var node in nodes)
                {
                    BaseNode newNode = null;

                    switch (node.Type)
                    {
                        case "DialogueFragment":
                            newNode = ParseDialogueNode(characters, node, newChapter);
                            break;
                        case "Condition":
                            newNode = ParseConditionNode(node, newChapter);
                            break;
                        case "Instruction":
                            newNode = ParseInstructionNode(data, node, newChapter, path);
                            break;
                        case "Dialogue":
                            newNode = ScriptableUtils.AddToAsset<ChapterNode>(newChapter);

                            chapterNodes.Add(node.Properties.Id, newNode as ChapterNode);
                            break;
                        case "Jump":
                            chapterJumps.Add(node.Properties.Id, node.Properties.Target);
                            break;

                        default:
                            break;
                    }

                    if (newNode == null)
                    {
                        Debug.Log(node.Type);
                        continue;
                    }

                    newNode.id = GUID.Generate().ToString();
                    newNode.UpdateName();
                    newNode.position = ((Vector2) node.Properties.Position) * 1.4f;

                    nodesDic.Add(node.Properties.Id, newNode.id);
                    newChapter.AddNode(newNode);
                }

                // create node connections
                foreach (var node in nodes)
                {
                    var id = node.Properties.Id;
                    if (!nodesDic.ContainsKey(id))
                        continue;

                    var createdNodeId = nodesDic[id];
                    var createdNode   = newChapter.NodesByID[createdNodeId];

                    var connections = new List<Connection>();

                    if (node.Properties.OutputPins != null
                        && node.Properties.OutputPins.Count > 1)
                    {
                        // condition node
                        foreach (var pin in node.Properties.OutputPins)
                        {
                            if (pin.Connections != null)
                            {
                                connections.Add(pin.Connections[0]);
                            }
                        }
                    }
                    else
                    {
                        // other nodes

                        if (node.Properties.OutputPins != null
                            && node.Properties.OutputPins[0].Connections != null)
                        {
                            connections.AddRange(node.Properties.OutputPins[0].Connections);
                        }
                    }

                    //  Apply jumps
                    for (int i = 0; i < connections.Count; i++)
                    {
                        if (chapterJumps.TryGetValue(connections[i].Target, out var newTarget))
                            connections[i].Target = newTarget;
                    }

                    foreach (var connection in connections)
                    {
                        // check if connection.Target is NOT Dialogue
                        if (nodesDic.TryGetValue(connection.Target, out string targetNodeId))
                        {
                            var targetNode = newChapter.NodesByID[targetNodeId];

                            createdNode.nextNodes.Add(targetNodeId);

                            if (node.Type == "DialogueFragment" && connections.Count > 1)
                            {
                                if (nodes.Any(n => n.Properties.Id.Equals(connection.Target)))
                                {
                                    var connectedNode = nodes.First(n => n.Properties.Id.Equals(connection.Target));

                                    var choiceNode = createdNode as ChoiceNode;

                                    if (choiceNode.choices == null)
                                        choiceNode.choices = new List<ChoiceNode.Choice>();

                                    choiceNode.choices.Add(new ChoiceNode.Choice
                                    {
                                        localizedText = connectedNode.Properties.Text
                                    });
                                }
                            }
                        }
                        else
                        {
                            // meaning that we are trying to link to Parent Chapter
                            if (chaptersDictionary.TryGetValue(connection.Target, out var parentChapter))
                            {
                                createdNode.nextNodes.Add(exitNode.id);
                            }
                        }
                    }
                }
                
                // position entry node and exit node at the very ends of the chapter
                var leftMostNodePosition = Vector2.one * float.MaxValue;
                var leftMostNodeId       = string.Empty;

                foreach (var node in newChapter.nodes)
                {
                    if (node.position.x < leftMostNodePosition.x)
                    {
                        leftMostNodePosition = node.position;
                        leftMostNodeId       = node.id;
                    }
                }

                if (backgroundNode == null)
                {
                    entryNode.position  = leftMostNodePosition + new Vector2(-500f, 0f);
                    entryNode.nextNodes = new List<string> {leftMostNodeId};
                }
                else
                {
                    entryNode.position       = leftMostNodePosition + new Vector2(-900f, 0f);
                    entryNode.nextNodes      = new List<string> {backgroundNode.id};
                    backgroundNode.position  = leftMostNodePosition + new Vector2(-500f, 0f);
                    backgroundNode.nextNodes = new List<string> {leftMostNodeId};
                }

                // position entry node and exit node at the very ends of the chapter
                var rightMostNodePosition = Vector2.zero;

                foreach (var node in newChapter.nodes)
                {
                    if (node.position.x > rightMostNodePosition.x)
                        rightMostNodePosition = node.position;
                }

                exitNode.position = rightMostNodePosition + new Vector2(500f, 0f);

                newChapter.AddNode(entryNode);
                if (backgroundNode != null)
                    newChapter.AddNode(backgroundNode);
                newChapter.AddNode(exitNode);

                // fix choice nodes choices sorting
                var choiceNodes = newChapter.nodes
                    .Where(n => n is ChoiceNode)
                    .Select(n => n as ChoiceNode);

                foreach (var choiceNode in choiceNodes)
                {
                    if (newChapter.nodes.Any(n => choiceNode.nextNodes.Contains(n.id)))
                    {
                        var choicesMap    = new Dictionary<string, string>();
                        var choiceOutputs = newChapter.nodes.Where(n => choiceNode.nextNodes.Contains(n.id)).ToList();

                        for (int i = 0; i < choiceNode.choices.Count; i++)
                        {
                            choicesMap.Add(choiceNode.nextNodes[i], choiceNode.choices[i].localizedText);
                        }

                        choiceOutputs.Sort((a, b) => { return (int) ((a.position.y * 100f) - (b.position.y * 100f)); });

                        choiceNode.choices   = new List<ChoiceNode.Choice>();
                        choiceNode.nextNodes = new List<string>();

                        foreach (var choice in choiceOutputs)
                        {
                            choiceNode.choices.Add(new ChoiceNode.Choice
                            {
                                localizedText = choicesMap[choice.id]
                            });
                            choiceNode.nextNodes.Add(choice.id);
                        }
                    }
                }

                EditorUtility.SetDirty(newChapter);
                AssetDatabase.SaveAssets();

                chapterList.Add(newChapter);
            }

            // fill in chapter nodes
            foreach (var chapterNode in chapterNodes)
            {
                var node = chapterNode.Value;
                if (chaptersDictionary.TryGetValue(chapterNode.Key, out var chapter))
                {
                    node.chapter = chapter;
                }
            }

            void DumpChapters()
            {
                var sb = new StringBuilder();
                sb.AppendLine("Chapters:");
                foreach (var chapter in chapters)
                    sb.AppendLine($"  {chapter.Properties.Id} : {chapter.Properties.DisplayName}");
                sb.AppendLine();
                sb.AppendLine("Chapters assets:");
                foreach (var pair in chaptersDictionary)
                    sb.AppendLine($"  {pair.Key} : {pair.Value}");
                Debug.LogWarning(sb.ToString());
            }

            DumpChapters();

            // set each chapter asset parent chapter if needed
            foreach (var chapter in chapters)
            {
                if (chaptersDictionary.TryGetValue(chapter.Properties.Parent, out var parentChapter))
                {
                    var chapterAsset = chaptersDictionary[chapter.Properties.Id];
                    chapterAsset.parentChapter = parentChapter;

                    Debug.Log(
                        $"Assign parent '{parentChapter}' ({parentChapter != null}) for '{chapterAsset}' ({chapterAsset != null}) -- {chapter.Properties.Id}");

                    EditorUtility.SetDirty(chapterAsset);
                    AssetDatabase.SaveAssets();
                }
            }

            foreach (var chapterNode in chapterNodes.Values)
            {
                var node = chapterNode
                    .chapter
                    .nodes
                    .FirstOrDefault(n => n is ExitNode);

                if (node != null)
                {
                    var exitNode        = node as ExitNode;
                    var nextChapterNode = chapterNode.chapter.parentChapter.NodesByID[chapterNode.NextNode];

                    exitNode.nextNodes = new List<string> {nextChapterNode.id};
                }
            }

            return chapterList;
        }

        private static BaseNode ParseDialogueNode(
            Dictionary<HexValue, Character> characters,
            StoryNode                       node,
            Chapter                         newChapter)
        {
            BaseNode newNode;
            if (node.Properties.OutputPins != null
                && node.Properties.OutputPins[0].Connections != null
                && node.Properties.OutputPins[0].Connections.Count > 1)
            {
                newNode = ScriptableUtils.AddToAsset<ChoiceNode>(newChapter);
            }
            else
            {
                newNode = ScriptableUtils.AddToAsset<ReplicaNode>(newChapter);
            }

            var replica = newNode as ReplicaNode;

            replica.localized = string.IsNullOrEmpty(node.Properties.Text)
                ? node.Properties.MenuText
                : node.Properties.Text;

            replica.character = null;
            var speaker = node.Properties.Speaker;
            if (characters.ContainsKey(node.Properties.Speaker))
                replica.character = characters[speaker];

            return newNode;
        }

        private static BaseNode ParseConditionNode(
            StoryNode node,
            Chapter   newChapter)
        {
            BaseNode newNode;
            newNode = ScriptableUtils.AddToAsset<ConditionNode>(newChapter);

            var cond = newNode as ConditionNode;
            cond.rawCondition = node.Properties.Expression;

            return newNode;
        }

        private static readonly Regex _specialReg = new Regex(@"pause\(\""(?<assetName>[^""]+)""\);?");

        private static BaseNode ParseInstructionNode(
            ArticyData data,
            StoryNode  node,
            Chapter    newChapter,
            string     path)
        {
            BaseNode newNode;
            newNode = ScriptableUtils.AddToAsset<ActiveNode>(newChapter);

            var cond   = newNode as ActiveNode;
            var action = ScriptableUtils.AddToAsset<SetVariableAction>(newNode);
            action.rawExpression = node.Properties.Expression;

            cond.actions = new List<BaseAction> {action};

            var match = _specialReg.Match(action.rawExpression);
            if (match.Success)
            {
                action.rawExpression.Remove(match.Index, match.Length);
                var assetName = match.Groups["assetName"].Value;
                foreach (var asset in data.Assets)
                {
                    if (asset.Value.AssetRef.Contains(assetName))
                    {
                        var assetPath = Path.Combine(path, "Assets/Locations", Path.GetFileName(asset.Value.AssetRef));
                        var setBackAction = ScriptableUtils.AddToAsset<PauseAction>(newNode);
                        setBackAction.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                        cond.actions.Add(setBackAction);
                    }
                }
            }

            return newNode;
        }
    }
}