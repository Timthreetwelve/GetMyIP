The GetMyIP ReadMe file


Introduction
============
GetMyIP shows IP and geolocation information for your computer. You can optionally write this
information to a log file.

Specifically it will display:

	• The internal IP address(es)
	• The external (public) IP address
	• City, State and Zip Code
	• Country and Continent
	• Longitude and Latitude
	• Time Zone
	• Time offset from UTC
	• ISP name

This geolocation information, supplied by your ISP, is available to any website that you visit.

At the upper right there is a map icon. Clicking on it will open the default browser to a map
centered on the longitude and latitude pulled from the external IP information.


The Pages
=========
Navigate between pages using the menu on the left side.

The first page shows internal IP address(es). It will show IPv4 and optionally IPv6 addresses.

The second page shows the external IP address and geolocation information.

The third page is the settings page. There are options for the initial view, the UI size,the theme
(light, dark, darker or system), and the accent color, row spacing and the map provider.

Below that you will find options to toggle the display of internal IPv6 addresses in addition to
IPv4 addresses, keep the window on top and to control the verbosity of the log file.

Lastly, there is a text box where you can supply a log file name. Please supply
the full path to a log file. If the log file doesn't exist, it will be created.

The last page is the About page. It contains information such as the version number and a link to its
GitHub page.

There are additional options available when clicking on the three-dot menu in the upper right. You can
save the IP information to a text file or copy it to the Windows clipboard. You can also open the
application log file or open this ReadMe file.


Logging
=======
To write the external IP information to a log file, make sure that there is a path in the Log File Name
text box. Click the Test Logging button to ensure that the information is indeed written to the file.
Once the log file has been written to, you can click the View Log button to see the contents of the log.

If you execute GetMyIP.exe with either /hide or /write as an argument, the information will be written
to the log file and the program will shutdown without showing a window. This can be done from a shortcut,
a batch file or ideally, in Task Scheduler.


Uninstalling GetMyIP
====================
To uninstall, use the regular Windows add/remove programs feature.


Notices and License
===================
Get My IP was written in C# by Tim Kennedy.

Get My IP uses the following icons & packages:

	• Material Design in XAML Toolkit https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit

	• Material Design Extensions https://spiegelp.github.io/MaterialDesignExtensions/

	• NLog https://nlog-project.org/

	• Inno Setup was used to create the installer. https://jrsoftware.org/isinfo.php


Get My IP uses IP address and geolocation data from ip-api.com. Note that there are usage limits.
Do not execute this program more than 45 times per minute. (Limit info as of January 3, 2023)



MIT License
Copyright (c) 2019 - 2023 Tim Kennedy

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
