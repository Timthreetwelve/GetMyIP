// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetMyIP
{
    public class UserSettings : SettingsManager<UserSettings>, INotifyPropertyChanged
    {
        #region Constructor
        public UserSettings()
        {
            // Set defaults
            GridZoom = 1;
            IncludeV6 = false;
            KeepOnTop = false;
            ShadeAltRows = true;
            URL = "http://ip-api.com/json/?fields=status,message,country,continent,regionName,city,zip,lat,lon,timezone,offset,isp,query";
            WindowLeft = 100;
            WindowTop = 100;
        }
        #endregion Constructor

        #region Properties
        public double GridZoom
        {
            get
            {
                if (gridZoom <= 0)
                {
                    gridZoom = 1;
                }
                return gridZoom;
            }
            set
            {
                gridZoom = value;
                OnPropertyChanged();
            }
        }

        public bool IncludeV6
        {
            get => includev6;
            set
            {
                includev6 = value;
                OnPropertyChanged();
            }
        }

        public bool KeepOnTop
        {
            get => keepOnTop;
            set
            {
                keepOnTop = value;
                OnPropertyChanged();
            }
        }

        public bool ShadeAltRows
        {
            get => shadeAltRows;
            set
            {
                shadeAltRows = value;
                OnPropertyChanged();
            }
        }

        public string URL
        {
            get => url;
            set
            {
                url = value;
                OnPropertyChanged();
            }
        }

        public double WindowLeft
        {
            get
            {
                if (windowLeft < 0)
                {
                    windowLeft = 100;
                }
                return windowLeft;
            }
            set => windowLeft = value;
        }

        public double WindowTop
        {
            get
            {
                if (windowTop < 0)
                {
                    windowTop = 100;
                }
                return windowTop;
            }
            set => windowTop = value;
        }
        #endregion Properties

        #region Private backing fields
        private double gridZoom;
        private bool includev6;
        private bool keepOnTop;
        private bool shadeAltRows;
        private double windowLeft;
        private double windowTop;
        private string url;
        #endregion Private backing fields

        #region Handle property change event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Handle property change event
    }
}
