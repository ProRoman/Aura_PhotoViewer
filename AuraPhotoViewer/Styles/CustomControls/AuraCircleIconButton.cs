using System.Windows;
using System.Windows.Controls;

namespace AuraPhotoViewer.Styles.CustomControls
{
    public class AuraCircleIconButton : Button
    {
        static AuraCircleIconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AuraCircleIconButton), new FrameworkPropertyMetadata(typeof(AuraCircleIconButton)));
        }

        public static readonly DependencyProperty AuraIconTypeProperty = DependencyProperty.Register("AuraIconType", typeof(AuraIconButtonType), typeof(AuraCircleIconButton), new FrameworkPropertyMetadata(AuraIconButtonType.NavigateBefore));

        public AuraIconButtonType AuraIconType
        {
            get
            {
                return (AuraIconButtonType)GetValue(AuraIconTypeProperty);
            }
            set
            {
                SetValue(AuraIconTypeProperty, value);
            }
        }
    }
}
