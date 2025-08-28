// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Net.NetworkInformation;
global using System.Reflection;
global using System.Runtime.InteropServices;
global using System.Runtime.Versioning;
global using System.Security.Principal;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Data;
global using System.Windows.Input;
global using System.Windows.Markup;
global using System.Windows.Media;

global using CommandLineParser.Arguments;
global using CommandLineParser.Exceptions;

global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

global using GetMyIP.Configuration;
global using GetMyIP.Constants;
global using GetMyIP.Converters;
global using GetMyIP.Dialogs;
global using GetMyIP.Helpers;
global using GetMyIP.Models;
global using GetMyIP.ViewModels;

global using H.NotifyIcon;
global using H.NotifyIcon.Core;

global using MaterialDesignColors;
global using MaterialDesignThemes.Wpf;

global using Microsoft.Win32;

global using NLog;
global using NLog.Config;
global using NLog.Targets;

global using static GetMyIP.Helpers.NLogHelpers;
global using static GetMyIP.Helpers.ResourceHelpers;
