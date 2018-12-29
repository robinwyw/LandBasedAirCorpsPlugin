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
    [ExportMetadata("Title", "LandBasedAirCorpsPlugin")]
    [ExportMetadata("Description", "基地航空隊の情報を表示します。")]
    [ExportMetadata("Version", "1.0.1")]
    [ExportMetadata("Author", "@ame225")]
    [ExportMetadata("Guid", "E7B62940-0702-4369-898F-BC177042514D")]
    public class LandBasedAirCorpsPlugin : IPlugin, ITool
    {
        private readonly SerialDisposable toolviewDisposer = new SerialDisposable();
        private LandBaseViewModel vm;

        public void Initialize()
        {
            this.vm = new LandBaseViewModel(new LandBase());
        }

        private T RegisterResources<T>(T view, SerialDisposable disposable) where T : FrameworkElement
        {
            var unregister = ThemeService.Current.Register(view.Resources);
            disposable.Disposable = unregister;

            return view;
        }

        string ITool.Name => "LandBase";

        object ITool.View => this.RegisterResources(new ToolView() { DataContext = this.vm }, this.toolviewDisposer);
    }
}
