using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleViewer.Composition;
using LandBasedAirCorpsPlugin.Models;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;
using Livet;

namespace LandBasedAirCorpsPlugin
{
    public class Notifier
    {
        internal static class Types
        {
            private static string name = nameof(LandBasedAirCorpsPlugin) + ".";

            public static string RelocatingComplete { get; } = name + nameof(RelocatingComplete);
        }

        private readonly LandBasedAirCorpsPlugin plugin;

        internal Notifier(LandBasedAirCorpsPlugin plugin)
        {
            this.plugin = plugin;
        }

        public void Notify(string type, string header, string body) => this.Notify(type, header, body, null, null);


        public void Notify(string type, string header, string body, Action activated, Action<Exception> failed)
        {
            var e = new NotifyEventArgs(type, header, body)
            {
                Activated = activated,
                Failed = failed
            };

            this.plugin.InvokeNotifyRequested(e);
        }
    }
}
