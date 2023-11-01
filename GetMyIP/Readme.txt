The Get My IP ReadMe file


Introduction
============
Get My IP shows IP and geolocation information for your computer. You can optionally show a
user-configurable icon in the system tray. You also write information about the external IP address
to a log file of your choice.

Specifically, it will display:

    • The internal IP address(es)
    • The external (public) IP address
    • City, State (or region) and Zip Code (or postal code)
    • Country and Continent
    • Longitude and Latitude
    • Time Zone
    • Time offset from UTC
    • ISP (internet provider) name
    • Autonomous System (AS) number
    • Autonomous System (AS) name

This geolocation information, supplied by your ISP, is available to any website that you visit.

At the upper right, there is a map icon. Clicking on it will open the default browser to a map
centered on the longitude and latitude pulled from the external IP information.

Next to the map icon is a refresh icon. Click on it to refresh all IP address information.


The Pages
=========
Navigate between pages using the menu on the left side.

The first page shows the internal IP address(es). It will show IPv4 and, optionally, IPv6 address(es).

The second page shows the external IP address and geolocation information. The details listed depend
on which IP information provider is selected.

The third page is the Settings page. Settings are covered in the next section.

The last page is the About page. It contains information such as the version number and a link to its
GitHub page. At the bottom of the information is a link that, when clicked, will check to see if a new
version is available.

There are additional options available when clicking on the three-dot menu in the upper right. You can
save the IP information to a text file or copy it to the Windows clipboard. You can also open the
application log file or open this ReadMe file.


Settings
========
There are five sections on the Settings page. Click on the chevron on the right to expand the section.

    Application Settings
    --------------------
    There are options to select the initial page shown, to toggle the display of internal IPv6 addresses in
    addition to IPv4 addresses, to control the verbosity of the temp log file and to choose a map provider.

    Starting in version 0.9.2, you can choose the external IP information provider. Also, beginning in version
    0.9.2, you can choose to obfuscate sensitive IP information in the log file. You may want to do this if
    you need to open an issue and attach the log file.

    UI settings
    -----------
    Here you will find options to set the theme (Light, Material Dark, Darker, or System), the UI size,
    the accent color, and row spacing. There are also options to start the application centered on the
    screen and to keep the window on top of other applications.

    Tray Icon Settings
    ------------------
    If you wish to display an icon in the system tray, check the "Minimize to tray and enable tray icon"
    checkbox. Once checked, the remaining checkboxes are enabled. Check the box corresponding to the
    information that you want displayed in the tray icon tooltip. The internal IPv6 option will be
    disabled if IPv6 is not checked in the Application Settings section. If the minimize to tray option
    is checked and no other options are checked, then the tooltip will display the application name.
    After changing any of the tooltip information options, click the "Refresh Tooltip" button or restart
    the application. Due to limitations beyond my control, the total size of the tooltip is limited to
    about 127 bytes. There is a counter next to the refresh button that displays the current tooltip size.
    The size is updated after the refresh button is clicked.

    The tray icon has a right-click context menu that has options for showing the main window, refreshing
    the IP information, and exiting the application.

    Permanent Log File Settings
    ---------------------------
    If you don't intend to keep a permanent log file you can ignore this section.

    There is a text box where you can supply a log file name. Please supply the full path (without quotes)
    to a log file. If the log file doesn't exist, it will be created. There are buttons for testing and
    viewing this log file.

    Language Settings
    -----------------
    You can choose the language used for the user interface, provided that a translation has been
    contributed for that language. Checking the ""Use Windows display language"" check box will tell
    the app to use the language specified in the Windows settings, which will be used if a translation
    is available; otherwise, English (en-US) will be used. The drop-down allows you to choose a
    specific language from the list of defined languages. Changing the language will cause the
    application to restart.

    Starting in version, language contributors can test a language file prior to submitting it. See
    https://github.com/Timthreetwelve/GetMyIP/wiki/Testing-a-Language-File for details.


Logging
=======
To write the external IP information to a log file, make sure that there is a path in the Log File Name
text box. Click the Test Logging button to ensure that the information is indeed written to the file.
Once the log file has been written to, you can click the View Log button to see the contents of the log.

If you execute GetMyIP.exe with either -h or --hide as an argument, the information will be written
to the log file and the program will shutdown without showing a window. This can be done from a shortcut,
a batch file, or ideally, via Task Scheduler.


Keyboard Shortcuts
==================
These keyboard shortcuts are available:

  F1 = Go to the About screen
  F5 = Refresh (same as clicking the Refresh icon)
  Ctrl + Comma = Go to Settings
  Ctrl + C = Copy IP address information to the keyboard
  Ctrl + Numpad Plus = Increase size
  Ctrl + Numpad Minus = Decrease size
  Ctrl + Shift + C = Change the accent Color
  Ctrl + Shift + T = Change the Theme


Uninstalling GetMyIP
====================
To uninstall, use the regular Windows add/remove programs feature.


Notices and License
===================
Get My IP was written in C# by Tim Kennedy.

Get My IP uses the following packages and applications:

    * Material Design in XAML Toolkit https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit

    * CommandLineParser https://github.com/commandlineparser/commandline

    * CommunityToolkit.Mvvm https://github.com/CommunityToolkit/dotnet

    * NLog https://nlog-project.org/

    * GitVersion https://github.com/GitTools/GitVersion

    * H.NotifyIcon.Wpf https://github.com/HavenDV/H.NotifyIcon

    * Octokit https://github.com/octokit/octokit.net

    * GitKraken was used for everything Git related. https://www.gitkraken.com/

    * Inno Setup was used to create the installer. https://jrsoftware.org/isinfo.php

    * Visual Studio Community was used throughout the development of Get My IP. https://visualstudio.microsoft.com/vs/community/

    * XAML Styler is indispensable when working with XAML. https://github.com/Xavalon/XamlStyler

    * And of course, the essential PowerToys https://github.com/microsoft/PowerToys


Get My IP uses IP address and geolocation data from https://ip-api.com/. Note that there are usage limits.
Do not execute this program more than 45 times per minute. (Limit info as of October 16, 2023)

Get My IP optionally uses IP address information from https://ipext.org/. As of October 28th, 2023, ipext.org
did not list any usage limits.


MIT License
Copyright (c) 2019-2023 Tim Kennedy

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject
to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
