// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP
{
    public class UserSettings : SettingsManager<UserSettings>, INotifyPropertyChanged
    {
        #region Properties
        public int PrimaryColor
        {
            get => _primaryColor;
            set
            {
                _primaryColor = value;
                OnPropertyChanged();
            }
        }

        public int DarkMode
        {
            get => _darkmode;
            set
            {
                _darkmode = value;
                OnPropertyChanged();
            }
        }

        public bool IncludeDebug
        {
            get => _includeDebug;
            set
            {
                _includeDebug = value;
                OnPropertyChanged();
            }
        }

        public bool IncludeV6
        {
            get => _includev6;
            set
            {
                _includev6 = value;
                OnPropertyChanged();
            }
        }

        public int InitialPage
        {
            get => _initialPage;
            set
            {
                _initialPage = value;
                OnPropertyChanged();
            }
        }

        public bool KeepOnTop
        {
            get => _keepOnTop;
            set
            {
                _keepOnTop = value;
                OnPropertyChanged();
            }
        }

        public string URL
        {
            get => _url;
            set
            {
                _url = value;
                OnPropertyChanged();
            }
        }

        public int UISize
        {
            get => _uiSize;
            set
            {
                _uiSize = value;
                OnPropertyChanged();
            }
        }

        public double WindowHeight
        {
            get
            {
                if (_windowHeight < 100)
                {
                    _windowHeight = 100;
                }
                return _windowHeight;
            }
            set => _windowHeight = value;
        }

        public double WindowLeft
        {
            get
            {
                if (_windowLeft < 0)
                {
                    _windowLeft = 0;
                }
                return _windowLeft;
            }
            set => _windowLeft = value;
        }

        public double WindowTop
        {
            get
            {
                if (_windowTop < 0)
                {
                    _windowTop = 0;
                }
                return _windowTop;
            }
            set => _windowTop = value;
        }

        public double WindowWidth
        {
            get
            {
                if (_windowWidth < 100)
                {
                    _windowWidth = 100;
                }
                return _windowWidth;
            }
            set => _windowWidth = value;
        }
        #endregion Properties

        #region Private backing fields with initial values
        private bool _includeDebug;
        private bool _includev6 = true;
        private bool _keepOnTop;
        private double _windowHeight = 575;
        private double _windowLeft = 200;
        private double _windowTop = 100;
        private double _windowWidth = 600;
        private int _darkmode;
        private int _initialPage;
        private int _primaryColor = 5;
        private int _uiSize = 2;
        private string _url = "http://ip-api.com/json/?fields=status,message,country,continent,regionName,city,zip,lat,lon,timezone,offset,isp,query";
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
