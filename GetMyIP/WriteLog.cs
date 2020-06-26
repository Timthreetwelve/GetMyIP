// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

// The WriteLog class contains methods and a property that enable writing messages to a log file.
// There are two overloads defined for the WriteLogFile method, one that prepends a time stamp to
// the message, and one that doesn't.
//
// Three parameters are accepted,
// 1. a string for a path to the log file
// 2. a string for the message to be written to the file
// 3. a char for the time stamp
// 4. a char for brackets
//
// The WriteLogFile method returns a boolean value, true for success and false indicating an error.
//
// Example usage: bool result = WriteLog.WriteLogFile(logFilePath, logMessage(), 'U', 'Y' );

#region using directives

using System;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion using directives

namespace TKUtils
{
    /// <summary>
    ///  Class for writing log files
    /// </summary>
    public static class WriteLog
    {
        #region WL Message

        /// <summary>
        ///  Success or failure message string from the WriteLog class
        /// </summary>
        public static string WLMessage { get; set; }

        #endregion WL Message

        #region Write Log File

        /// <Summary>Writes to a log file. This overload is used when a time stamp is desired.</Summary>
        /// <param name="path">Path to log file</param>
        /// <param name="message">String to be written to log file</param>
        /// <param name="timeStampType">Valid time stamp chars are U, S, L, E, M, N, or X</param>
        /// <param name="useBrackets">Enclose time stamp with square brackets. Y or N</param>
        /// <returns>Bool value indicating success or failure</returns>
        public static bool WriteLogFile(string path, string message, char timeStampType, char useBrackets)
        {
            if (CheckLogFile(path, timeStampType, useBrackets))
            {
                string tsMessage = FormatTimeStamp(timeStampType, useBrackets) + message.TrimEnd() + Environment.NewLine;
                try
                {
                    File.AppendAllText(path, tsMessage);
                    WLMessage = "OK";
                    return true;
                }
                catch (Exception e)
                {
                    WLMessage = e.Message;
                    return false;
                }
            }
            return false;
        }

        /// <Summary>Writes to a log file. This overload is used when a time stamp is not desired</Summary>
        /// <param name="path">Path to log file</param>
        /// <param name="message">String to be written to log file</param>
        public static bool WriteLogFile(string path, string message)
        {
            if (CheckLogFile(path))
            {
                string tsMessage = message + Environment.NewLine;
                try
                {
                    File.AppendAllText(path, tsMessage);
                    WLMessage = "OK";
                    return true;
                }
                catch (Exception e)
                {
                    WLMessage = e.Message;
                    return false;
                }
            }
            return false;
        }

        #endregion Write Log File

        #region Write Temp File

        /// <Summary>Writes a string to a file in the user's Temp directory</Summary>
        /// <param name="msg">String to be written to temp file</param>
        /// <param name="filename">Optional file name</param>
        public static void WriteTempFile(string msg, string filename = "default")
        {
            WriteLogFile(GetTempFile(filename), msg, 'U', 'N');
        }

        #endregion Write Temp File

        #region Validate File Path

        /// <summary>
        ///  Validates that are no invalid characters, the drive exists, etc. Check
        ///  WriteLog.WLMessage for errors
        /// </summary>
        /// <param name="testPath">Complete path with file name.</param>
        /// <returns>True if no errors are found, otherwise returns False.</returns>
        public static bool ValidateFilePath(string testPath)
        {
            // Is file null or blanks?
            if (string.IsNullOrWhiteSpace(testPath))
            {
                WLMessage = "Error - Path is null, empty or white space";
                return false;
            }

            // Check path for invalid characters
            if (testPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                WLMessage = $"Error - Bad char(s) in path {testPath}";
                return false;
            }

            // Make sure that there's more than just a drive letter
            if (testPath.EndsWith(@":\"))
            {
                WLMessage = $"Error - Path appears to be just a drive letter {testPath}";
                return false;
            }

            try
            {
                string testFileName = Path.GetFileName(testPath);
                string testFileNameWithoutExt = Path.GetFileNameWithoutExtension(testPath);

                // Check file name for invalid characters
                if (testFileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    WLMessage = $"Error - Bad char(s) in file name {testFileName}";
                    return false;
                }

                // Check file name for reserved device names
                if ((new string[] {
                "AUX", "CON", "NUL", "PRN",
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
                }).Contains(testFileNameWithoutExt))
                {
                    WLMessage = $"Error - File name is reserved word {testFileNameWithoutExt}";
                    return false;
                }
            }
            catch (Exception e)
            {
                WLMessage = $"Error - {e.Message}";
                return false;
            }

            // Make sure path is rooted
            if (!Path.IsPathRooted(testPath))
            {
                WLMessage = "Error - Path must be rooted";
                return false;
            }

            // Make sure drive exists
            string root = Path.GetPathRoot(testPath);
            if (!Directory.GetLogicalDrives().Contains(root.ToUpper()))
            {
                WLMessage = "Error - Drive not found";
                return false;
            }

            // If we got here return true
            WLMessage = "OK";
            return true;
        }

        #endregion Validate File Path

        #region Get Temp File Name

        /// <Summary>Gets a file name for a file in the user's temp directory</Summary>
        /// <param name="filename">Optional file name</param>
        public static string GetTempFile(string filename = "default")
        {
            string myExe = Assembly.GetExecutingAssembly().GetName().Name;
            string tStamp = string.Format("{0:yyyyMMdd}", DateTime.Now);
            string path = Path.GetTempPath();
            if (filename == "default")
            {
                // Change filename depending on debug mode or not
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    filename = myExe + ".debug." + tStamp + ".log";
                }
                else
                {
                    filename = myExe + ".temp." + tStamp + ".log";
                }
            }

            return Path.Combine(path, filename);
        }

        #endregion Get Temp File Name

        #region Private Helper Methods

        // If the log file doesn't exist create it
        private static bool CheckLogFile(string path, char tstype, char brackets)
        {
            if (!File.Exists(path))
            {
                if (!ValidateFilePath(path))
                {
                    return false;
                }

                try
                {
                    string CreatedMsg = string.Format("{0}This log file created by {1}" + Environment.NewLine,
                            FormatTimeStamp(tstype, brackets), GetExeName());
                    File.WriteAllText(path, CreatedMsg.ToString());
                }
                catch (Exception e)
                {
                    WLMessage = $"Error - {e.Message}";
                    return false;
                }
                return true;
            }
            return true;
        }

        // If the log file doesn't exist create it
        private static bool CheckLogFile(string path)
        {
            if (!File.Exists(path))
            {
                if (!ValidateFilePath(path))
                {
                    return false;
                }
                try
                {
                    string CreatedMsg = string.Format("This log file created by {0}" + Environment.NewLine,
                            GetExeName());
                    File.WriteAllText(path, CreatedMsg.ToString());
                }
                catch (Exception e)
                {
                    WLMessage = $"Error - {e.Message}";
                    return false;
                }
                return true;
            }
            return true;
        }

        // Determine the name of our executable.
        private static string GetExeName()
        {
            return Path.GetFileName(Assembly.GetEntryAssembly().Location.ToString());
        }

        // Format the timestamp
        private static string FormatTimeStamp(char tstype, char brackets)
        {
            DateTime Now = DateTime.Now;
            char ts = char.ToUpper(tstype);
            char br = char.ToUpper(brackets);
            string TimeStamp;

            switch (ts)
            {
                case 'U': // US
                    TimeStamp = Now.ToString("[MM/dd/yyyy HH:mm:ss]  ");
                    break;

                case 'S': // Short
                    TimeStamp = Now.ToString("[MM/dd/yy HH:mm]  ");
                    break;

                case 'L': // Long
                    TimeStamp = Now.ToString("[MM/dd/yyyy HH:mm:ss.ffff]  ");
                    break;

                case 'E': // European
                    TimeStamp = Now.ToString("[yyyy/MM/dd HH:mm:ss]  ");
                    break;

                case 'M': // Month
                    TimeStamp = Now.ToString("[dd MMM yyyy HH:mm:ss]  ");
                    break;

                default:
                    TimeStamp = "";
                    break;
            }

            // No Brackets
            if (br != 'Y')
            {
                TimeStamp = TimeStamp.Replace("[", "").Replace("]", "");
            }
            return TimeStamp;
        }
        #endregion Private Helper Methods
    }
}