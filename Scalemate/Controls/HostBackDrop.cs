using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Scalemate.Controls
{
    public class HostBackDrop : Control
    {

        Compositor m_compositor;
        SpriteVisual m_blurVisual;
        CompositionEffectBrush m_blurBrush;
        bool m_setUpExpressions;
        Visual m_rootVisual;

        public HostBackDrop()
        {
            
            m_rootVisual = ElementCompositionPreview.GetElementVisual(this as UIElement);
            Compositor = m_rootVisual.Compositor;

            m_blurBrush = BuildBlurBrush();
            m_blurBrush.SetSourceParameter("source", m_compositor.CreateHostBackdropBrush());

            m_blurVisual = Compositor.CreateSpriteVisual();
            m_blurVisual.Brush = m_blurBrush;

            BlurAmount = 30;

            ElementCompositionPreview.SetElementChildVisual(this as UIElement, m_blurVisual);

            this.Loading += OnLoading;
            this.Unloaded += OnUnloaded;
        }

        public const string BlurAmountProperty = nameof(BlurAmount);

        public double BlurAmount
        {
            get
            {
                float value = 0;
                m_rootVisual.Properties.TryGetScalar(BlurAmountProperty, out value);
                return value;
            }
            set
            {
                if (!m_setUpExpressions)
                {
                    m_blurBrush.Properties.InsertScalar("Blur.BlurAmount", (float)value);
                }
                m_rootVisual.Properties.InsertScalar(BlurAmountProperty, (float)value);
            }
        }
        
        public CompositionPropertySet VisualProperties
        {
            get
            {
                if (!m_setUpExpressions)
                {
                    SetUpPropertySetExpressions();
                }
                return m_rootVisual.Properties;
            }
        }

        public Compositor Compositor
        {
            get
            {
                return m_compositor;
            }

            private set
            {
                m_compositor = value;
            }
        }

        private void OnLoading(FrameworkElement sender, object args)
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.SizeChanged += OnSizeChanged;
                OnSizeChanged(this, null);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.SizeChanged -= OnSizeChanged;
            }
        }


        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (m_blurVisual != null)
                {
                    m_blurVisual.Size = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
                }
            }
        }

        private void SetUpPropertySetExpressions()
        {
            m_setUpExpressions = true;

            var exprAnimation = Compositor.CreateExpressionAnimation();
            exprAnimation.Expression = $"sourceProperties.{BlurAmountProperty}";
            exprAnimation.SetReferenceParameter("sourceProperties", m_rootVisual.Properties);

            m_blurBrush.Properties.StartAnimation("Blur.BlurAmount", exprAnimation);

            m_blurBrush.Properties.StartAnimation("Color.Color", exprAnimation);
        }


        private CompositionEffectBrush BuildBlurBrush()
        {
            GaussianBlurEffect blurEffect = new GaussianBlurEffect()
            {
                Name = "Blur",
                BlurAmount = 0.0f,
                BorderMode = EffectBorderMode.Hard,
                Optimization = EffectOptimization.Balanced,
                Source = new CompositionEffectSourceParameter("source"),
            };
            
            SaturationEffect saturationEffect = new SaturationEffect
            {
                Source = blurEffect,
                Saturation = 2f,
            };

            var factory = Compositor.CreateEffectFactory(saturationEffect, new[] { "Blur.BlurAmount" });

            CompositionEffectBrush brush = factory.CreateBrush();
            return brush;
        }
    }

}
