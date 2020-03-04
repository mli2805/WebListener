using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BanksListener
{
    public static class PathFinder
    {
      
        // ReSharper disable once UnusedMember.Local
        public static string GetDropboxPath()
        {
            const string infoPath = @"Dropbox\info.json";
            var jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData") ?? throw new InvalidOperationException(), infoPath);
            if (!File.Exists(jsonPath)) jsonPath = Path.Combine(Environment.GetEnvironmentVariable("AppData") ?? throw new InvalidOperationException(), infoPath);
            if (!File.Exists(jsonPath)) throw new Exception("Dropbox could not be found!");
            var strings = File.ReadAllText(jsonPath).Split('\"');
            var index = strings.ToList().IndexOf("path");
            var dropboxPath = strings[index + 2].Replace(@"\\", @"\");
            return dropboxPath;
        }

        /// <summary>
        /// Retrieves the local Google Drive directory, if any.
        /// </summary>
        /// <returns>Directory, or string.Empty if it can't be found</returns>
        // ReSharper disable once UnusedMember.Local
        public static string GetGoogleDriveDirectory()
        {
            try
            {
                // Google Drive's sync database can be in a couple different locations. Go find it. 
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string dbName = "sync_config.db";
                var pathsToTry = new[] { @"Google\Drive\" + dbName, @"Google\Drive\user_default\" + dbName };

                string syncDbPath = (from p in pathsToTry
                        where File.Exists(Path.Combine(appDataPath, p))
                        select Path.Combine(appDataPath, p))
                    .FirstOrDefault();
                if (syncDbPath == null)
                    throw new FileNotFoundException("Cannot find Google Drive sync database", dbName);

                // Build the connection and sql command
                string conString = string.Format(@"Data Source='{0}';Version=3;New=False;Compress=True;", syncDbPath);
                using (var con = new SQLiteConnection(conString))
                using (var cmd = new SQLiteCommand("select * from data where entry_key='local_sync_root_path'", con))
                {
                    // Open the connection and execute the command
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    reader.Read();

                    // Extract the data from the reader
                    string path = reader["data_value"]?.ToString();
                    if (string.IsNullOrWhiteSpace(path))
                        throw new InvalidDataException("Cannot read 'local_sync_root_path' from Google Drive configuration db");

                    // By default, the path will be prefixed with "\\?\" (unless another app has explicitly changed it).
                    // \\?\ indicates to Win32 that the filename may be longer than MAX_PATH (see MSDN). 
                    // Parts of .NET (e.g. the File class) don't handle this very well, so remove this prefix.
                    if (path.StartsWith(@"\\?\"))
                        path = path.Substring(@"\\?\".Length);

                    return path;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Cannot determine Google Drive location. Error {0} - {1}", ex.Message, ex.StackTrace);
                return string.Empty;
            }
        }

    }
}