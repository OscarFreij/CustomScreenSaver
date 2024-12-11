using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CustomScreenSaver
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void SaveSettings()
        {
            // Create or get existing Registry subkey
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\CustomScreenSaver");

            key.SetValue("path", textBox1.Text, RegistryValueKind.String);
            key.SetValue("mode", listBox1.Text, RegistryValueKind.String);
            key.SetValue("timeout", numericUpDown1.Value, RegistryValueKind.DWord);
        }

        private void LoadSettings()
        {
            // Get the value stored in the Registry
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\CustomScreenSaver");
            if (key == null)
            {
                textBox1.Text = string.Empty;
                listBox1.Text = Program.defaultImageMode;
                numericUpDown1.Value = Program.defaultTimeoutMs;
            }
            else
            {
                textBox1.Text = (string)key.GetValue("path");
                listBox1.Text = (string)key.GetValue("mode");
                numericUpDown1.Value = (int)key.GetValue("timeout");
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
