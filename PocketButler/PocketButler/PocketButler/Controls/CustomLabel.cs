using System;
using Xamarin.Forms;

namespace PocketButler
{
	public class CustomLabel : Label
	{
		public CustomLabel ()
		{
		}

		/// <summary>
		/// The font size property
		/// </summary>
		public static readonly BindableProperty FontSizeProperty =
			BindableProperty.Create<CustomLabel, double>(
				p => p.FontSize, -1);

        public Action Tapped;
		/// <summary>
		/// Gets or sets the size of the font.
		/// </summary>
		/// <value>The size of the font.</value>
		public double FontSize
		{
			get
			{
				return (double)GetValue(FontSizeProperty);
			}
			set
			{
				SetValue(FontSizeProperty, value);
			}
		}

		/// <summary>
		/// The font name android property.
		/// </summary>
		[Obsolete("This is now obsolete. Please rather use FontName and FriendlyFontName to cover all platforms.")]
		public static readonly BindableProperty FontNameAndroidProperty =
			BindableProperty.Create<CustomLabel, string>(
				p => p.FontNameAndroid, string.Empty);

		/// <summary>
		/// Gets or sets the font name android.
		/// </summary>
		/// <value>The font name android.</value>
		[Obsolete("This is now obsolete. Please rather use FontName and FriendlyFontName to cover all platforms.")]
		public string FontNameAndroid
		{
			get
			{
				return (string)GetValue(FontNameAndroidProperty);
			}
			set
			{
				SetValue(FontNameAndroidProperty, value);
			}
		}

		/// <summary>
		/// The font name ios property.
		/// </summary>
		[Obsolete("This is now obsolete. Please rather use FontName and FriendlyFontName to cover all platforms.")]
		public static readonly BindableProperty FontNameIOSProperty =
			BindableProperty.Create<CustomLabel, string>(
				p => p.FontNameIOS, string.Empty);

		/// <summary>
		/// Gets or sets the font name ios.
		/// </summary>
		/// <value>The font name ios.</value>
		[Obsolete]
		public string FontNameIOS
		{
			get
			{
				return (string)GetValue(FontNameIOSProperty);
			}
			set
			{
				SetValue(FontNameIOSProperty, value);
			}
		}

		/// <summary>
		/// The font name property.
		/// </summary>
		public static readonly BindableProperty FontNameProperty =
			BindableProperty.Create<CustomLabel, string>(
				p => p.FontName, string.Empty);

		/// <summary>
		/// Gets or sets the name of the font file including extension. If no extension given then ttf is assumed.
		/// Fonts need to be included in projects accoring to the documentation.
		/// </summary>
		/// <value>The full name of the font file including extension.</value>
		public string FontName
		{
			get
			{
				return (string)GetValue(FontNameProperty);
			}
			set
			{
				SetValue(FontNameProperty, value);
			}
		}

		/// <summary>
		/// The friendly font name property. This can be found on the first line of the font or in the font preview. 
		/// This is only required on Windows Phone. If not given then the file name excl. the extension is used.
		/// </summary>
		public static readonly BindableProperty FriendlyFontNameProperty =
			BindableProperty.Create<CustomLabel, string>(
				p => p.FriendlyFontName, string.Empty);

		/// <summary>
		/// Gets or sets the name of the font.
		/// </summary>
		/// <value>The name of the font.</value>
		public string FriendlyFontName
		{
			get
			{
				return (string)GetValue(FriendlyFontNameProperty);
			}
			set
			{
				SetValue(FriendlyFontNameProperty, value);
			}
		}

		/// <summary>
		/// The is underlined property.
		/// </summary>
		public static readonly BindableProperty IsUnderlineProperty =
			BindableProperty.Create<CustomLabel, bool>(p => p.IsUnderline, false);

		/// <summary>
		/// Gets or sets a value indicating whether the text in the label is underlined.
		/// </summary>
		/// <value>A <see cref="bool"/> indicating if the text in the label should be underlined.</value>
		public bool IsUnderline
		{
			get
			{
				return (bool)GetValue(IsUnderlineProperty);
			}
			set
			{
				SetValue(IsUnderlineProperty, value);
			}
		}

		/// <summary>
		/// The is underlined property.
		/// </summary>
		public static readonly BindableProperty IsStrikeThroughProperty =
			BindableProperty.Create<CustomLabel, bool>(p => p.IsStrikeThrough, false);

		/// <summary>
		/// Gets or sets a value indicating whether the text in the label is underlined.
		/// </summary>
		/// <value>A <see cref="bool"/> indicating if the text in the label should be underlined.</value>
		public bool IsStrikeThrough
		{
			get
			{
				return (bool)GetValue(IsStrikeThroughProperty);
			}
			set
			{
				SetValue(IsStrikeThroughProperty, value);
			}
		}

		/// <summary>
		/// This is the drop shadow property
		/// </summary>
		public static readonly BindableProperty IsDropShadowProperty =
			BindableProperty.Create<CustomLabel, bool>(p => p.IsDropShadow, false);

		public bool IsDropShadow 
		{
			get 
			{
				return (bool)GetValue (IsDropShadowProperty);
			}
			set 
			{
				SetValue (IsDropShadowProperty, value);
			}
		}
	}
}

