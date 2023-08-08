using System;
using System.Collections.Generic;
using UnityEngine;

namespace Self.ArticyImporter
{
	[Serializable]
	public class ArticyData
	{
		public ArticyProject            Project;
		public List<ArticyVariablesSet> GlobalVariables;
		public List<ArticyPackageData>  Packages;
		public HierarchyNode            Hierarchy;

		public Dictionary<HexValue, StoryNode> Assets;
		public Dictionary<HexValue, StoryNode> Locations;

		public void InitAssets()
		{
			Assets    = new Dictionary<HexValue, StoryNode>();
			Locations = new Dictionary<HexValue, StoryNode>();

			foreach (var package in Packages)
			foreach (var node in package.Models)
			{
				if (node.Type == "Asset")
					Assets.Add(node.Properties.Id, node);
				if (node.Type == "Location")
					Locations.Add(node.Properties.Id, node);
			}

			Debug.Log($"Assets count: {Assets.Count}");
			Debug.Log($"Locations count: {Locations.Count}");
		}
	}

	[Serializable]
	public class HierarchyNode
	{
		public HexValue            Id;
		public string              TechnicalName;
		public string              Type;
		public List<HierarchyNode> Children;
	}

#region Project

	[System.Serializable]
	public class ArticyProject
	{
		public string Name;
		public string DetailName;
		public string Guid;
		public string TechnicalName;
	}

#endregion

#region Variables

	[Serializable]
	public class ArticyVariablesSet
	{
		public string               Namespace;
		public string               Description;
		public List<ArticyVariable> Variables;
	}

	[Serializable]
	public class ArticyVariable
	{
		public string Variable; // name
		public string Type;
		public string Value;
		public string Description;
	}

#endregion

#region Packages

	[Serializable]
	public class ArticyPackageData
	{
		public string          Name;
		public string          Description;
		public bool            IsDefaultPackage;
		public List<StoryNode> Models;
	}

	[Serializable]
	public class StoryNode
	{
		public string         Type;
		public string         AssetRef;
		public NodeProperties Properties;
	}

	[Serializable]
	public class NodeProperties
	{
		public string        TechnicalName;
		public HexValue      Id;
		public HexValue      Parent;
		public HexValue      Target;
		public HexValue[]    Attachments;
		public string        MenuText;
		public string        StageDirections;
		public string        DisplayName;
		public PreviewImage  PreviewImage;
		public float         SplitHeight;
		public Color         Color;
		public HexValue      Speaker;
		public string        Text;
		public string        ExternalId;
		public string        Expression;
		public Position      Position;
		public float         ZIndex;
		public Size          Size;
		public uint          ShortId;
		public List<NodePin> InputPins;
		public List<NodePin> OutputPins;
	}

	[Serializable]
	public class PreviewImage
	{
		public ViewBox  ViewBox;
		public string   Mode;
		public HexValue Asset;
	}

	[Serializable]
	public struct ViewBox
	{
		public float x, y, w, h;

		public ViewBox(float x, float y, float w, float h)
		{
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}

		public override string ToString()
		{
			return $"[x:{x}, y:{y}, w:{w}, h:{h}]";
		}

		public static explicit operator ViewBox(Rect rect) => new ViewBox(rect.x, rect.y, rect.width, rect.height);
		public static explicit operator Rect(ViewBox vb)   => new Rect(vb.x, vb.y, vb.w, vb.h);
	}

	[Serializable]
	public struct Position
	{
		public float x, y;

		public Position(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return $"[x:{x}, y:{y}]";
		}

		public static explicit operator Vector2(Position p) => new Vector2(p.x, p.y);
		public static explicit operator Position(Vector2 v) => new Position(v.x, v.y);
	}

	[Serializable]
	public struct Size
	{
		public float w, h;

		public Size(float w, float h)
		{
			this.w = w;
			this.h = h;
		}

		public override string ToString()
		{
			return $"[w:{w}, h:{h}]";
		}

		public static explicit operator Vector2(Size s) => new Vector2(s.w, s.h);
		public static explicit operator Size(Vector2 v) => new Size(v.x, v.y);
	}

	[Serializable]
	public struct HexValue : IEquatable<ulong>, IEquatable<HexValue>
	{
		public ulong Value;

		public static explicit operator string(HexValue value) => value.ToString();
		public static explicit operator HexValue(string from)  => new HexValue {Value = Convert.ToUInt64(from, 16)};

		public override string ToString()
		{
			return $"0x{Value.ToString("X16")}";
		}

		public bool Equals(ulong    other) => Value.Equals(other);
		public bool Equals(HexValue other) => Value.Equals(other.Value);
	}

	[Serializable]
	public class NodePin
	{
		public string           Text;
		public HexValue         Id;
		public HexValue         Owner;
		public List<Connection> Connections;
	}

	[Serializable]
	public class Connection
	{
		public Color    Color;
		public string   Label;
		public HexValue TargetPin;
		public HexValue Target;
	}

#endregion
}