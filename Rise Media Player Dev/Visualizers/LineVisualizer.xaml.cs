using AudioVisualizer;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Rise.App.Visualizers
{
    public sealed partial class LineVisualizer : UserControl
    {
        private SpectrumData _emptySpectrum;
        private SpectrumData _previousSpectrum;
        private SpectrumData _previousPeakSpectrum;

        private const double fps = 1000d / 30d;
        private const int NumOfLines = 16;

        private static readonly TimeSpan _rmsRiseTime = TimeSpan.FromMilliseconds(12 * fps);
        private static readonly TimeSpan _rmsFallTime = TimeSpan.FromMilliseconds(12 * fps);
        private static readonly TimeSpan _peakRiseTime = TimeSpan.FromMilliseconds(12 * fps);
        private static readonly TimeSpan _peakFallTime = TimeSpan.FromMilliseconds(12 * fps);
        private static readonly TimeSpan _frameDuration = TimeSpan.FromMilliseconds(fps);

        private float canvasWidth;
        private float canvasHeight;

        public IVisualizationSource VisualizerSource
        {
            get => (IVisualizationSource)GetValue(VisualizerSourceProperty);
            set => SetValue(VisualizerSourceProperty, value);
        }

        public LineVisualizer()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _emptySpectrum = SpectrumData.CreateEmpty(2,
                NumOfLines,
                ScaleType.Linear,
                ScaleType.Linear,
                0,
                20000);

            canvasWidth = (float)VUBar.ActualWidth;
            canvasHeight = (float)VUBar.ActualHeight;
        }
    }

    // Event handlers
    public sealed partial class LineVisualizer
    {
        private void VUBar_Draw(object sender, VisualizerDrawEventArgs args)
        {
            using CanvasDrawingSession drawingSession = (CanvasDrawingSession)args.DrawingSession;

            float barWidth = (float)(canvasWidth / (2 * NumOfLines));

            var barSize = new Vector2(barWidth, canvasHeight - 2 * barWidth);

            var spectrumData = args.Data != null && VUBar.Source?.PlaybackState == SourcePlaybackState.Playing ?
                                            args.Data.Spectrum.LogarithmicTransform(NumOfLines, 20f, 20000f) : _emptySpectrum;

            _previousSpectrum = spectrumData.ApplyRiseAndFall(_previousSpectrum, _rmsRiseTime, _rmsFallTime, _frameDuration);
            _previousPeakSpectrum = spectrumData.ApplyRiseAndFall(_previousPeakSpectrum, _peakRiseTime, _peakFallTime, _frameDuration);

            var logSpectrum = _previousSpectrum.ConvertToDecibels(-50, 0);

            var step = canvasWidth / NumOfLines;
            var flaw = (step - barSize.X) / 2;

            using var brush = new CanvasLinearGradientBrush(drawingSession, new CanvasGradientStop[] { new CanvasGradientStop() { Color = Color.FromArgb(255, 66, 245, 141), Position = 0f }, new CanvasGradientStop() { Color = Color.FromArgb(255, 66, 126, 245), Position = 1f } })
            {
                StartPoint = new Vector2(canvasWidth, 0),
                EndPoint = new Vector2(0, canvasWidth)
            };

            for (int index = 0; index < NumOfLines; index++)
            {
                float barX = (float)(step * index + flaw);
                float spectrumBarHeight = barSize.Y * (1.0f - (logSpectrum[0][index] + logSpectrum[1][index]) / -100.0f);
                drawingSession.FillRoundedRectangle(barX, (float)(canvasHeight - barWidth - spectrumBarHeight), barSize.X, spectrumBarHeight, barSize.X / 2, barSize.X / 2, brush);
            }
        }
    }

    // Dependency properties
    public sealed partial class LineVisualizer
    {
        private readonly static DependencyProperty VisualizerSourceProperty =
            DependencyProperty.Register(nameof(VisualizerSource), typeof(IVisualizationSource),
                typeof(LineVisualizer), null);
    }
}
