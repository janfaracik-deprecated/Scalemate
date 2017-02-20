using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Scalemate.Helpers
{
    public static class Animationmate
    {

        public static void ChangeObjectWidth(DependencyObject o, double oldWidth, double newWidth, double duration = 250)
        {
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseOut;

            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = oldWidth,
                To = newWidth,
                Duration = TimeSpan.FromMilliseconds(duration),
                EnableDependentAnimation = true,
                EasingFunction = easingFunction
            };

            Storyboard.SetTargetProperty(widthAnimation, "(FrameworkElement.Width)");
            Storyboard.SetTarget(widthAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(widthAnimation);
            s.Begin();
        }

        public static void ChangeObjectHeight(DependencyObject o, double oldHeight, double newHeight, double duration = 250)
        {
            SineEase easingFunction = new SineEase();
            easingFunction.EasingMode = EasingMode.EaseOut;

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = oldHeight,
                To = newHeight,
                Duration = TimeSpan.FromMilliseconds(duration),
                EnableDependentAnimation = true,
                EasingFunction = easingFunction
            };

            Storyboard.SetTargetProperty(heightAnimation, "(FrameworkElement.Height)");
            Storyboard.SetTarget(heightAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(heightAnimation);
            s.Begin();
        }

        public static void ChangeObjectOpacity(DependencyObject o, double oldOpacity, double newOpacity, double duration = 250, double delay = 0)
        {
            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = oldOpacity,
                To = newOpacity,
                Duration = TimeSpan.FromMilliseconds(duration),
                BeginTime = TimeSpan.FromMilliseconds(delay)
            };

            Storyboard.SetTargetProperty(heightAnimation, "(FrameworkElement.Opacity)");
            Storyboard.SetTarget(heightAnimation, o);

            Storyboard s = new Storyboard();
            s.Children.Add(heightAnimation);
            s.Begin();
        }

    }
}
