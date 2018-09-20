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

        public StandbyViewModel(AirFleet source)
        {
            this.source = source;
            this.CompositeDisposable.Add(new Livet.EventListeners.PropertyChangedEventListener(this.source)
            {
                { nameof(AirFleet.IsReady), (_, __) => this.RaisePropertyChanged(nameof(this.IsReady)) }
            });
        }
    }
}
