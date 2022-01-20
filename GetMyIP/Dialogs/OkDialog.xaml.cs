// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Windows.Controls;

namespace GetMyIP.Dialogs
{
    /// <summary>
    /// A dialog to display a message with an OK button.
    /// </summary>
    public partial class OkDialog : UserControl
    {
        /// <summary>
        /// Message to be displayed
        /// </summary>
        public string Message { get; set; }

        public OkDialog()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}