using NAudio.Dsp;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;

namespace Rise.Effects
{
    public sealed class EqualizerEffect : IBasicAudioEffect
    {
        private static EqualizerEffect current;

        public static EqualizerEffect Current => current;

        public bool IsEnabled
        {
            get => (bool)configuration["Enabled"];
            set => configuration["Enabled"] = value;
        }

        private List<AudioEncodingProperties> _supportedEncodingProperties;

        public IReadOnlyList<AudioEncodingProperties> SupportedEncodingProperties
        {
            get
            {
                if (_supportedEncodingProperties == null)
                {
                    _supportedEncodingProperties = new List<AudioEncodingProperties>();

                    AudioEncodingProperties encodingProps1 = AudioEncodingProperties.CreatePcm(44100, 1, 32);
                    encodingProps1.Subtype = MediaEncodingSubtypes.Float;
                    AudioEncodingProperties encodingProps2 = AudioEncodingProperties.CreatePcm(48000, 1, 32);
                    encodingProps2.Subtype = MediaEncodingSubtypes.Float;

                    AudioEncodingProperties encodingProps3 = AudioEncodingProperties.CreatePcm(44100, 2, 32);
                    encodingProps3.Subtype = MediaEncodingSubtypes.Float;
                    AudioEncodingProperties encodingProps4 = AudioEncodingProperties.CreatePcm(48000, 2, 32);
                    encodingProps4.Subtype = MediaEncodingSubtypes.Float;

                    AudioEncodingProperties encodingProps5 = AudioEncodingProperties.CreatePcm(96000, 2, 32);
                    encodingProps5.Subtype = MediaEncodingSubtypes.Float;
                    AudioEncodingProperties encodingProps6 = AudioEncodingProperties.CreatePcm(192000, 2, 32);
                    encodingProps6.Subtype = MediaEncodingSubtypes.Float;

                    _supportedEncodingProperties.Add(encodingProps1);
                    _supportedEncodingProperties.Add(encodingProps2);
                    _supportedEncodingProperties.Add(encodingProps3);
                    _supportedEncodingProperties.Add(encodingProps4);
                    _supportedEncodingProperties.Add(encodingProps5);
                    _supportedEncodingProperties.Add(encodingProps6);
                }

                return _supportedEncodingProperties;
            }
        }

        public bool UseInputFrameForOutput => true;

        private AudioEncodingProperties currentEncodingProperties;
        private EqualizerBand[] bands;
        private BiQuadFilter[,] filters;
        private int channels;
        private int bandCount;
        private IPropertySet configuration;

        public EqualizerEffect()
        {
            current = this;
        }

        public void UpdateEqualizerBand(IReadOnlyList<float> equalizerBands)
        {
            if (bandCount != equalizerBands.Count)
            {
                throw new ArgumentException("Bands count must be 10");
            }
            // Generalize to 0@max
            var max = float.MinValue;
            foreach (var gain in equalizerBands)
            {
                if (gain > max)
                    max = gain;
            }
            for (int i = 0; i < bandCount; i++)
            {
                bands[i].Gain = equalizerBands[i] - max;
            }
            CreateFilters();
        }

        public void SetEncodingProperties(AudioEncodingProperties encodingProperties)
        {
            currentEncodingProperties = encodingProperties;

            bands = ReadConfiguration();

            if (channels != (int)currentEncodingProperties.ChannelCount || bandCount != bands.Length)
            {
                channels = (int)currentEncodingProperties.ChannelCount;
                bandCount = bands.Length;

                filters = new BiQuadFilter[channels, bandCount];
            }
            CreateFilters();
        }

        private EqualizerBand[] ReadConfiguration()
        {
            if (bands != null)
            {
                return bands;
            }
            // Generalize to 0@max
            var max = float.MinValue;
            foreach (var gain in (float[])configuration["Gain"])
            {
                if (gain > max)
                    max = gain;
            }
            return new EqualizerBand[]
            {
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 30, Gain = ((float[])configuration["Gain"])[0] - max },
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 75, Gain = ((float[])configuration["Gain"])[1] - max },
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 150, Gain = ((float[])configuration["Gain"])[2] - max },
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 30, Gain = ((float[])configuration["Gain"])[3] - max },
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 600, Gain = ((float[])configuration["Gain"])[4] - max },
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 1250, Gain = ((float[])configuration["Gain"])[5] - max },
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 2500, Gain = ((float[])configuration["Gain"])[6] - max },
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 5000, Gain = ((float[])configuration["Gain"])[7] - max },
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 10000, Gain = ((float[])configuration["Gain"])[8] - max },
                new EqualizerBand { Bandwidth = 0.8f, Frequency = 20000, Gain = ((float[])configuration["Gain"])[9] - max },
            };
        }

        private void CreateFilters()
        {
            for (int bandIndex = 0; bandIndex < bandCount; bandIndex++)
            {
                var band = bands[bandIndex];
                for (int n = 0; n < channels; n++)
                {
                    if (filters[n, bandIndex] == null)
                        filters[n, bandIndex] = BiQuadFilter.PeakingEQ(currentEncodingProperties.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                    else
                        filters[n, bandIndex].SetPeakingEq(currentEncodingProperties.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                }
            }
        }

        public void ProcessFrame(ProcessAudioFrameContext context)
        {
            // Check if we don't have the effect enabled in the configuration.
            // This is a workaround for the fact that MediaPlayer does not
            // have a way to remove/disable the effect.
            if (!IsEnabled)
                return;

            unsafe
            {
                AudioFrame inputFrame = context.InputFrame;

                using (AudioBuffer inputBuffer = inputFrame.LockBuffer(AudioBufferAccessMode.ReadWrite))
                using (IMemoryBufferReference inputReference = inputBuffer.CreateReference())
                {
                    ((IMemoryBufferByteAccess)inputReference).GetBuffer(out byte* inputDataInBytes, out uint inputCapacity);

                    float* inputDataInFloat = (float*)inputDataInBytes;
                    int dataInFloatLength = (int)inputBuffer.Length / sizeof(float);

                    // Process audio data
                    for (int n = 0; n < dataInFloatLength; n++)
                    {
                        int ch = n % channels;

                        // Cascaded filter to perform EQ
                        for (int band = 0; band < bandCount; band++)
                        {
                            inputDataInFloat[n] = filters[ch, band].Transform(inputDataInFloat[n]);
                        }
                    }
                }
            }
        }

        public void Close(MediaEffectClosedReason reason)
        {
            switch (reason)
            {
                case MediaEffectClosedReason.Done:
                case MediaEffectClosedReason.UnknownError:
                case MediaEffectClosedReason.UnsupportedEncodingFormat:
                default:
                    break;
                case MediaEffectClosedReason.EffectCurrentlyUnloaded:
                    if (filters != null)
                        for (int i = 0; i < filters.Rank; i++)
                        {
                            for (int j = 0; j < filters.GetLength(i); j++)
                            {
                                filters[i, j] = null;
                            }
                        }
                    channels = 0;
                    bandCount = 0;
                    filters = null;
                    break;
            }
        }

        public void DiscardQueuedFrames()
        {
        }

        public void SetProperties(IPropertySet configuration)
        {
            this.configuration = configuration;
        }
    }
}
