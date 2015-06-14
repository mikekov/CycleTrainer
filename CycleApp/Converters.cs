using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UiStyles
{
	static class Helpers
	{
		public enum Side { Single, Left, Middle, Right }

		// determine where the button is located: value - a radio button
		// this routine is going to look at its parent container, to determine if button is:
		// first one, in the middle, last one, or lone one
		public static Side ButtonSide(object value)
		{
			var ctrl = value as Control;

			if (ctrl != null)
			{
				var parent = ctrl.Parent as Panel;

				if (parent != null)
				{
					var c = parent.Children;
					if (c.Count < 2)
						return Side.Single;

					if (c[0] == ctrl)			// leftmost?
						return Side.Left;

					if (c[c.Count - 1] == ctrl)	// rightmost?
						return Side.Right;

					// middle ones
					return Side.Middle;
				}
			}

			return Side.Single;
		}
	}

	// Prepare right Margin value depending on button's location
	// Margin is used to shift one side of rounded rect outside the clipping region of its parent grid
	// to hide rounded corners on one side
	public class ButtonToMarginConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var margin = double.Parse(parameter as string);
			bool internal_part = false;
			if (margin < 0)
			{
				internal_part = true;
				margin = -margin;
			}

			switch (Helpers.ButtonSide(value))
			{
			case Helpers.Side.Single:
				return new Thickness(internal_part ? margin + 1 : margin, margin, margin, margin);

			case Helpers.Side.Left:
				return new Thickness(internal_part ? margin + 1 : margin, margin, SHIFT, margin);

			case Helpers.Side.Middle:
				return new Thickness(SHIFT, margin, SHIFT, margin);

			case Helpers.Side.Right:
				return new Thickness(SHIFT, margin, margin, margin);
			}

			return new Thickness(0);
		}

		// amount of shift of the rounded rect intended to hide one side
		const double SHIFT = -10.0;

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	// Prepare vertical highlight width based on button's location
	// This vertical highlight is used between neighbouring buttons and needs to be hidden for left/rightmost buttons
	public class ButtonToWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool left = (parameter as string) == "0";

			switch (Helpers.ButtonSide(value))
			{
			case Helpers.Side.Single:
				return 0.0;

			case Helpers.Side.Left:
				return left ? 0.0 : 1.0;

			case Helpers.Side.Middle:
				return 1.0;

			case Helpers.Side.Right:
				return left ? 1.0 : 0.0;
			}

			return 0.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
