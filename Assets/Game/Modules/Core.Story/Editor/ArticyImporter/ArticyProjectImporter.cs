using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Self.Story;
using Self.Story.Editors;
using System.IO;
using System.Text.RegularExpressions;

namespace Self.ArticyImporter
{
	public static class ArticyProjectImporter
	{
		public static ArticyData ImportFromJsonAsset(TextAsset textAsset)
		{
			return JsonConvert.DeserializeObject<ArticyData>(textAsset.text);
		}

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

		private static void ImportFromArticy(string file)
		{
			if (string.IsNullOrEmpty(file))
				throw new System.Exception("Could not open file, path is null or empty");

			var relativeFilePath = $"Assets/{file.Split("Assets/")[1]}";
			var jsonAsset        = AssetDatabase.LoadAssetAtPath<TextAsset>(relativeFilePath);

			if (jsonAsset == null)
				throw new System.Exception($"Error loading asset at path {relativeFilePath}");

			var data       = ImportFromJsonAsset(jsonAsset);
			var storyNodes = new Dictionary<ulong, StoryNode>();

			data.Packages[0].Models.ForEach(m => storyNodes.Add(m.Properties.Id.Value, m));

			var creationPath = System.IO.Path.GetDirectoryName(relativeFilePath);
			var characters   = CreateCharactersFromImport(data, creationPath);

			var book = CreateBookFromImport(data, creationPath);
			book.characters = characters.Values.ToList();

			var chapters = CreateChaptersFromImport(book, data, characters, creationPath);

			foreach (var chapter in chapters)
			{
				chapter.book = book;
			}

			AssetDatabase.SaveAssets();
		}

		private static Dictionary<HexValue, Character> CreateCharactersFromImport(ArticyData data, string path)
		{
			var characters = data.Packages[0].Models.Where(m => m.Type.Equals("DefaultMainCharacterTemplate")
			                                                    || m.Type.Equals("DefaultSupportingCharacterTemplate"));

			path = $"{path}/Characters/";

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			var characterList = new Dictionary<HexValue, Character>();

			foreach (var character in characters)
			{
				var newCharacter =
					ScriptableUtils.CreateAsset<Character>(path, $"Character_{character.Properties.DisplayName}");
				newCharacter.isMainCharacter = (character.Type.Equals("DefaultMainCharacterTemplate")) ? true : false;
				newCharacter.characterName   = character.Properties.DisplayName;

				EditorUtility.SetDirty(newCharacter);
				AssetDatabase.SaveAssets();

				characterList.Add(character.Properties.Id, newCharacter);
			}

			return characterList;
		}

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
			path = $"{path}/Chapters/";

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			var chapterList = new List<Chapter>();
			var chapters = data.Packages[0].Models.Where(m => m.Type.Equals("Dialogue")
			                                                  || m.Type.Equals("FlowFragment"));

			var chaptersDictionary = new Dictionary<HexValue, Chapter>();
			var chapterNodes       = new Dictionary<HexValue, ChapterNode>();
			var inputConnections   = new Dictionary<HexValue, Chapter>();

			// create new chapter assets
			foreach (var chapter in chapters)
			{
				var chapterName = chapter.Properties.DisplayName;
				var nodesDic    = new Dictionary<HexValue, string>();
				var newChapter  = ScriptableUtils.CreateAsset<Chapter>(path, $"Chapter_{chapterName}");

				newChapter.chapterName = chapterName;

				chaptersDictionary.Add(chapter.Properties.Id, newChapter);

				var entryNode = ScriptableUtils.AddToAsset<EntryNode>(newChapter);
				var exitNode  = ScriptableUtils.AddToAsset<ExitNode>(newChapter);

				entryNode.id = GUID.Generate().ToString();
				exitNode.id  = GUID.Generate().ToString();
				entryNode.UpdateName();
				exitNode.UpdateName();
				newChapter.startNodeID = entryNode.id;

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
							newNode = ParseInstructionNode(node, newChapter);
							break;
						case "Dialogue":
							newNode = ScriptableUtils.AddToAsset<ChapterNode>(newChapter);

							chapterNodes.Add(node.Properties.Id, newNode as ChapterNode);
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
										localizedText = connectedNode.Properties.MenuText
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

				EditorUtility.SetDirty(newChapter);
				AssetDatabase.SaveAssets();

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

				entryNode.position  = leftMostNodePosition + new Vector2(-500f, 0f);
				entryNode.nextNodes = new List<string> {leftMostNodeId};

				// position entry node and exit node at the very ends of the chapter
				var rightMostNodePosition = Vector2.zero;

				foreach (var node in newChapter.nodes)
				{
					if (node.position.x > rightMostNodePosition.x)
						rightMostNodePosition = node.position;
				}

				exitNode.position = rightMostNodePosition + new Vector2(500f, 0f);

				newChapter.AddNode(entryNode);
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

			// set each chapter asset parent chapter if needed
			foreach (var chapter in chapters)
			{
				if (chaptersDictionary.TryGetValue(chapter.Properties.Parent, out var parentChapter))
				{
					var chapterAsset = chaptersDictionary[chapter.Properties.Id];
					chapterAsset.parentChapter = parentChapter;

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
			cond.nextNodes = new List<string>(2) { string.Empty, string.Empty };
			cond.rawCondition = node.Properties.Expression;

			return newNode;
		}

		private static BaseNode ParseInstructionNode(
			StoryNode node,
			Chapter   newChapter)
		{
			BaseNode newNode;
			newNode = ScriptableUtils.AddToAsset<ActiveNode>(newChapter);

			var cond   = newNode as ActiveNode;
			var action = ScriptableUtils.AddToAsset<SetVariableAction>(newChapter);
			action.rawExpression = node.Properties.Expression;

			cond.actions = new List<BaseAction> {action};

			return newNode;
		}
	}
}