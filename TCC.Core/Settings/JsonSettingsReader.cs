﻿using Newtonsoft.Json;
using System.IO;
using System.Windows;
using TCC.UI.Windows;
using TCC.Utils;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.Settings
{
    public class JsonSettingsReader : SettingsReaderBase
    {
        public JsonSettingsReader()
        {
            FileName = SettingsGlobals.SettingsFileName;
        }
        public SettingsContainer LoadSettings(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var file = File.ReadAllText(path);
                    #region Compatibility
                    file = file.Replace("\"TabName\"", "\"Name\"")
                               .Replace("\"ExcludedAuthors\"", "\"HiddenAuthors\"")
                               .Replace("\"ExcludedChannels\"", "\"HiddenChannels\"")
                               .Replace("\"Channels\"", "\"ShowedChannels\"")
                               .Replace("\"Authors\"", "\"ShowedAuthors\"")
                               .Replace("\"LanguageOverride\": \"\"", "\"LanguageOverride\" : 0");

                    #endregion
                    return JsonConvert.DeserializeObject<SettingsContainer>(file);
                }
//                else
//                {
//#if false
//                    var res = TccMessageBox.Show(SR.SettingsNotFoundImport, MessageBoxType.ConfirmationWithYesNo);
//                    if (res == MessageBoxResult.No)
//                    {
//                        App.Settings = new SettingsContainer();
//                        return;
//                    }
//                    var diag = new OpenFileDialog
//                    {
//                        Title = $"Import TCC settings file ({FileName})",
//                        Filter = $"{FileName} (*.json)|*.json"
//                    };
//                    if (diag.ShowDialog() == true)
//                    {
//                        path = diag.FileName;
//                        LoadSettings(path);
//                    }
//                    else App.Settings = new SettingsContainer();
//#else
//                    return new SettingsContainer();
//#endif
//                }
            }
            catch
            {
                var res = TccMessageBox.Show("TCC", SR.SettingsNotFoundDefault, MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (res == MessageBoxResult.Yes) File.Delete(path);
                LoadSettings(path);
            }
            return new SettingsContainer();
        }
    }
}
