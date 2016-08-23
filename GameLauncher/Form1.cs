using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
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
        private int quality = 2;
        private bool fullScreen = true;

        public Form1()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon;
            MouseDown += Form1_MouseDown;

            quality = GameConfig.GetQuality;
            switch (quality)
            {
            case 0: label1.Text = "MINIMUM"; break;
            case 1: label1.Text = "MEDIUM"; break;
            case 2: label1.Text = "MAXIMUM"; break;
            default: label1.Text = "?" + GameConfig.GetQuality; break;
            }

            fullScreen = GameConfig.GetCurrentFullScreen;
            label2.Text = fullScreen ? "FULL SCREEN" : "WINDOW MODE";

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

        // quality
        private void label1_Click(object sender, EventArgs e)
        {
            if (++quality == 3)
            {
                quality = 0;
                label1.Text = "MINIMUM";
            }
            else if (quality == 2)
            {
                label1.Text = "MAXIMUM";
            }
            else if (quality == 1)
            {
                label1.Text = "MEDIUM";
            }
            else
            {
                label1.Text = "?" + quality;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            fullScreen = !fullScreen;
            label2.Text = fullScreen ? "FULL SCREEN" : "WINDOW MODE";
        }

        // START
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            GameConfig.SetQuality(quality, fullScreen);
            GameConfig.SaveConfigFile();

            if (startGame("game.exe")) Close();
            MessageBox.Show("Файл game.exe не найден.", "Ошибка");
        }

        // find and run game exe
        private static bool startGame(string fileName)
        {
            if (File.Exists(fileName))
            {
                Process.Start(fileName);
                return true;
            }
            return false;
        }

        // CLOSE
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            GameConfig.SetQuality(quality, fullScreen);
            GameConfig.SaveConfigFile();
            Close();
        }
    }
}
