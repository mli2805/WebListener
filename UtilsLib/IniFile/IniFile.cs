using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilsLib
{
    public class IniFile
    {
        public string FilePath { get; private set; }
        private readonly object _obj = new object();

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// should be done separately from creation for tests sake
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="isFileReady"></param>
        public IniFile AssignFile(string filename, bool isFileReady = false)
        {
            lock (_obj)
            {
                FilePath = isFileReady ? filename : Utils.FileNameForSure(@"..\Ini\", filename, false);
            }
            return this;
        }

        #region Base (String)

        public void Write(IniSection section, IniKey key, string value)
        {
            lock (_obj)
            {
                WritePrivateProfileString(section.ToString(), key.ToString(), value, FilePath);
            }
        }

        public string Read(IniSection section, IniKey key, string defaultValue)
        {
            lock (_obj)
            {
                StringBuilder temp = new StringBuilder(255);
                if (GetPrivateProfileString(section.ToString(), key.ToString(), "", temp, 255, FilePath) != 0)
                {
                    return temp.ToString();
                }

                Write(section, key, defaultValue);
                return defaultValue;
            }
        }

        public string ReadForeignIni(string filepath, IniSection section, IniKey key)
        {
            StringBuilder temp = new StringBuilder(255);
            if (GetPrivateProfileString(section.ToString(), key.ToString(), "", temp, 255, filepath) != 0)
            {
                return temp.ToString();
            }
            return null;
        }

        public void DeleteKey(IniSection section, IniKey key)
        {
            lock (_obj)
            {
                WritePrivateProfileString(section.ToString(), key.ToString(), null, FilePath);
            }
        }

        #endregion

        public void DeleteSection(IniSection section)
        {
            lock (_obj)
            {
                WritePrivateProfileString(section.ToString(), null, null, FilePath);
            }
        }

        #region Extensions (Other types)

        public void Write(IniSection section, IniKey key, bool value)
        {
            Write(section, key, value.ToString());
        }

        public void Write(IniSection section, IniKey key, int value)
        {
            Write(section, key, value.ToString());
        }

        public void Write(IniSection section, IniKey key, double value)
        {
            Write(section, key, value.ToString(CultureInfo.InvariantCulture));
        }

        public bool Read(IniSection section, IniKey key, bool defaultValue)
        {
            return bool.TryParse(Read(section, key, defaultValue.ToString()), out var result) ? result : defaultValue;
        }

        public int Read(IniSection section, IniKey key, int defaultValue)
        {
            return int.TryParse(Read(section, key, defaultValue.ToString()), out var result) ? result : defaultValue;
        }

        public double Read(IniSection section, IniKey key, double defaultValue)
        {
            var str = Read(section, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            return double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
        }

        #endregion
    }
}