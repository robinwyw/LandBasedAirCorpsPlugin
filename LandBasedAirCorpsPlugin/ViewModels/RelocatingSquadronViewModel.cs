using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LandBasedAirCorpsPlugin.Models;
using Livet;
using Livet.EventListeners;

namespace LandBasedAirCorpsPlugin.ViewModels
{
    public class RelocatingSquadronViewModel : ViewModel
    {
        private readonly RelocatingSquadron source;

        public Squadron Squadron => this.source.Squadron;

        public string RegimentName => this.source.RegimentName;

        public string MapAreaName => this.source.MapAreaName;

        public string Remaining => this.source.Remaining.HasValue
            ? this.source.Remaining.Value.ToString(@"mm\:ss")
            : "--:--";

        public string CompleteTime => this.source.CompleteTime.HasValue
            ? this.source.CompleteTime.Value.LocalDateTime.ToString("MM/dd HH:mm:ss")
            : "--/-- --:--:--";

        public RelocatingSquadronViewModel(RelocatingSquadron source)
        {
            this.source = source;
            this.CompositeDisposable.Add(new PropertyChangedEventListener(this.source)
            {
                (sender, args) => this.RaisePropertyChanged(args.PropertyName)
            });
        }
    }
}
