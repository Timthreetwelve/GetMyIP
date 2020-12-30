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


Program Options
===============
On the File menu:

	• Copy to clipboard: Copies the geolocation information to the clipboard in tab delimited
	  format.

	• Save to text file: Saves the geolocation information to a text file in tab delimited format.

On the Options menu:

	• Show on map: Opens LatLong.net with your default browser, showing the latitude and longitude
	  found in the geolocation information.

	• Include IPv6 addresses: Toggles the display of IPv6 addresses in addition to IPv4 addresses.

	• Shade alternate rows: Colors alternate rows in the data grid to improve readability.

	• Keep on Top: Keep the GetMyIP window on top of other windows.

	• Zoom: Change the zoom level.


Uninstalling GetMyIP
====================
To uninstall, use the regular Windows add/remove programs feature.


Notices and License
===================

GetMyIP was written in C# by Tim Kennedy.

GetMyIP uses the following icons & packages:

	• Fugue Icons set https://p.yusukekamiyamane.com/

	• Json.net v12.0.3 from Newtonsoft https://www.newtonsoft.com/json

	• NLog v4.7.6 https://nlog-project.org/

	• Inno Setup was used to create the installer. https://jrsoftware.org/isinfo.php


GetMyIP uses IP address and geolocation data from ip-api.com. Note that there are usage limits.
Do not execute this program more than 45 times per minute. (Limit info as of 12/29/2020)



MIT License
Copyright (c) 2020 Tim Kennedy

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
