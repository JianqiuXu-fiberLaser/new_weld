using RobotWeld.Weldding;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld.ViewModel
{
    /// <summary>
    /// the data for the SelectMaterial window.
    /// </summary>
    public class SelectMaterialViewModel : INotifyPropertyChanged
    {
        private MetalMaterial _metalMaterial;

        public SelectMaterialViewModel()
        {
            _metalMaterial = new MetalMaterial();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public int MaterialType
        {
            get { return _metalMaterial.MaterialType; }
            set
            {
                _metalMaterial.MaterialType = value;
                OnPropertyChanged();
            }
        }

        public double SheetThickness
        {
            get { return _metalMaterial.Thickness; }
            set
            {
                _metalMaterial.Thickness = value;
                OnPropertyChanged();
            }
        }

        public double WireDiameter
        {
            get { return _metalMaterial.WireDiameter; }
            set
            {
                _metalMaterial.WireDiameter = value;
                OnPropertyChanged();
            }
        }
    }
}