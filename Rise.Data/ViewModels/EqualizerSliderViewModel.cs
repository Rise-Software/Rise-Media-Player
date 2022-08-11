namespace Rise.Data.ViewModels
{
    public class EqualizerSliderViewModel : ViewModel
    {
        private float _gain;
        public float Gain
        {
            get => _gain;
            set => Set(ref _gain, value);
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
    }
}
