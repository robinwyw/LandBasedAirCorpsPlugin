using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using LandBasedAirCorpsPlugin.Models.Raw;
using Livet;

namespace LandBasedAirCorpsPlugin.Models
{
    public class AirFleet : NotificationObject, IIdentifiable
    {
        public int Id { get; }

        #region MapArea変更通知プロパティ
        private MapArea _MapArea;

        public MapArea MapArea
        {
            get { return this._MapArea; }
            set
            { 
                if (this._MapArea == value)
                    return;
                this._MapArea = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Regiments変更通知プロパティ
        private MemberTable<AirRegiment> _Regiments;

        public MemberTable<AirRegiment> Regiments
        {
            get { return this._Regiments; }
            set
            { 
                if (this._Regiments == value)
                    return;
                this._Regiments = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region State変更通知プロパティ
        private AirFleetSituation _State;

        public AirFleetSituation State
        {
            get { return this._State; }
            set
            { 
                if (this._State == value)
                    return;
                this._State = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region IsInSortie変更通知プロパティ
        private bool _IsInSortie;

        public bool IsInSortie
        {
            get { return this._IsInSortie; }
            set
            {
                if (this._IsInSortie == value)
                    return;
                this._IsInSortie = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region IsReady変更通知プロパティ
        private bool _IsReady;

        public bool IsReady
        {
            get { return this._IsReady; }
            set
            { 
                if (this._IsReady == value)
                    return;
                this._IsReady = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public bool CanSortie => this.Regiments.Values.Any(x => x.Behavior == AirRegimentBehavior.Sortie || x.Behavior == AirRegimentBehavior.Defense);

        public AirFleet(IEnumerable<kcsapi_air_base> raw, int mapId)
        {
            var regiments = raw.Select(x => new AirRegiment(x));
            this.Regiments = new MemberTable<AirRegiment>(regiments);
            this.MapArea = KanColleClient.Current.Master.MapAreas[mapId];
            this.Id = mapId;

            this.UpdateFleetState();
        }

        internal void Sortie()
        {
            if (this.IsInSortie) return;

            this.IsInSortie = true;
            this.UpdateFleetState();
        }

        internal void Homing()
        {
            if (!this.IsInSortie) return;

            this.IsInSortie = false;
            this.UpdateFleetState();
        }

        internal void UpdateFleetState()
        {
            var state = AirFleetSituation.Empty;
            var isReady = false;
            var regiments = this.Regiments.Values;
            var squadrons = regiments.SelectMany(x => x.Squadrons);

            if (squadrons.Any(x => x.State == SquadronState.Deployed))
            {
                if (this.IsInSortie)
                {
                    if (regiments.Any(x => x.Behavior == AirRegimentBehavior.Sortie))
                    {
                        state |= AirFleetSituation.Sortie;
                        isReady = false;
                    }
                    if (regiments.Any(x => x.Behavior == AirRegimentBehavior.Defense))
                    {
                        state |= AirFleetSituation.Defense;
                        isReady = false;
                    }
                }
                else
                {
                    state |= AirFleetSituation.Standby;
                    isReady = true;
                }

                if (state.HasFlag(AirFleetSituation.Standby))
                {
                    if (squadrons.Any(x => x.WorkingCount != x.DeploymentCount))
                    {
                        state |= AirFleetSituation.InShortSupply;
                        isReady = false;
                    }
                    if (squadrons.Any(x => x.State == SquadronState.Relocating))
                    {
                        state |= AirFleetSituation.Relocating;
                        isReady = false;
                    }
                }
            }

            this.State = state;
            this.IsReady = isReady;
        }
    }
}
