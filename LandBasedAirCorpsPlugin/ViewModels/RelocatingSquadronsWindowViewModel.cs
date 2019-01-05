using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTrilithon.Mvvm;
using Livet;
using Livet.EventListeners;
using LandBasedAirCorpsPlugin.Models;

namespace LandBasedAirCorpsPlugin.ViewModels
{
    public class RelocatingSquadronsWindowViewModel : WindowViewModel
    {
        #region RelocatingSquadrons 変更通知プロパティ

        private ReadOnlyDispatcherCollection<RelocatingSquadronViewModel> _RelocatingSquadrons;

        public ReadOnlyDispatcherCollection<RelocatingSquadronViewModel> RelocatingSquadrons
        {
            get
            { return this._RelocatingSquadrons; }
            set
            { 
                if (this._RelocatingSquadrons == value)
                    return;
                this._RelocatingSquadrons = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        public RelocatingSquadronsWindowViewModel(LandBase landBase)
        {
            this.Title = "配置転換中の航空機一覧";

            this.RelocatingSquadrons = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                landBase.RelocatingSquadrons,
                x => new RelocatingSquadronViewModel(x),
                DispatcherHelper.UIDispatcher);
        }
    }
}
