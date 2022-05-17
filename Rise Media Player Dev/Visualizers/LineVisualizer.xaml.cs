using AudioVisualizer;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Rise.App.Visualizers
{
    public sealed partial class LineVisualizer : UserControl
    {
        private SpectrumData _emptySpectrum;
        private SpectrumData _previousSpectrum;
        private SpectrumData _previousPeakSpectrum;

        private const double fps = 1000d / 60d;

        private static readonly TimeSpan _rmsRiseTime = TimeSpan.FromMilliseconds(12 * fps);
        private static readonly TimeSpan _rmsFallTime = TimeSpan.FromMilliseconds(12 * fps);
        private static readonly TimeSpan _peakRiseTime = TimeSpan.FromMilliseconds(12 * fps);
        private static readonly TimeSpan _peakFallTime = TimeSpan.FromMilliseconds(12 * fps);
        private static readonly TimeSpan _frameDuration = TimeSpan.FromMilliseconds(fps);

        private PlaybackSource _src => App.MPViewModel.VisualizerPlaybackSource;

        public int NumOfLines = 16;

        public LineVisualizer()
        {
            InitializeComponent();

            Loaded += LineEqualizer_Loaded;
        }

        private void LineEqualizer_Loaded(object sender, RoutedEventArgs e)
        {
            _emptySpectrum = SpectrumData.CreateEmpty(2, (uint)NumOfLines, ScaleType.Linear, ScaleType.Linear, 0, 20000);
            VUBar.Source = _src.Source;
            _src.SourceChanged += Src_SourceChanged;
        }

        private void VUBar_Draw(object sender, VisualizerDrawEventArgs args)
        {
            var drawingSession = (CanvasDrawingSession)args.DrawingSession;

            float barWidth = (float)(args.ViewExtent.Width / (2 * NumOfLines));

            var barSize = new Vector2(barWidth, (float)(args.ViewExtent.Height - 2 * barWidth));

            var spectrumData = args.Data != null && VUBar.Source?.PlaybackState == SourcePlaybackState.Playing ?
                                            args.Data.Spectrum.LogarithmicTransform((uint)NumOfLines, 20f, 20000f) : _emptySpectrum;

            _previousSpectrum = spectrumData.ApplyRiseAndFall(_previousSpectrum, _rmsRiseTime, _rmsFallTime, _frameDuration);
            _previousPeakSpectrum = spectrumData.ApplyRiseAndFall(_previousPeakSpectrum, _peakRiseTime, _peakFallTime, _frameDuration);

            var logSpectrum = _previousSpectrum.ConvertToDecibels(-50, 0);

            var step = args.ViewExtent.Width / NumOfLines;
            var flaw = (step - barSize.X) / 2;

            using var brush = new CanvasLinearGradientBrush(drawingSession, new CanvasGradientStop[] { new CanvasGradientStop() { Color = Color.FromArgb(255, 66, 245, 141), Position = 0f }, new CanvasGradientStop() { Color = Color.FromArgb(255, 66, 126, 245), Position = 1f } })
            {
                StartPoint = new Vector2((float)args.ViewExtent.Width, 0),
                EndPoint = new Vector2(0, (float)args.ViewExtent.Width)
            };
            for (int index = 0; index < NumOfLines; index++)
            {
                float barX = (float)(step * index + flaw);
                float spectrumBarHeight = barSize.Y * (1.0f - (logSpectrum[0][index] + logSpectrum[1][index]) / -100.0f);
                drawingSession.FillRoundedRectangle(barX, (float)(args.ViewExtent.Height - barWidth - spectrumBarHeight), barSize.X, spectrumBarHeight, barSize.X / 2, barSize.X / 2, brush);
            }
        }

        private void Src_SourceChanged(object sender, IVisualizationSource args)
        {
            VUBar.Source = args;
        }
    }
}
