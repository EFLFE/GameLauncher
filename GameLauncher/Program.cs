using System;
using System.Windows.Forms;

namespace GameLauncher
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
            GameConfig.LoadConfigFile();
#else
            try
            {
                GameConfig.LoadConfigFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки конфигураций");
                return;
            }
#endif

            Application.Run(new Form1());
        }
    }
}
