using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using LandBasedAirCorpsPlugin.Models;

namespace LandBasedAirCorpsPlugin.ViewModels
{
    public class SortieViewModel : ViewModel
    {
        public SortieViewModel()
        {
        }
    }

    public class DefenseViewModel : ViewModel
    {
        public DefenseViewModel()
        {
        }
    }

    public class StandbyViewModel : ViewModel
    {
        private readonly AirFleet source;

        public bool IsReady => this.source.IsReady;

        public bool IsRelocating => this.source.State.HasFlag(AirFleetSituation.Relocating);

        public StandbyViewModel(AirFleet source)
        {
            this.source = source;
            this.CompositeDisposable.Add(new Livet.EventListeners.PropertyChangedEventListener(this.source)
            {
                { nameof(AirFleet.State), (_, __) =>
                    {
                        this.RaisePropertyChanged(nameof(this.IsReady));
                        this.RaisePropertyChanged(nameof(this.IsRelocating));
                    }
                }
            });
        }
    }
}
