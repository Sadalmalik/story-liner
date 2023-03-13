using System;
using UnityEngine;

namespace Self.StoryV2
{
    public enum VariableType
    {
        Float,
        Int,
        Bool,
        String
    }

    public abstract class Variable : ScriptableObject
    {
        public string id;

        public abstract void SetValue(object value);
        public abstract object GetValue();
    }

    public class FloatVariable : Variable 
    {
        public float value;
        public float minValue = 0f;
        public float maxValue = 100f;

        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object value)
        {
            if (value == null)
                throw new Exception($"[{nameof(FloatVariable)}.SetValue] Tryint to set a null value as float!");

            try
            {
                var convertedValue = Convert.ToSingle(value);

                this.value = Mathf.Clamp(convertedValue, minValue, maxValue);
            }
            catch (Exception)
            {
                throw new InvalidCastException($"[{nameof(FloatVariable)}.SetValue] Cannot cast from {value.GetType()} to {typeof(float)}");
            }
        }
    }

    public class IntVariable : Variable
    {
        public int value;
        public int minValue = 0;
        public int maxValue = 100;

        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object value)
        {
            if (value == null)
                throw new Exception($"[{nameof(IntVariable)}.SetValue] Tryint to set a null value as int!");
            try
            {
                var convertedValue = Convert.ToInt32(value);

                this.value = Mathf.Clamp(convertedValue, minValue, maxValue);
            }
            catch (Exception)
            {
                throw new InvalidCastException($"[{nameof(IntVariable)}.SetValue] Cannot cast from {value.GetType()} to {typeof(int)}");
            }
        }
    }

    public class BoolVariable : Variable
    {
        public bool value;

        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object value)
        {
            if(value == null)
            {
                throw new Exception($"[{nameof(BoolVariable)}.SetValue] Tryint to set a null value as bool!");
            }

            try
            {
                var convertedValue = Convert.ToBoolean(value);

                this.value = convertedValue;
            }
            catch (Exception)
            {
                throw new InvalidCastException($"[{nameof(BoolVariable)}.SetValue] Cannot cast from {value.GetType()} to {typeof(bool)}");
            }

        }
    }

    public class StringVariable : Variable
    {
        public string value;



        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object value)
        {
            if(value == null)
            {
                this.value = string.Empty;
            }

            try
            {
                var converted = Convert.ToString(value);
            }
            catch (Exception)
            {
                throw new InvalidCastException($"[{nameof(StringVariable)}.SetValue] Cannot cast from {value.GetType()} to {typeof(string)}");
            }
        }
    }    
}