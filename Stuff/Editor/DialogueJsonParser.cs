#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeekyHouse.Architecture.CSV;
using GeekyHouse.Subsystem.Save;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace GeekyHouse.Submodule.LocationDialogue
{
    public class DialogueJsonParser
    {
        private static readonly char[] TRIM_CHARS = {' ', '\r', '\n', '\t'};
        
        private const string LOCAL_VARIABLES_NAME = "LocalVariables";
        private const string GLOBAL_FLAGS_NAME = "GlobalFlags";
        private const string SLOTS_NAME = "Slots";
        private const string DIALOGUE = "Dialogue";
        private const string INSTRUCTION = "Instruction";
        private const string DIALOGUE_FRAGMENT = "DialogueFragment";
        private const string CONDITION = "Condition";

        private const string LOCALIZATION_LOCATION = "GeekyHouse/Modules/Localization/Settings/Dialogues.csv";
        
        [MenuItem("Assets/[GeekyHouse Actions]/Dialogue/ParseFromJson")]
        private static void Parse()
        {
            string json = (Selection.activeObject as TextAsset).text;
            
            string directoryPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
            directoryPath = directoryPath.Substring(0, directoryPath.LastIndexOf('/') + 1);
            
            Root   root = JsonConvert.DeserializeObject<Root>(json);

            List<DialogueConfig> _dialogues = new List<DialogueConfig>();

            Dictionary<Model, List<Model>> jsonDialogues = new Dictionary<Model, List<Model>>();
            Dictionary<string, string> characterById = new Dictionary<string, string>();
            Dictionary<string, string> variableTypeByName = new Dictionary<string, string>();

            GlobalVariable localVariables = root.GlobalVariables.Find(e => e.Namespace == LOCAL_VARIABLES_NAME);
            
            foreach (var variable in localVariables.Variables)
                variableTypeByName[variable.Variable] = variable.Type;
            
            foreach (var package in root.Packages)
            {
                foreach (var mainModel in package.Models.FindAll(e => e.Type == DIALOGUE))
                {
                    string dialogueID = mainModel.Properties.Id;
                    
                    jsonDialogues.Add(mainModel, package.Models.FindAll(e => e.Type != DIALOGUE && e.Properties.Parent == dialogueID));
                }
                
                foreach (var characterModel in package.Models.FindAll(e => e.Type == "Entity"))
                    characterById[characterModel.Properties.Id] = characterModel.Properties.DisplayName;
            }

            List<string[]> localizationCSV   = new List<string[]>();

            string filePath = Path.Combine(Application.dataPath, LOCALIZATION_LOCATION);
            if (File.Exists(filePath))
            {
                localizationCSV = CSVUtils.FromString(File.ReadAllText(filePath));
                var header = localizationCSV[0];
                if (header[0] == "Term")
                {
                    localizationCSV.RemoveAt(0);
                    var line = string.Join(", ", header);
                    Debug.Log($"localizationCSV: {line}");
                }
            }
            
            Queue<string> branchesQueue   = new Queue<string>();
            List<Model>   allModels       = new List<Model>();
            
            foreach (var jsonDialogue in jsonDialogues)
            {
                DialogueConfig     dialogue = null;
                
                string name = $"{jsonDialogue.Key.Properties.DisplayName}.asset";
                string fullName = directoryPath + name;
                
                dialogue = ScriptableObject.CreateInstance<DialogueConfig>();
                
                List<DialogueNode> nodes = new List<DialogueNode>();
                
                _dialogues.Add(dialogue);

                dialogue.id = jsonDialogue.Key.Properties.DisplayName;

                string currentModuleId = jsonDialogue.Key.Properties.InputPins[0].Connections[0].Target;

                jsonDialogue.Value.Find(e => e.Properties.Id == currentModuleId).index = 0;

                dialogue.firstNode = currentModuleId;
                
                branchesQueue.Clear();
                allModels = jsonDialogue.Value;

                while (true)
                {
                    Model currentModel = jsonDialogue.Value.Find(e => e.Properties.Id == currentModuleId);

                    switch (currentModel.Type)
                    {
                        case INSTRUCTION:
                            ParseInstruction(dialogue, currentModel, nodes);
                            break;
                        case DIALOGUE_FRAGMENT:
                            ParseFragment(dialogue, currentModel, nodes);
                            break;
                        case CONDITION:
                            ParseCondition(dialogue, currentModel, nodes);
                            break;
                    }

                    if (!dialogue.lastNodes.Contains(currentModel.Properties.Id))
                    {
                        currentModuleId = currentModel.Properties.OutputPins[0].Connections[0].Target;
                        
                        if (!nodes.Any(e => e.id == currentModuleId))
                            continue;
                    }
                    
                    if (branchesQueue.Count == 0)
                        break;

                    currentModuleId = branchesQueue.Dequeue();
                }
                
                nodes.Sort((n1, n2) => 
                    allModels.Find(e => e.Properties.Id == n1.id).index - allModels.Find(e => e.Properties.Id == n2.id).index);

                Dictionary<string, int> idByArticyId = new Dictionary<string, int>();

                for (int index = 0; index < nodes.Count; index++)
                {
                    DialogueNode node         = nodes[index];
                    dialogue.nodes[index + 1] = node;
                    idByArticyId[node.id]     = index + 1;
                }

                string nodesJson     = JsonConvert.SerializeObject(dialogue.nodes);
                string lastNodesJson = JsonConvert.SerializeObject(dialogue.lastNodes);

                foreach (var pair in idByArticyId)
                {
                    nodesJson     = nodesJson.Replace(pair.Key, pair.Value.ToString());
                    lastNodesJson = lastNodesJson.Replace(pair.Key, pair.Value.ToString());
                }

                dialogue.nodes = 
                    JsonConvert.DeserializeObject<Dictionary<int, DialogueNode>>(nodesJson, SaveCreationConverter.Instance);
                dialogue.lastNodes = 
                    JsonConvert.DeserializeObject<List<string>>(lastNodesJson, SaveCreationConverter.Instance);
                dialogue.firstNode = 1.ToString();

                Dictionary<string, string> characterIdByName = new Dictionary<string, string>();
                foreach (var character in dialogue.characters)
                {
                    string characterId = $"character/{character.name}";
                    characterIdByName[character.name] = characterId;
                    SetLocalizationLine(characterId, character.name);
                    character.name = characterId;
                }

                foreach (var node in dialogue.nodes)
                {
                    switch (node.Value)
                    {
                        case DialogueNodeMonologueWithChoices withChoices:
                            string id = $"dialogue/{dialogue.id}/replica_{withChoices.id}";
                            SetLocalizationLine(id, withChoices.text);
                            withChoices.text            = id;
                            withChoices.activeCharacter = characterIdByName[withChoices.activeCharacter];
                            foreach (var choiceId in withChoices.choices)
                            {
                                DialogueNodeTalkChoice choice =
                                    dialogue.nodes[int.Parse(choiceId)] as DialogueNodeTalkChoice;
                                
                                string choiceTextId = id + $"/choice_{choiceId}";
                                SetLocalizationLine(choiceTextId, choice.text);
                                choice.text = choiceTextId;
                            }

                            break;
                        case DialogueNodeMonologue monologue:
                            string monologueTextId = $"dialogue/{dialogue.id}/replica_{monologue.id}";
                            SetLocalizationLine(monologueTextId, monologue.text);
                            monologue.text            = monologueTextId;
                            monologue.activeCharacter = characterIdByName[monologue.activeCharacter];
                            break;
                    }
                }
                
                AssetDatabase.CreateAsset(dialogue, fullName);
                AssetDatabase.SaveAssets();
            }
            
            // localizationCSV.Sort((s1, s2) => string.Compare(s1[0], s2[0]));
            // Term,Description,English,Russian
            localizationCSV.Insert(0, new []{"Term", "Description", "English", "Russian"});
            string localizationString = CSVUtils.ToString(localizationCSV);
            File.WriteAllText(filePath, localizationString);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            void SetLocalizationLine(string id, string text)
            {
                string[] localizationLine =
                    localizationCSV.FirstOrDefault(e => e != null && e.Length == 4 && e[0] == id);

                if (localizationLine == null)
                {
                    localizationLine = new string[4];
                    localizationCSV.Add(localizationLine);
                }

                localizationLine[0] = id.Trim(TRIM_CHARS);
                localizationLine[1] = "";
                // Сделал так потому что в бэту мы не делаем несколько языков а оставлять строки пустыми - неудобно для тестов
                localizationLine[2] = text.Trim(TRIM_CHARS);
                localizationLine[3] = text.Trim(TRIM_CHARS);
            }

            void ParseInstruction(DialogueConfig dialogue, Model model, List<DialogueNode> nodes)
            {
                DialogueNodeInstruction nodeInstruction = new DialogueNodeInstruction();
                nodeInstruction.id = model.Properties.Id;

                if (!CheckLastNode(dialogue, model))
                {
                    nodeInstruction.nextNode = model.Properties.OutputPins[0].Connections[0].Target;
                    allModels.Find(e => e.Properties.Id == nodeInstruction.nextNode).index = model.index + 1;
                }
                
                nodes.Add(nodeInstruction);
                
                string expression = model.Properties.Expression.Replace(" ", "");
                expression = expression.Replace("\n", "");

                string[] fields = expression.Split(';');

                foreach (string field in fields)
                {
                    if (string.IsNullOrEmpty(field))
                        continue;
                    
                    string[] typeNameValue = field.Split('=');
                    string[] typeName = typeNameValue[0].Split('.');
                    string type = typeName[0];
                    string name = typeName[1];
                    string value = typeNameValue[1];

                    switch (type)
                    {
                        case LOCAL_VARIABLES_NAME:
                            var variableType = variableTypeByName[name];
                            var localValue = new DialogueValue(variableType, name, value);
                            nodeInstruction.localValues.Add(localValue);
                            break;
                        case GLOBAL_FLAGS_NAME:
                            var flag = new DialogueGlobalFlag {name = name, value = bool.Parse(value)};
                            nodeInstruction.globalFlags.Add(flag);
                            break;
                        case SLOTS_NAME:
                            var character = new DialogueCharacter(value.Split('.')[1],name);
                            dialogue.characters.Add(character);
                            break;
                    }
                }
            }

            void ParseFragment(DialogueConfig dialogue, Model currentModel, List<DialogueNode> nodes)
            {
                bool isLastNode = CheckLastNode(dialogue, currentModel);
                
                if (currentModel.isChoice)
                {
                    DialogueNodeTalkChoice choise = new DialogueNodeTalkChoice();
                    choise.id   = currentModel.Properties.Id;
                    choise.text = currentModel.Properties.Text;

                    if (!isLastNode)
                    {
                        choise.nextNode = currentModel.Properties.OutputPins[0].Connections[0].Target;
                        allModels.Find(e => e.Properties.Id == choise.nextNode).index = currentModel.index + 1;
                    }
                    
                    nodes.Add(choise);
                    
                    return;
                }
                
                if (!isLastNode && currentModel.Properties.OutputPins[0].Connections.Count > 1)
                {
                    DialogueNodeMonologueWithChoices monologueWithChoices = new DialogueNodeMonologueWithChoices();
                    monologueWithChoices.text            = currentModel.Properties.Text;
                    monologueWithChoices.activeCharacter = characterById[currentModel.Properties.Speaker];
                    monologueWithChoices.emotionColor    = currentModel.Properties.Color.GetHtmlColor();
                    monologueWithChoices.id              = currentModel.Properties.Id;
                    
                    nodes.Add(monologueWithChoices);

                    for (var i = 0; i < currentModel.Properties.OutputPins[0].Connections.Count; i++)
                    {
                        Connection connection = currentModel.Properties.OutputPins[0].Connections[i];

                        Model choise = allModels.Find(e => e.Properties.Id == connection.Target);
                        choise.isChoice = true;
                        choise.index    = currentModel.index + 1;
                        
                        monologueWithChoices.choices.Add(connection.Target);
                        
                        if (i != 0)
                            branchesQueue.Enqueue(connection.Target);
                    }
                    
                    return;
                }

                DialogueNodeMonologue monologue = new DialogueNodeMonologue();
                monologue.text            = currentModel.Properties.Text;
                monologue.activeCharacter = characterById[currentModel.Properties.Speaker];
                monologue.emotionColor    = currentModel.Properties.Color.GetHtmlColor();
                monologue.id              = currentModel.Properties.Id;
                if (!isLastNode)
                {
                    monologue.nextNode = currentModel.Properties.OutputPins[0].Connections[0].Target;
                    allModels.Find(e => e.Properties.Id == monologue.nextNode).index = currentModel.index + 1;
                }
                
                nodes.Add(monologue);
            }

            void ParseCondition(DialogueConfig dialogue, Model model, List<DialogueNode> nodes)
            {
                string nodeTrue = model.Properties.OutputPins[0].Connections[0].Target;
                string nodeFalse = model.Properties.OutputPins[1].Connections[0].Target;

                allModels.Find(e => e.Properties.Id == nodeTrue).index = model.index + 1;
                allModels.Find(e => e.Properties.Id == nodeFalse).index = model.index + 1;
                
                branchesQueue.Enqueue(nodeFalse);
                
                string expression = model.Properties.Expression.Replace(" ", "");
                expression = expression.Replace("\n", "");
                expression = expression.Replace(";", "");

                string[] typeNameValue = expression.Split(new[] {"=="}, StringSplitOptions.None);
                string[] typeName = typeNameValue[0].Split('.');
                string type = typeName[0];
                string name = typeName[1];
                string value = typeNameValue[1];
                
                switch (type)
                {
                    case LOCAL_VARIABLES_NAME:
                        var variableType = variableTypeByName[name];
                        var localValue = new DialogueValue(variableType, name, value);
                        
                        var dialogueNodeConditionLocal = new DialogueNodeConditionLocal();
                        dialogueNodeConditionLocal.value     = localValue;
                        dialogueNodeConditionLocal.nodeTrue  = nodeTrue;
                        dialogueNodeConditionLocal.nodeFalse = nodeFalse;
                        dialogueNodeConditionLocal.id        = model.Properties.Id;
                        
                        nodes.Add(dialogueNodeConditionLocal);
                        break;
                    case GLOBAL_FLAGS_NAME:
                        var flag = new DialogueGlobalFlag {name = name, value = bool.Parse(value)};
                        
                        var dialogueNodeConditionGlobal = new DialogueNodeConditionGlobal();
                        dialogueNodeConditionGlobal.value     = flag;
                        dialogueNodeConditionGlobal.nodeTrue  = nodeTrue;
                        dialogueNodeConditionGlobal.nodeFalse = nodeFalse;
                        dialogueNodeConditionGlobal.id        = model.Properties.Id;

                        nodes.Add(dialogueNodeConditionGlobal);
                        break;
                }
            }

            bool CheckLastNode(DialogueConfig dialogue, Model model)
            {
                if (model.Properties.OutputPins[0].Connections == null ||
                    model.Properties.OutputPins[0].Connections.Count == 0 ||
                    allModels.Find(e => e.Properties.Id == model.Properties.OutputPins[0].Connections[0].Target) == null
                    )
                {
                    dialogue.lastNodes.Add(model.Properties.Id);
                    return true;
                }

                return false;
            }
        }

        [MenuItem("Assets/[GeekyHouse Actions]/Dialogue/ParseFromJson", true)]
        private static bool CheckJson()
        {
            if (Selection.activeObject != null)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                return path.Contains(".json");
            }
            
            return false;
        }
    }
}
#endif