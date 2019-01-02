using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using MetroTrilithon.Serialization;

namespace LandBasedAirCorpsPlugin.Models.Settings
{
    public static class PluginSettings
    {
        public static SerializableProperty<bool> NotifyRelocatingCompleted { get; }
            = new SerializableProperty<bool>(GetKey(), Provider.Roaming, true) { AutoSave = true };

        private static string GetKey([CallerMemberName]string propertyName = "")
        {
            return $"{nameof(LandBasedAirCorpsPlugin)}.{nameof(PluginSettings)}.{propertyName}";
        }
    }
}
