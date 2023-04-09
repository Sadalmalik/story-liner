// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kaleb.TweenAnimator
{
    public static class ColorHelper
    {
        public static FloatSetter GetTMPTextFloatSetter(ColorDataType type, TMP_Text text)
        {
            FloatSetter setter = null;

            switch (type)
            {
                case ColorDataType.ColorR:
                    setter = value => text.color = text.color.SetR(value);
                    break;
                case ColorDataType.ColorG:
                    setter = value => text.color = text.color.SetG(value);
                    break;
                case ColorDataType.ColorB:
                    setter = value => text.color = text.color.SetB(value);
                    break;
                case ColorDataType.ColorA:
                    setter = value => text.color = text.color.SetA(value);
                    break;
            }

            return setter;
        }
        
        public static ColorSetter GetImageColorSetter(ColorDataType type, Image image)
        {
            ColorSetter setter = null;

            setter = value => image.color = value;

            return setter;
        }
        
        public static FloatSetter GetImageFloatSetter(ColorDataType type, Image image)
        {
            FloatSetter setter = null;

            switch (type)
            {
                case ColorDataType.ColorR:
                    setter = value => image.color = image.color.SetR(value);
                    break;
                case ColorDataType.ColorG:
                    setter = value => image.color = image.color.SetG(value);
                    break;
                case ColorDataType.ColorB:
                    setter = value => image.color = image.color.SetB(value);
                    break;
                case ColorDataType.ColorA:
                    setter = value => image.color = image.color.SetA(value);
                    break;
            }

            return setter;
        }
        
        public static ColorSetter GetMaterialColorSetter(ColorDataType type, Material material)
        {
            ColorSetter setter = null;

            setter = value => material.color = value;

            return setter;
        }
        
        public static FloatSetter GetMaterialFloatSetter(ColorDataType type, Material material)
        {
            FloatSetter setter = null;

            switch (type)
            {
                case ColorDataType.ColorR:
                    setter = value => material.color = material.color.SetR(value);
                    break;
                case ColorDataType.ColorG:
                    setter = value => material.color = material.color.SetG(value);
                    break;
                case ColorDataType.ColorB:
                    setter = value => material.color = material.color.SetB(value);
                    break;
                case ColorDataType.ColorA:
                    setter = value => material.color = material.color.SetA(value);
                    break;
            }

            return setter;
        }
    }
}