using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher
{
    internal partial class Form1 : Form
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        private Rectangle windowSize;

        public Form1()
        {
            InitializeComponent();
            MouseDown += Form1_MouseDown;

            try
            {
                GameConfig.LoadConfigFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки конфигураций");
            }

            // def desktop size
            windowSize = Screen.AllScreens[0].Bounds;
            defaultGameSizeLabel.Text = $"{windowSize.Width}x{windowSize.Height}";
        }

        // movement
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        // close
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
