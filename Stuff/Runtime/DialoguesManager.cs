using System.Collections.Generic;
using System.Threading.Tasks;
using GeekyHouse.Architecture.IOC;
using GeekyHouse.Subsystem.ConfigCommands;
using GeekyHouse.Subsystem.IsometricCamera;
using GeekyHouse.Subsystem.UI;
using UnityEngine;

namespace GeekyHouse.Submodule.LocationDialogue
{
    public class DialoguesManager : SharedObject
    {
        [Inject] private UIManager              _uiManager;
        [Inject] private GlobalFlagsManager     _globalFlagsManager;
        [Inject] private UICinematicManager     _cinematicManager;
        [Inject] private IsometricCameraManager _isometricCamera;
        
        private TaskCompletionSource<bool> _completionTask;

        private DialogueConfig _currentDialogueConfig;
        private DialogueNode   _currentDialogueNode;

        private DialogueUI _dialogueUI;

        private Dictionary<string, DialogueValue> _localValues;

        private bool exitCinematic;
        
        public override void Init()
        {
            _uiManager.UILoaded += HandleUILoaded;
        }

        public override void Dispose()
        {
            _uiManager.UILoaded -= HandleUILoaded;
        }

        private void HandleUILoaded()
        {
            _dialogueUI               =  GameObject.FindObjectOfType<DialogueUI>();
            _dialogueUI.onChoiceClick += HandleChoiceClick;
            _dialogueUI.onNextClick   += HandleNextClick;
            _dialogueUI.onSkipClick   += HandleSkipClick;
        }

        public Task<bool> RunDialogue(DialogueConfig dialogueConfig)
        {
            _completionTask = new TaskCompletionSource<bool>();

            if (!_cinematicManager.IsCinematic)
            {
                _cinematicManager.EnterCinematic();
                _isometricCamera.LockBy(this);
                exitCinematic = true;
            }
            else
            {
                exitCinematic = false;
            }

            _currentDialogueConfig = dialogueConfig;
            _currentDialogueNode   = _currentDialogueConfig.nodes[int.Parse(_currentDialogueConfig.firstNode)];

            _localValues = new Dictionary<string, DialogueValue>();
            
            _dialogueUI.SetCharacters(_currentDialogueConfig.characters);
            FindNextNodeAndRun(_currentDialogueConfig.firstNode);

            return _completionTask.Task;
        }

        private void ExitDialogue()
        {
            _dialogueUI.Hide();
            _completionTask.SetResult(true);

            if (exitCinematic)
            {
                _cinematicManager.ExitCinematic();
                _isometricCamera.FreeBy(this);
            } 
        }

        private void FindNextNodeAndRun(string id = null)
        {
            if (CurrentNodeIsLust())
            {
                ExitDialogue();
                return;
            }
            
            if (id != null)
            {
                _currentDialogueNode = _currentDialogueConfig.nodes[int.Parse(id)];
            }
            else
            {
                DialogueNodeWithNext nodeWithNext = _currentDialogueNode as DialogueNodeWithNext;
                _currentDialogueNode = _currentDialogueConfig.nodes[int.Parse(nodeWithNext.nextNode)];
            }
            
            while (true)
            {
                if (_currentDialogueNode is DialogueNodeMonologueWithChoices nodeWithChoices)
                {
                    nodeWithChoices.nodeChoices = new List<DialogueNodeTalkChoice>();
                    
                    foreach (var choiceId in nodeWithChoices.choices)
                    {
                        DialogueNodeTalkChoice nodeChoice =
                            _currentDialogueConfig.nodes[int.Parse(choiceId)] as DialogueNodeTalkChoice;
                        nodeWithChoices.nodeChoices.Add(nodeChoice);
                    }
                    
                    break;
                }
                
                if (_currentDialogueNode is DialogueNodeMonologue)
                {
                    break;
                }

                if (_currentDialogueNode is DialogueNodeInstruction instruction)
                {
                    if (instruction.localValues != null)
                        foreach (var localValue in instruction.localValues)
                            _localValues[localValue.name] = localValue;
                    
                    if (instruction.globalFlags != null)
                        foreach (var globalFlag in instruction.globalFlags)
                        {
                            _globalFlagsManager[globalFlag.name] = globalFlag.value;
                        }

                    if (CurrentNodeIsLust())
                    {
                        ExitDialogue();
                        return;
                    }
                    
                    _currentDialogueNode = _currentDialogueConfig.nodes[int.Parse(instruction.nextNode)];
                    
                    continue;
                }

                if (_currentDialogueNode is DialogueNodeConditionLocal conditionLocal)
                {
                    bool result = false;

                    if (_localValues.ContainsKey(conditionLocal.value.name))
                        result = _localValues[conditionLocal.value.name] == conditionLocal.value;
                    
                    if (result)
                        _currentDialogueNode = _currentDialogueConfig.nodes[int.Parse(conditionLocal.nodeTrue)];
                    else
                        _currentDialogueNode = _currentDialogueConfig.nodes[int.Parse(conditionLocal.nodeFalse)];
                }
                
                if (_currentDialogueNode is DialogueNodeConditionGlobal conditionGlobal)
                {
                    bool result = false;
                    
                    if (conditionGlobal.value != null)
                    {
                        result = _globalFlagsManager[conditionGlobal.value.name] == conditionGlobal.value.value;
                    }
                    
                    if (result)
                        _currentDialogueNode = _currentDialogueConfig.nodes[int.Parse(conditionGlobal.nodeTrue)];
                    else
                        _currentDialogueNode = _currentDialogueConfig.nodes[int.Parse(conditionGlobal.nodeFalse)];
                }
            }
            
            RunCurrentNode();
        }

        private bool CurrentNodeIsLust()
        {
            return _currentDialogueConfig.lastNodes.Contains(_currentDialogueNode.id);
        }

        private void RunCurrentNode()
        {
            _dialogueUI.SetNode(_currentDialogueNode);
        }

        private void HandleNextClick()
        {
            FindNextNodeAndRun();
        }

        private void HandleChoiceClick(DialogueNodeTalkChoice choice)
        {
            FindNextNodeAndRun(choice.nextNode);
        }

        private void HandleSkipClick()
        {
            ExitDialogue();
        }
    }
}