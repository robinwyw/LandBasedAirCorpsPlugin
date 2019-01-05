using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;
using LandBasedAirCorpsPlugin.Models;
using LandBasedAirCorpsPlugin.ViewModels;
using LandBasedAirCorpsPlugin.Views;
using MetroRadiance.UI;
using MetroTrilithon.Lifetime;

namespace LandBasedAirCorpsPlugin
{
    [Export(typeof(IPlugin))]
    [Export(typeof(ITool))]
    [Export(typeof(ISettings))]
    [Export(typeof(IRequestNotify))]
    [ExportMetadata("Title", "LandBasedAirCorpsPlugin")]
    [ExportMetadata("Description", "基地航空隊の情報を表示します。")]
    [ExportMetadata("Version", "1.1")]
    [ExportMetadata("Author", "@ame225")]
    [ExportMetadata("Guid", "E7B62940-0702-4369-898F-BC177042514D")]
    public class LandBasedAirCorpsPlugin : IPlugin, ITool, ISettings, IRequestNotify
    {
        private readonly SerialDisposable toolviewDisposable = new SerialDisposable();

        private LandBaseViewModel vm;

        public event EventHandler<NotifyEventArgs> NotifyRequested;

        public void Initialize()
        {
            var notifier = new Notifier(this);
            this.vm = new LandBaseViewModel(new LandBase(notifier));
        }

        internal void InvokeNotifyRequested(NotifyEventArgs e) => this.NotifyRequested?.Invoke(this, e);

        string ITool.Name => "LandBase";

        object ITool.View => new ToolView() { DataContext = this.vm };

        object ISettings.View => new SettingsView() { DataContext = this.vm };
    }
}
