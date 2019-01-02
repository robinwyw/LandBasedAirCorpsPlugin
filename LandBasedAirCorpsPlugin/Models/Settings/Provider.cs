using System;
using System.IO;
using MetroTrilithon.Serialization;

namespace LandBasedAirCorpsPlugin.Models.Settings
{
    public static class Provider
    {
        public static string RoamingFolderPath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Jif_cr499", "KanColleViewer", nameof(LandBasedAirCorpsPlugin));

        public static string RoamingFilePath { get; } = Path.Combine(RoamingFolderPath, "Settings.xaml");

        public static ISerializationProvider Roaming { get; } = new FileSettingsProvider(RoamingFilePath);
    }
}
