using System.Text.RegularExpressions;
using Self.Architecture.IOC;
using Self.Story;
using UnityEngine;

namespace Self.Story
{
	public class SetVariableAction : BaseAction
	{
		private static readonly Regex _conditionReg =
			new Regex(
				@"(?<variable>[\w\.]*) *(?<operation>=|\+=|\-=|\*=|/=) *(?:(?<value>true|false|[\+\-]{0,1}\d+)|""(?<string>[^""]*)"")");

		[DisplayOnNode(1)] public string rawExpression;

		[Inject] private StoryController _StoryController;

		private VariablesContainer _variables;

		public override void Execute(BaseNode node)
		{
			_variables = _StoryController.CurrentChapter.book.variables;

			var expressions = rawExpression.Split(';');

			foreach (var exp in expressions)
				ExecuteExpression(exp);
		}

		private void ExecuteExpression(string expression)
		{
			// expressions
			var match = _conditionReg.Match(expression);
			if (match.Success)
			{
				var _var = match.Groups["variable"].Value;
				var _op  = match.Groups["operation"].Value;
				var _val = match.Groups["value"].Value;
				var _str = match.Groups["string"].Value;

				var variable = _variables.VariablesById[_var];

				if (variable is IntVariable intVariable)
				{
					if (!int.TryParse(_val, out var intValue))
						return;

					switch (_op)
					{
						case "+=":
							intVariable.value += intValue;
							break;
						case "-=":
							intVariable.value -= intValue;
							break;
						case "*=":
							intVariable.value *= intValue;
							break;
						case "/=":
							intVariable.value /= intValue;
							break;
						default:
                            if (_val.StartsWith('+')) {
                                intVariable.value += intValue;
                            }
                            else if (_val.StartsWith('-')) {
                                intVariable.value -= intValue;
                            }
                            else {
                                intVariable.value = intValue;
                            }
							break;
					}
				}
				else if (variable is BoolVariable boolVariable)
				{
					if (!bool.TryParse(_val, out var boolValue))
						return;
					if (!_op.Equals("="))
					{
						Debug.LogWarning("Wrong operation with bool!");
						return;
					}

					boolVariable.value = boolValue;
				}
				else if (variable is StringVariable stringVariable)
				{
					switch (_op)
					{
						case "=":
							stringVariable.value = _str;
							break;
						case "+=":
							stringVariable.value += _str;
							break;
						default:
							Debug.LogWarning("Wrong operation with string!");
							break;
					}
				}
			}
		}
	}
}