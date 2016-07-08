using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Theme.Controls
{
    public class Button : System.Windows.Controls.Button
    {
        #region dependency properties

        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(Button), null);

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public static readonly DependencyProperty PressedIconSourceProperty =
            DependencyProperty.Register("PressedIconSource", typeof(ImageSource), typeof(Button), null);

        public ImageSource PressedIconSource
        {
            get { return (ImageSource)GetValue(PressedIconSourceProperty); }
            set { SetValue(PressedIconSourceProperty, value); }
        }
        #endregion
    }
}
