using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using LandBasedAirCorpsPlugin.Models;
using Livet;
using Livet.EventListeners;

namespace LandBasedAirCorpsPlugin.ViewModels
{
    public class AirRegimentViewModel : ViewModel
    {
        private readonly AirRegiment source;

        public int Id => this.source.Id;

        public string Name => this.source.Name;

        public MapArea MapArea => this.source.MapArea;

        public int Distance => this.source.Distance;

        public AirRegimentBehavior Behavior => this.source.Behavior;

        public string BehaviorText => this.Behavior.GetText();

        public SquadronViewModel[] Squadrons => this.source.Squadrons.Select(x => new SquadronViewModel(x)).ToArray();

        public double AirSuperiority => this.source.AirSuperiority;

        public AirRegimentViewModel(AirRegiment regiment)
        {
            this.source = regiment;
            this.CompositeDisposable.Add(new PropertyChangedEventListener(regiment)
            {
                (sender, args) => this.RaisePropertyChanged(args.PropertyName),
                { nameof(AirRegiment.Behavior), (sender, args) => this.RaisePropertyChanged(nameof(this.BehaviorText)) }
            });
        }
    }
}
