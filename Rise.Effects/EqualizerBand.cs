using System.ComponentModel;

namespace Rise.Effects
{
    public sealed class EqualizerBand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Index)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HzText)));
                }
            }
        }

        private float _gain;
        public float Gain
        {
            get => _gain;
            set
            {
                if (_gain != value)
                {
                    _gain = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gain)));
                }
            }
        }

        public string HzText
        {
            get
            {
                switch (Index)
                {
                    case 0: return "30";
                    case 1: return "75";
                    case 2: return "150";
                    case 3: return "300";
                    case 4: return "300";
                    case 5: return "1.2k";
                    case 6: return "2.5k";
                    case 7: return "5k";
                    case 8: return "10k";
                    case 9: return "20k";
                }
                return string.Empty;
            }
        }

        internal float Bandwidth { get; set; }
        internal float Frequency { get; set; }
    }
}
