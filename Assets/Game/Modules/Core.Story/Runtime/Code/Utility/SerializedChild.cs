using System;

namespace Self.Story
{
    /// <summary>
    /// Used to detect child [SerializedObject]s 
    /// that are contained within a Chapter object
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializedChild : Attribute
    {

    }
}