namespace Self.Story
{
    public enum VariableType
    {
        Float,
        Int,
        Bool,
        String
    }

    [System.Serializable]
    public class VariableData<T>
    {
        public string id;
        public T value;
    }

    public class FloatVariableData : VariableData<float> { }
    public class IntVariableData : VariableData<int> { }
    public class BoolVariableData : VariableData<bool> { }
    public class StringVariableData : VariableData<string> { }    
}