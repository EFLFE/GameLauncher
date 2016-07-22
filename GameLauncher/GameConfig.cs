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
        private static int currentQuality;
        private static bool currentFullScreen;

        public static int GetQuality => currentQuality;
        public static bool GetCurrentFullScreen => currentFullScreen;

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
            data = new List<string[]>();

            if (iniFile == null)
            {
                // create default ini file
                if (!File.Exists(DEFAULT_FILE))
                {
                    throw new FileNotFoundException("Файл " + DEFAULT_FILE + " не найден.", DEFAULT_FILE);
                }
                File.Copy(DEFAULT_FILE, CONFIG_NAME + CONFIG_EXT);
                iniFile = CONFIG_NAME + CONFIG_EXT;
            }

            using (var file = File.OpenText(iniFile))
            {
                string[] defq = file.ReadLine().Trim().Split(' ');

                if (defq.Length != 3)
                {
                    data.Add(new[] { "#", "2", "F" }); // default quality
                }
                else
                {
                    data.Add(defq);
                    currentQuality = Convert.ToInt32(defq[1]);
                    currentFullScreen = defq[2] == "F";
                }

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

        public static void SetQuality(int qv, bool fs)
        {
            currentQuality = qv;
            currentFullScreen = fs;

            foreach (var item in dataDic)
            {
                if (item.Key == "fullscreen")
                {
                    setDataValue("fullscreen", fs.ToString().ToLower());
                    continue;
                }
                setDataValue(item.Key, item.Value[qv]);
            }

            data[0][1] = qv.ToString();
            data[0][2] = fs ? "F" : "W";
        }

        public static void SaveConfigFile()
        {
            if (File.Exists(iniFile))
            {
                File.Delete(iniFile);
                System.Threading.Thread.Sleep(9);
            }

            using (var stream = File.CreateText(iniFile))
            {
                stream.WriteLine($"# {data[0][1]} {data[0][2]}");

                for (i = 1; i < data.Count; i++)
                {
                    if (data[i].Length == 1)
                        stream.WriteLine(data[i][0]);
                    else
                        stream.WriteLine($"{data[i][0]}={data[i][1]}");
                }
            }
        }

        // найти/перейти на найденую в строке позицию -> dataPos
        private static bool searchPos(string search, bool andFixPos = false)
        {
            for (i = 0; i < data.Count; i++)
            {
                if (data[i][0] == search)
                {
                    if (andFixPos)
                        dataPos = i;
                    return true;
                }
            }
            System.Diagnostics.Trace.TraceWarning("Search pos '" + search + "' not found.");
            return false;
        }

        private static void setDataValue(string search, string value, int offset = 0)
        {
            for (i = 0; i < data.Count; i++)
            {
                if (data[i][0] == search)
                {
                    data[i][1 + offset] = value;
                    return;
                }
            }
            throw new Exception($"Config data '{search}' not found.");
        }

        private static void searchConfigFile()
        {
            var iniFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.ini");

            if (iniFiles.Any(t => t.Contains(CONFIG_NAME + "-")))
            {
                // user conf
                iniFile = iniFiles.First(t => t.Contains(CONFIG_NAME + "-"));
            }
            else if (iniFiles.Any(t => t.Contains(CONFIG_NAME)))
            {
                iniFile = iniFiles.First(t => t.Contains(CONFIG_NAME));
            }
        }

    }
}
