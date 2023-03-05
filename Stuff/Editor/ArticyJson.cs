using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeekyHouse.Submodule.LocationDialogue
{
    public class Child
    {
        public string      Id            { get; set; }
        public string      TechnicalName { get; set; }
        public string      Type          { get; set; }
        public List<Child> Children      { get; set; }
    }

    public class ArticyColor
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }

        public string GetHtmlColor()
        {
            return ColorUtility.ToHtmlStringRGB(new Color(r, g, b, a));
        }
    }

    public class Connection
    {
        public ArticyColor Color { get; set; }
        public string Label { get; set; }
        public string TargetPin { get; set; }
        public string Target { get; set; }
    }

    public class Constraint
    {
        public string Property { get; set; }
        public string Type { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string Precision { get; set; }
        public string MinPrecision { get; set; }
        public string MaxPrecision { get; set; }
        public object Unit { get; set; }
        public string MaxLength { get; set; }
        public string PlaceholderValue { get; set; }
        public string IsLocalized { get; set; }
        public string AllowsLinebreaks { get; set; }
        public string SortMode { get; set; }
    }

    public class DisplayNames
    {
        public string Male { get; set; }
        public string Female { get; set; }
        public string Unknown { get; set; }
        public string Invalid { get; set; }
        public string Spot { get; set; }
        public string Circle { get; set; }
        public string Rectangle { get; set; }
        public string Path { get; set; }
        public string Polygon { get; set; }
        public string Link { get; set; }
        public string Unselectable { get; set; }
        public string Selectable { get; set; }
        public string Invisible { get; set; }
        public string Visible { get; set; }
        public string Solid { get; set; }
        public string Dot { get; set; }
        public string Dash { get; set; }
        public string DashDot { get; set; }
        public string DashDotDot { get; set; }
        public string ColoredDot { get; set; }
        public string None { get; set; }
        public string LineArrowHead { get; set; }
        public string FilledArrowHead { get; set; }
        public string Diamond { get; set; }
        public string Square { get; set; }
        public string Disc { get; set; }
        public string Small { get; set; }
        public string Medium { get; set; }
        public string Large { get; set; }
        public string FromAsset { get; set; }
        public string Custom { get; set; }
    }

    public class Feature
    {
        public string TechnicalName { get; set; }
        public string DisplayName { get; set; }
        public List<PropertyObj> Properties { get; set; }
        public List<Constraint> Constraints { get; set; }
    }

    public class GlobalVariable
    {
        public string Namespace { get; set; }
        public string Description { get; set; }
        public List<VariableObj> Variables { get; set; }
    }

    public class Hierarchy
    {
        public string Id { get; set; }
        public string TechnicalName { get; set; }
        public string Type { get; set; }
        public List<Child> Children { get; set; }
    }

    public class InputPin
    {
        public string Text { get; set; }
        public string Id { get; set; }
        public string Owner { get; set; }
        public List<Connection> Connections { get; set; }
    }

    public class Model
    {
        public                 string     Type       { get; set; }
        public                 Properties Properties { get; set; }
        [NonSerialized] public int        index;
        [NonSerialized] public bool       isChoice;
    }

    public class ObjectDefinition
    {
        public string Type { get; set; }
        public string Class { get; set; }
        public List<PropertyObj> Properties { get; set; }
        public string DisplayName { get; set; }
        public Values Values { get; set; }
        public DisplayNames DisplayNames { get; set; }
        public string InheritsFrom { get; set; }
        public Template Template { get; set; }
    }

    public class OutputPin
    {
        public string Text { get; set; }
        public string Id { get; set; }
        public string Owner { get; set; }
        public List<Connection> Connections { get; set; }
    }

    public class Package
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefaultPackage { get; set; }
        public List<Model> Models { get; set; }
    }

    public class Position
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class PreviewImage
    {
        public ViewBox ViewBox { get; set; }
        public string Mode { get; set; }
        public string Asset { get; set; }
    }

    public class Project
    {
        public string Name { get; set; }
        public string DetailName { get; set; }
        public string Guid { get; set; }
        public string TechnicalName { get; set; }
    }

    public class Properties
    {
        public string TechnicalName { get; set; }
        public string Id { get; set; }
        public string Parent { get; set; }
        public List<object> Attachments { get; set; }
        public string DisplayName { get; set; }
        public PreviewImage PreviewImage { get; set; }
        public ArticyColor Color { get; set; }
        public string Text { get; set; }
        public string ExternalId { get; set; }
        public Position Position { get; set; }
        public double ZIndex { get; set; }
        public Size Size { get; set; }
        public object ShortId { get; set; }
        public List<InputPin> InputPins { get; set; }
        public List<OutputPin> OutputPins { get; set; }
        public string MenuText { get; set; }
        public string StageDirections { get; set; }
        public string Speaker { get; set; }
        public double? SplitHeight { get; set; }
        public string Expression { get; set; }
        public string Property { get; set; }
        public string Type { get; set; }
        public string ItemType { get; set; }
    }

    public class Root
    {
        public Settings Settings { get; set; }
        public Project Project { get; set; }
        public List<GlobalVariable> GlobalVariables { get; set; }
        public List<ObjectDefinition> ObjectDefinitions { get; set; }
        public List<Package> Packages { get; set; }
        public List<object> ScriptMethods { get; set; }
        public Hierarchy Hierarchy { get; set; }
    }

    public class Settings
    {
        public string set_Localization { get; set; }
        public string set_TextFormatter { get; set; }
        public string set_IncludedNodes { get; set; }
        public string set_UseScriptSupport { get; set; }
        public string ExportVersion { get; set; }
    }

    public class Size
    {
        public double w { get; set; }
        public double h { get; set; }
    }

    public class Template
    {
        public string TechnicalName { get; set; }
        public string DisplayName { get; set; }
        public List<Feature> Features { get; set; }
    }

    public class Values
    {
        public int Male { get; set; }
        public int Female { get; set; }
        public int Unknown { get; set; }
        public int? Invalid { get; set; }
        public int? Spot { get; set; }
        public int? Circle { get; set; }
        public int? Rectangle { get; set; }
        public int? Path { get; set; }
        public int? Polygon { get; set; }
        public int? Link { get; set; }
        public int? Unselectable { get; set; }
        public int? Selectable { get; set; }
        public int? Invisible { get; set; }
        public int? Visible { get; set; }
        public int? Solid { get; set; }
        public int? Dot { get; set; }
        public int? Dash { get; set; }
        public int? DashDot { get; set; }
        public int? DashDotDot { get; set; }
        public int? ColoredDot { get; set; }
        public int? None { get; set; }
        public int? LineArrowHead { get; set; }
        public int? FilledArrowHead { get; set; }
        public int? Diamond { get; set; }
        public int? Square { get; set; }
        public int? Disc { get; set; }
        public int? Small { get; set; }
        public int? Medium { get; set; }
        public int? Large { get; set; }
        public int? FromAsset { get; set; }
        public int? Custom { get; set; }
    }

    public class VariableObj
    {
        public string Variable { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }

    public class ViewBox
    {
        public double x { get; set; }
        public double y { get; set; }
        public double w { get; set; }
        public double h { get; set; }
    }

    public class PropertyObj
    {
        public string Property { get; set; }
        public string Type { get; set; }
    }
}