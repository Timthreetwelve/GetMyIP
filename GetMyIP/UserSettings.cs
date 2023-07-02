// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using GetMyIP.Models;

namespace GetMyIP;

/// <summary>
/// A class and methods for reading, updating and saving user settings in a JSON file
/// </summary>
public class UserSettings : SettingsManager<UserSettings>, INotifyPropertyChanged
{
    #region Properties
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

    public string LogFile
    {
        get => _logFile;
        set
        {
            _logFile = value;
            OnPropertyChanged();
        }
    }

    public int MapProvider
    {
        get => _mapProvider;
        set
        {
            _mapProvider = value;
            OnPropertyChanged();
        }
    }

    public bool MinimizeToTray
    {
        get { return _minimizeToTray; }
        set { _minimizeToTray = value;
            OnPropertyChanged();
        }
    }

    public int PrimaryColor
    {
        get => _primaryColor;
        set
        {
            _primaryColor = value;
            OnPropertyChanged();
        }
    }

    public int RowSpacing
    {
        get => _rowSpacing;
        set
        {
            _rowSpacing = value;
            OnPropertyChanged();
        }
    }

    public bool ShowCity
    {
        get => _showCity;
        set
        {
            _showCity = value;
            OnPropertyChanged();
        }
    }

    public bool ShowCountry
    {
        get => _showCountry;
        set
        {
            _showCountry = value;
            OnPropertyChanged();
        }
}

    public bool ShowExternalIP
    {
        get => _showExternalIP;
        set
        {
            _showExternalIP = value;
            OnPropertyChanged();
        }
    }

    public bool ShownInternalIPv4
    {
        get => _showInternalIPv4;
        set
        {
            _showInternalIPv4 = value;
            OnPropertyChanged();
        }
    }

    public bool ShownInternalIPv6
    {
        get => _showInternalIPv6;
        set
        {
            _showInternalIPv6 = value;
            OnPropertyChanged();
        }
    }

    public bool ShowISP
    {
        get => _showISP;
        set
        {
            _showISP = value;
            OnPropertyChanged();
        }
    }

    public bool ShowOffset
    {
        get => _showoffset;
    set
        {
            _showoffset = value;
            OnPropertyChanged();
        }
    }

    public bool ShowState
    {
        get => _showState;
        set
        {
    _showState = value;
            OnPropertyChanged();
        }
    }

    public bool ShowTimeZone
    {
        get => _showTimeZone;
        set
        {
            _showTimeZone = value;
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

    public string URL
    {
        get => _url;
        set
        {
            _url = value;
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
    private int _darkmode = (int)ThemeType.System;
    private bool _includeDebug = true;
    private bool _includev6 = true;
    private int _initialPage = (int)NavPage.External;
    private bool _keepOnTop;
    private string _logFile = string.Empty;
    private int _mapProvider = 1;
    private bool _minimizeToTray;
    private int _primaryColor = 5;
    private int _rowSpacing = (int)Spacing.Comfortable;
    private bool _showCity;
    private bool _showCountry = true;
    private bool _showExternalIP = true;
    private bool _showInternalIPv4 = true;
    private bool _showInternalIPv6;
    private bool _showISP;
    private bool _showoffset;
    private bool _showState;
    private bool _showTimeZone;
    private int _uiSize = (int)MySize.Default;
    private string _url = "http://ip-api.com/json/?fields=status,message,country,continent,regionName,city,zip,lat,lon,timezone,offset,isp,query";
    private double _windowHeight = 575;
    private double _windowLeft = 200;
    private double _windowWidth = 700;
    private double _windowTop = 100;
    #endregion Private backing fields

    #region Handle property change event
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion Handle property change event
}
