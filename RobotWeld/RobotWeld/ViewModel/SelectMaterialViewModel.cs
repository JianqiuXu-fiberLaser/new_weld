using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using RobotWeld.Weldding;
using RobotWeld.AlgorithmsBase;

namespace RobotWeld.ViewModel
{
    /// <summary>
    /// the data for the SelectMaterial window.
    /// </summary>
    public class SelectMaterialViewModel : INotifyPropertyChanged
    {
        private MetalMaterial ? _metalMaterial;
        private string _materialName = "";
        private string _stringSheetThickness = "";
        private string _stringWireDiameter = "";

        private int _fileIndex = 0;
        private int _materialType = 0;
        private int _oldMaterialType = 0;
        private double _oldSheetThickness = 0;
        private double _oldWireDiameter = 0;

        public delegate void SendFile(int filename);
        public SendFile ? sendfile;

        public SelectMaterialViewModel() 
        {
            _metalMaterial = new MetalMaterial();
        }
        
        // get the information in the  material file.
        public void GetfileInfo(int fileMaterial)
        {         
            if (_metalMaterial == null) { return; }

            string line;
            try
            {
                string fname = "./Stuff/" + fileMaterial.ToString() + ".ma";
                FileStream rfile = new(fname, FileMode.OpenOrCreate);
                StreamReader sr = new(rfile);

                line = sr.ReadLine() ?? "1";
                MaterialName = line;
                _oldMaterialType = Convert.ToInt32(line);
                MaterialType = _oldMaterialType;

                line = sr.ReadLine() ?? "0";
                SheetThickness = line;
                _oldSheetThickness = Convert.ToDouble(line);

                line = sr.ReadLine() ?? "0";
                WireDiameter = line;
                _oldWireDiameter = Convert.ToDouble(line);

                sr.Close();

            }
            catch (Exception ex)
            {
                new Werr().WaringMessage(ex.Message);
            }
        }

        // set material information when exit dialog
        public void SetFileMaterial(bool isChanged)
        {
            if (_metalMaterial == null) { return; }

            if (isChanged) 
            {
                if((_metalMaterial.MaterialType == _oldMaterialType) &&
                    (_metalMaterial.Thickness == _oldSheetThickness) &&
                    (_metalMaterial.WireDiameter == _oldWireDiameter))
                {
                    return;
                }

                // delivary the fileindex to the dialog.
                if (sendfile != null)
                {
                    DateTime dateTime2020 = new DateTime(2020, 2, 14, 0, 0, 0);
                    DateTime DateNow = DateTime.Now;
                    TimeSpan timespan = (DateNow - dateTime2020);
                    FileIndex = (int)timespan.TotalSeconds;

                    FileStream afile = new FileStream("./Stuff/" +
                        FileIndex.ToString() + ".ma", FileMode.Create);
                    StreamWriter sw = new StreamWriter(afile);

                    sw.WriteLine(_metalMaterial.MaterialType.ToString());
                    sw.WriteLine(_metalMaterial.Thickness.ToString());
                    sw.WriteLine(_metalMaterial.WireDiameter.ToString());
                    sw.Close();

                    sendfile(FileIndex);
                }
            }
        }

        public int MaterialType
        {
            get { return _materialType; }
            set 
            { 
                _materialType = value;
                OnPropertyChanged();
            }
        }

        // get and set the properties.
        public string MaterialName
        {
            get { return _materialName; }
            set 
            {
                _materialName = value;
                OnPropertyChanged();

                if (_metalMaterial == null) { return; }
                if (_materialName != value) 
                {
                    if (int.TryParse(_materialName, out int varInt))
                    {
                        _metalMaterial.MaterialType = varInt;
                        MaterialType = varInt;
                    }
                }
            }
        }

        public string SheetThickness
        {
            get { return _stringSheetThickness; }
            set
            {
                if (_metalMaterial == null) { return; }
                try
                {
                    double var = Convert.ToDouble(value);

                    if (var >= 0 && var <= 20)
                    {
                        _metalMaterial.Thickness = var;
                    }

                    _stringSheetThickness = value;
                    OnPropertyChanged();
                }
                catch (FormatException ex)
                {
                    new Werr().WaringMessage(ex.Message);
                }
            }
        }

        public string WireDiameter
        {
            get { return _stringWireDiameter; }
            set
            {
                try
                {
                    if (_metalMaterial == null) { return; }

                    double var = Convert.ToDouble(value);
                    if (var >= 0 && var <= 3)
                    {
                        _metalMaterial.WireDiameter = var;
                        _stringWireDiameter = value;
                        OnPropertyChanged(WireDiameter);
                    }    
                }
                catch (FormatException ex)
                {
                    new Werr().WaringMessage(ex.Message);
                }
            }
        }

        //-- changed Events --
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int FileIndex 
        {
            get { return _fileIndex; }
            set { _fileIndex = value; } 
        }
    }
}