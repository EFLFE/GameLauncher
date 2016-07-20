using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace GameLauncher
{
    internal static class GameConfig
    {
        private const string DEFAULT_FILE = "default.ini";
        private const string CONFIG_NAME = "gzdoom";
        private const string CONFIG_EXT  = ".ini";

        private static string iniFile;
        private static List<string[]> data;
        private static int dataPos;
        private static int i;

        #region DATA INIT (dataDic)

        private static readonly Dictionary<string, string[]> dataDic = new Dictionary<string, string[]>()
        {
            // NAME | Hight, Middle, Low
            ["gl_texture_filter"] =             new[]{ "4", "2", "0" },
            ["gl_texture_filter_anisotropic"] = new[]{ "16", "4", "1" },
            ["gl_texture_usehires"] =           new[]{ "true", "true", "false" },
            ["gl_texture_hqresize"] =           new[]{ "6", "3", "0" },

            //["gl_texture_hqresize_textures"] =  new[]{ "1", "1", "0" },
            //["gl_texture_hqresize_sprites"] =   new[]{ "1", "1", "0" },
            //["gl_texture_hqresize_fonts"] =     new[]{ "1", "1", "0" },

            ["gl_render_precise"] =             new[]{ "true", "false", "false" },
            ["gl_lights"] =                     new[]{ "true", "true", "false" },
            ["gl_sprite_blend"] =               new[]{ "true", "false", "false" },

            ["fullscreen"] =                    new[]{ "true", "false" }
        };

        #endregion

        public static void LoadConfigFile()
        {
            searchConfigFile();
            if (iniFile == null)
            {
                // create default ini file
                if (!File.Exists(DEFAULT_FILE))
                {
                    throw new FileNotFoundException("Файл " + DEFAULT_FILE + " не найден.", DEFAULT_FILE);
                }
                File.Copy(DEFAULT_FILE, CONFIG_NAME + CONFIG_NAME);
            }

            data = new List<string[]>();

            using (var file = File.OpenText(iniFile))
            {
                while (!file.EndOfStream)
                {
                    string t = file.ReadLine().Trim();
                    if (t.Length > 3)
                    {
                        if (t.Contains("=") && t[0] != '[')
                        {
                            data.Add(t.Split(new[] { '=' }, 2));
                        }
                        else
                        {
                            data.Add(new[] { t });
                        }
                    }
                }
            }

            // поиск и фиксация всех необходимых данных
            searchPos("[GlobalSettings]", true);
            int globalSettingsPos = dataPos + 1;

            foreach (var item in dataDic)
            {
                if (!searchPos(item.Key))
                {
                    // add
                    data.Insert(globalSettingsPos, new[] { item.Key, item.Value[0] });
                }
            }

        }

        // найти/перейти на найденую в строке позицию
        private static bool searchPos(string search, bool andFixPos = false)
        {
            for (i = 0; i < data.Count; i++)
            {
                if (data[i][0].Contains(search))
                {
                    if (andFixPos)
                        dataPos = i;
                    return true;
                }
            }
            return false;
        }

        private static void searchConfigFile()
        {
            var iniFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.ini");

            if (iniFiles.Contains(CONFIG_NAME + "-"))
            {
                // user conf
                iniFile = iniFiles.First(t => t.Contains(CONFIG_NAME + "-"));
            }
            else if (iniFiles.Contains(CONFIG_NAME))
            {
                iniFile = iniFiles.First(t => t.Contains(CONFIG_NAME));
            }
        }

    }
}
