using Nesdesign.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Nesdesign
{
    public class SettingsManager
    {
     
        private Dictionary<string, Setting> _settingsDict;


        private static SettingsManager _instance;
        public static SettingsManager Instance => _instance ?? new SettingsManager();

        public ObservableCollection<Setting> SettingsList { get; private set; }

        private const string SettingsFile = "settings.xml";

        public void InitializeDefaultSettings()
        {
            
            AddSetting("BASE_PATH", "C:\\nesdesign\\", "Ścieżka bazowa");
            AddSetting("OFFERS_PATH", "oferty", "Folder ofert");
            AddSetting("PROJECTS_PATH", "projekty", "Folder projekty");
            AddSetting("ORDERS_PATH", "zamowienia", "Folder zamówień");
            AddSetting("OFFER_TEMPLATE_PATH", "szablon_oferty", "Ścieżka szablonu oferty");
            AddSetting("PROJECT_TEMPLATE_PATH", "szablon_projektu", "Ścieżka szablonu projektu");
          
        }

        public void Setup()
        {
            LoadFromXml();
            if(SettingsList.Count == 0)
                InitializeDefaultSettings();
           
        }

        public SettingsManager()
        {
     
            _instance = this;
            SettingsList = new ObservableCollection<Setting>();
            _settingsDict = new Dictionary<string, Setting>();
            Setup();
        }

        public void AddSetting(string key, string value, string description)
        {
            var setting = new Setting { Key = key, Value = value, Description = description };
            SettingsList.Add(setting);
            _settingsDict[key] = setting;
        }

        public Setting GetSetting(string key)
        {
            return _settingsDict.ContainsKey(key) ? _settingsDict[key] : null;
        }

        public string GetValue(string key)
        {
            return _settingsDict.ContainsKey(key) ? _settingsDict[key].Value : null;
        }

        public void SetValue(string key, string value)
        {
            if (_settingsDict.ContainsKey(key))
            {
                _settingsDict[key].Value = value;
            }
        }
    

     // ===============================
    // ZAPIS DO XML
    // ===============================
    public void SaveToXml()
        {
            var serializer = new XmlSerializer(typeof(List<Setting>));

            using var stream = new FileStream(SettingsFile, FileMode.Create);
            serializer.Serialize(stream, new List<Setting>(SettingsList));
        }

        // ===============================
        // ODCZYT Z XML
        // ===============================
        public void LoadFromXml()
        {
            if (!File.Exists(SettingsFile))
                return;

            var serializer = new XmlSerializer(typeof(List<Setting>));

            using var stream = new FileStream(SettingsFile, FileMode.Open);
            var loadedSettings = (List<Setting>)serializer.Deserialize(stream);

            SettingsList.Clear();
            _settingsDict.Clear();

            foreach (var setting in loadedSettings)
            {
                SettingsList.Add(setting);
                _settingsDict[setting.Key] = setting;
            }
        }
    }
}
