using System.Threading.Tasks;
using GeekyHouse.Architecture.IOC;
using GeekyHouse.Subsystem.ConfigCommands;
using UnityEngine;

namespace GeekyHouse.Submodule.LocationDialogue
{
    public class PlayDialogueCommand : Command
    {
        [SerializeField]
        private DialogueConfig dialogueConfig;
        
        public override async Task Execute(Container ctx)
        {
            await ctx.Get<DialoguesManager>().RunDialogue(dialogueConfig);
        }
    }
}