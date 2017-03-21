using GalaSoft.MvvmLight.Messaging;
using Scalemate.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Scalemate.Controls
{
    public sealed partial class PopupControl : UserControl
    {

        Visual visualBackground;
        Visual visualPopup;
        Compositor compositor;
        CubicBezierEasingFunction easeIn;

        public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(object), typeof(PopupControl), new PropertyMetadata(null));
        public object PopupContent
        {
            get { return GetValue(PopupContentProperty); }
            set
            {
                if (value != null)
                    SetValue(PopupContentProperty, value);
            }
        }

        public PopupControl()
        {
            this.InitializeComponent();
        }

        private void popupControl_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        public void Show()
        {

            Visibility = Visibility.Visible;

            visualBackground = ElementCompositionPreview.GetElementVisual(gridBackground);
            visualPopup = ElementCompositionPreview.GetElementVisual(gridPopupInner);
            compositor = visualPopup.Compositor;
            easeIn = compositor.CreateCubicBezierEasingFunction(new Vector2(0.4f, 1.21f), new Vector2(0.6f, 1.05f));

            ScalarKeyFrameAnimation opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(200);
            opacityAnimation.InsertKeyFrame(0f, 0f);
            opacityAnimation.InsertKeyFrame(1f, 1f, compositor.CreateLinearEasingFunction());

            ScalarKeyFrameAnimation offsetAnimation = compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(300);
            offsetAnimation.InsertKeyFrame(0f, (float)ActualHeight);
            offsetAnimation.InsertKeyFrame(1f, 0f, easeIn);

            visualBackground.StartAnimation("Opacity", opacityAnimation);
            visualPopup.StartAnimation("Offset.Y", offsetAnimation);

        }

        public void Hide()
        {

            ScalarKeyFrameAnimation opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(200);
            opacityAnimation.InsertKeyFrame(0f, 1f);
            opacityAnimation.InsertKeyFrame(1f, 0f, compositor.CreateLinearEasingFunction());

            ScalarKeyFrameAnimation offsetAnimation = compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(300);
            offsetAnimation.InsertKeyFrame(0f, 0);
            offsetAnimation.InsertKeyFrame(1f, (float)ActualHeight, easeIn);

            var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            visualBackground.StartAnimation("Opacity", opacityAnimation);
            visualPopup.StartAnimation("Offset.Y", offsetAnimation);

            batch.End();

            batch.Completed += OnBatchCompleted;

        }

        private void OnBatchCompleted(object sender, CompositionBatchCompletedEventArgs args)
        {
            Visibility = Visibility.Collapsed;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void rectangleBG_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Hide();
        }

    }
}
