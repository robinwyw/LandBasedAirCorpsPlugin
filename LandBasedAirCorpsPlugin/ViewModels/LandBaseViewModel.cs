using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using LandBasedAirCorpsPlugin.Models;
using MetroTrilithon.Mvvm;
using MetroTrilithon.Lifetime;
using Livet;
using Livet.Messaging;

namespace LandBasedAirCorpsPlugin.ViewModels
{
    public class LandBaseViewModel : ViewModel
    {
        private readonly LandBase source;
        private int selectedMapId;

        #region AirFleets変更通知プロパティ
        private AirFleetViewModel[] _AirFleets;

        public AirFleetViewModel[] AirFleets
        {
            get { return this._AirFleets; }
            set
            { 
                if (this._AirFleets == value)
                    return;
                this._AirFleets = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region SelectedFleet変更通知プロパティ
        private AirFleetViewModel _SelectedFleet;

        public AirFleetViewModel SelectedFleet
        {
            get { return this._SelectedFleet; }
            set
            { 
                if (this._SelectedFleet == value)
                    return;
                this._SelectedFleet = value;
                this.selectedMapId = value.MapArea.Id;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public LandBaseViewModel(LandBase source)
        {
            this.source = source;
            this.source.Subscribe(nameof(LandBase.AirFleets), () =>
            {
                var fleets = this.source.AirFleets.Select(kvp => new AirFleetViewModel(kvp.Value)).ToArray();
                if (this.SelectedFleet == null)
                    this.SelectedFleet = fleets.FirstOrDefault();
                else
                    this.SelectedFleet = fleets.FirstOrDefault(x => x.MapArea.Id == this.selectedMapId);

                this.AirFleets = fleets;
            }, false).AddTo(this);
        }

        public void ShowRelocatingSquadronsWindow()
        {
            var rsvm = new RelocatingSquadronsWindowViewModel(this.source);
            var message = new TransitionMessage(rsvm, TransitionMode.NewOrActive, "RelocatinSquadronsWindow.Show");
            this.Messenger.Raise(message);
        }
    }
}
