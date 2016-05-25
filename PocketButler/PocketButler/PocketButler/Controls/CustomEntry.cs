using System;
using Xamarin.Forms;

namespace PocketButler
{
	public class CustomEntry : Entry
	{
		public CustomEntry ()
		{
			Font = Font.SystemFontOfSize (NamedSize.Medium);
			YAlign = TextAlignment.Center;
		}

		/// <summary>
		/// The font property
		/// </summary>
		public static readonly BindableProperty FontProperty =
			BindableProperty.Create("Font", typeof(Font), typeof(CustomEntry), new Font());

		/// <summary>
		/// The XAlign property
		/// </summary>
		public static readonly BindableProperty XAlignProperty =
			BindableProperty.Create("XAlign", typeof(TextAlignment), typeof(CustomEntry), 
				TextAlignment.Start);

        /// <summary>
        /// The YAlign property
        /// </summary>
        public static readonly BindableProperty YAlignProperty =
            BindableProperty.Create("YAlign", typeof(TextAlignment), typeof(CustomEntry),
                TextAlignment.Start);

		/// <summary>
		/// The HasBorder property
		/// </summary>
		public static readonly BindableProperty HasBorderProperty =
			BindableProperty.Create("HasBorder", typeof(bool), typeof(CustomEntry), true);

		/// <summary>
		/// The HasBackground property
		/// </summary>
		public static readonly BindableProperty HasBackgroundProperty =
			BindableProperty.Create("HasBackground", typeof(bool), typeof(CustomEntry), true);

		/// <summary>
		/// The PlaceholderTextColor property
		/// </summary>
		public static readonly BindableProperty PlaceholderTextColorProperty =
			BindableProperty.Create("PlaceholderTextColor", typeof(Color), typeof(CustomEntry), Color.Default);

		/// <summary>
		/// Gets or sets the Font
		/// </summary>
		public Font Font
		{
			get { return (Font)GetValue(FontProperty); }
			set { SetValue(FontProperty, value); }
		}

		/// <summary>
		/// Gets or sets the X alignment of the text
		/// </summary>
		public TextAlignment XAlign
		{
			get { return (TextAlignment)GetValue(XAlignProperty); }
			set { SetValue(XAlignProperty, value); }
		}

        /// <summary>
        /// Gets or sets the X alignment of the text
        /// </summary>
        public TextAlignment YAlign
        {
            get { return (TextAlignment)GetValue(YAlignProperty); }
            set { SetValue(YAlignProperty, value); }
        }

		/// <summary>
		/// Gets or sets if the border should be shown or not
		/// </summary>
		public bool HasBorder
		{
			get { return (bool)GetValue(HasBorderProperty); }
			set { SetValue(HasBorderProperty, value); }
		}

		/// <summary>
		/// Gets or sets if the background image should be shown or not
		/// </summary>
		public bool HasBackgroundImage
		{
			get { return (bool)GetValue(HasBackgroundProperty); }
			set { SetValue(HasBackgroundProperty, value); }
		}

		/// <summary>
		/// Sets color for placeholder text
		/// </summary>
		public Color PlaceholderTextColor
		{
			get { return (Color)GetValue(PlaceholderTextColorProperty); }
			set { SetValue(PlaceholderTextColorProperty, value); }
		}
	}
}

