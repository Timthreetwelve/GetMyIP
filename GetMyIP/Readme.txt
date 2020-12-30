The GetMyIP ReadMe file


Introduction
============

GetMyIP shows IP and geolocation information for your computer.

Specifically it will display:

	• The internal IP address
	• The external (public) IP address
	• City, State and Zip Code
	• Longitude and Latitude
	• Time Zone
	• ISP name

This geolocation information, supplied by your ISP, is available to any website that you visit.


Program Options
===============

	• Show on map: Opens LatLong.net with your default browser, showing the latitude and longitude
	  found in the geolocation information.
	• Copy to clipboard: Copies the geolocation information to the clipboard in tab delimited
	  format.
	• Save to text file: Saves the geolocation information to a text file in tab delimited format.
	• Shade alternate rows: Colors alternate rows in the data grid to improve readability.

The window position alternate row shading preference will be remembered from session to session.

To uninstall, use the regular Windows add/remove programs feature.


Notices and License
===================

GetMyIP was written in C# by Tim Kennedy.

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


Privacy
=======

Links to privacy policies can be found in the Third party tools and resources section in the
About menu item.


Acknowledgments
===============

IP address and geolocation information is from ip-api.com. Note that there are usage limits.
Do not execute this program more than 45 times per minute. (Limit info as of 12/29/2020)

Json.net v12.0.2 from Newtonsoft is used to handle the json formatted data received from
ip-api.com.

Menu icons are from Yusuke Kamiyamane.

Mapping is from LatLong.net.

