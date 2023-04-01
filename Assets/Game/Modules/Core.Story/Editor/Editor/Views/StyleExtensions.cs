using UnityEngine.UIElements;

namespace Self.Story.Editors
{
	public static class StyleExtensions
	{
		public static void SetBorderRadius(this IStyle style, StyleLength value)
		{
			style.borderBottomLeftRadius =
				style.borderBottomRightRadius =
					style.borderTopLeftRadius =
						style.borderTopRightRadius = value;
		}

		public static void SetBorder(this IStyle style, StyleFloat value)
		{
			style.borderRightWidth =
				style.borderTopWidth =
					style.borderLeftWidth =
						style.borderBottomWidth = value;
		}

		public static void SetBorderColor(this IStyle style, UnityEngine.Color color)
		{
			SetBorderColor(style, new StyleColor(color));
		}

		public static void SetBorderColor(this IStyle style, StyleColor value)
		{
			style.borderRightColor =
				style.borderLeftColor =
					style.borderTopColor =
						style.borderBottomColor = value;
		}

		public static void SetPadding(this IStyle style, StyleLength value)
		{
			style.paddingBottom =
				style.paddingLeft =
					style.paddingRight =
						style.paddingTop = value;
		}

		public static void SetMargin(this IStyle style, StyleLength value)
		{
			style.marginBottom =
				style.marginLeft =
					style.marginRight =
						style.marginTop = value;
		}

		public static void SetMargin(this IStyle style, StyleLength top_bottom, StyleLength left_right)
		{
			style.marginTop =
				style.marginBottom = top_bottom;
			style.marginLeft =
				style.marginRight = left_right;
		}
	}
}