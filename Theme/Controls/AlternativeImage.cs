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
    public class AlternativeImage : UserControl
    {
        #region dependency properties

        public static readonly DependencyProperty AlternativeStateProperty =
            DependencyProperty.Register("AlternativeState", typeof(bool), typeof(AlternativeImage),
            new FrameworkPropertyMetadata((bool)true, new PropertyChangedCallback(OnAlternativeStateChanged)));

        public bool AlternativeState
        {
            get { return (bool)GetValue(AlternativeStateProperty); }
            set { SetValue(AlternativeStateProperty, value); }
        }

        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(AlternativeImage), null);

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public static readonly DependencyProperty TrueStateIconSourceProperty =
            DependencyProperty.Register("TrueStateIconSource", typeof(ImageSource), typeof(AlternativeImage),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAlternativeStateChanged)));

        public ImageSource TrueStateIconSource
        {
            get { return (ImageSource)GetValue(TrueStateIconSourceProperty); }
            set { SetValue(TrueStateIconSourceProperty, value); }
        }

        public static readonly DependencyProperty FalseStateIconSourceProperty =
            DependencyProperty.Register("FalseStateIconSource", typeof(ImageSource), typeof(AlternativeImage),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAlternativeStateChanged)));

        public ImageSource FalseStateIconSource
        {
            get { return (ImageSource)GetValue(FalseStateIconSourceProperty); }
            set { SetValue(FalseStateIconSourceProperty, value); }
        }

        #endregion

        #region methods

        private static void OnAlternativeStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AlternativeImage target = d as AlternativeImage;
            if (target.AlternativeState == true)
            {
                target.IconSource = target.TrueStateIconSource;
            }
            else
            {
                target.IconSource = target.FalseStateIconSource;
            }
        }
        #endregion
    }
}
