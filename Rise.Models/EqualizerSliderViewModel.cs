using System.ComponentModel;

namespace Rise.Models
{
    public class EqualizerSliderViewModel : INotifyPropertyChanged
    {
        private float gain;

        public float Gain
        {
            get => gain;
            set
            {
                gain = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gain)));
            }
        }

        public int Index { get; set; }

        public string HzText => Index switch
        {
            0 => "30",
            1 => "75",
            2 => "150",
            3 => "300",
            4 => "600",
            5 => "1.2k",
            6 => "2.5k",
            7 => "5k",
            8 => "10k",
            9 => "20k",
            _ => ""
        };

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
