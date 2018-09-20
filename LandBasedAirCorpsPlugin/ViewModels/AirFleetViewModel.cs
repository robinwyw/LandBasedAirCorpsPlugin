using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.ViewModels;
using LandBasedAirCorpsPlugin.Models;
using Livet;
using Livet.EventListeners;

namespace LandBasedAirCorpsPlugin.ViewModels
{
    public class AirFleetViewModel : ItemViewModel
    {
        private readonly AirFleet source;
        private readonly StandbyViewModel standby;
        private readonly SortieViewModel sortie;
        private readonly DefenseViewModel defense;

        public MapArea MapArea => this.source.MapArea;

        public AirRegimentViewModel[] Regiments => this.source.Regiments.Select(x => new AirRegimentViewModel(x.Value)).ToArray();

        public bool IsReady => this.source.IsReady;

        public ViewModel QuickStateView
        {
            get
            {
                if (this.source.State.HasFlag(AirFleetSituation.Standby))
                {
                    return this.standby;
                }
                if (this.source.State.HasFlag(AirFleetSituation.Sortie))
                {
                    return this.sortie;
                }
                if (this.source.State.HasFlag(AirFleetSituation.Defense))
                {
                    return this.defense;
                }

                return NullViewModel.Instance;
            }
        }

        public AirFleetViewModel(AirFleet source)
        {
            this.source = source;

            this.standby = new StandbyViewModel(this.source);
            this.sortie = new SortieViewModel();
            this.defense = new DefenseViewModel();

            this.CompositeDisposable.Add(new PropertyChangedEventListener(this.source)
            {
                (sender, args) => this.RaisePropertyChanged(args.PropertyName),
                { nameof(AirFleet.State), (sender, args) => this.RaisePropertyChanged(nameof(this.QuickStateView)) }
            });
        }
    }
}
