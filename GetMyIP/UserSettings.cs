// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP
{
    public class UserSettings : SettingsManager<UserSettings>, INotifyPropertyChanged
    {
        #region Properties
        public int PrimaryColor
        {
            get => primaryColor;
            set
            {
                primaryColor = value;
                OnPropertyChanged();
            }
        }

        public int DarkMode
        {
            get => darkmode;
            set
            {
                darkmode = value;
                OnPropertyChanged();
            }
        }

        public bool IncludeDebug
        {
            get => includeDebug;
            set
            {
                includeDebug = value;
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

        public int InitialPage
        {
            get => initialPage;
            set
            {
                initialPage = value;
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

        public string URL
        {
            get => url;
            set
            {
                url = value;
                OnPropertyChanged();
            }
        }

        public int UISize
        {
            get => uiSize;
            set
            {
                uiSize = value;
                OnPropertyChanged();
            }
        }

        public double WindowHeight
        {
            get
            {
                if (windowHeight < 100)
                {
                    windowHeight = 100;
                }
                return windowHeight;
            }
            set => windowHeight = value;
        }

        public double WindowLeft
        {
            get
            {
                if (windowLeft < 0)
                {
                    windowLeft = 0;
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
                    windowTop = 0;
                }
                return windowTop;
            }
            set => windowTop = value;
        }

        public double WindowWidth
        {
            get
            {
                if (windowWidth < 100)
                {
                    windowWidth = 100;
                }
                return windowWidth;
            }
            set => windowWidth = value;
        }
        #endregion Properties

        #region Private backing fields with initial values
        private bool includeDebug;
        private bool includev6 = true;
        private bool keepOnTop;
        private double windowHeight = 575;
        private double windowLeft = 200;
        private double windowTop = 100;
        private double windowWidth = 600;
        private int darkmode;
        private int initialPage;
        private int primaryColor = 5;
        private int uiSize = 2;
        private string url = "http://ip-api.com/json/?fields=status,message,country,continent,regionName,city,zip,lat,lon,timezone,offset,isp,query";
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
