The GetMyIP ReadMe file


Introduction
============
GetMyIP shows IP and geolocation information for your computer.

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


The Pages
=========
Click the hamburger menu on the left for page navigation.

The first page shows internal IP address(es). It will show IPv4 and optionally IPv6 addresses.

The second page shows the external IP address and geolocation information.

The third page is the settings page. The top section has options for the initial view, the UI size,
the theme (light, dark or system), and the accent color. The bottom section has options to toggle the
display of internal IPv6 addresses in addition to IPv4 addresses, keep the window on top and to
control the verbosity of the log file.

The last page is the About page. It contains information such as the version number and a link to its
GitHub page.

There are additional options available when clicking on the three-dot menu on the right.


Uninstalling GetMyIP
====================
To uninstall, use the regular Windows add/remove programs feature.


Notices and License
===================

GetMyIP was written in C# by Tim Kennedy.

GetMyIP uses the following icons & packages:

	• Material Design in XAML Toolkit https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit

	• Json.net https://www.newtonsoft.com/json

	• NLog https://nlog-project.org/

	• Inno Setup was used to create the installer. https://jrsoftware.org/isinfo.php


GetMyIP uses IP address and geolocation data from ip-api.com. Note that there are usage limits.
Do not execute this program more than 45 times per minute. (Limit info as of January 9, 2022)



MIT License
Copyright (c) 2019 - 2022 Tim Kennedy

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
