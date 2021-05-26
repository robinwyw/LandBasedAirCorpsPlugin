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
    public class AirRegiment : RawDataWrapper<kcsapi_air_base>, IIdentifiable
    {
        public int Id => this.RawData.api_rid;

        public MapArea MapArea { get; }

        #region Name変更通知プロパティ
        private string _Name;

        public string Name
        {
            get { return this._Name; }
            set
            { 
                if (this._Name == value)
                    return;
                this._Name = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Distance変更通知プロパティ
        private int _Distance;

        public int Distance
        {
            get { return this._Distance; }
            set
            { 
                if (this._Distance == value)
                    return;
                this._Distance = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.Behavior));
            }
        }
        #endregion

        #region Behavior変更通知プロパティ
        private AirRegimentBehavior _Behavior;

        public AirRegimentBehavior Behavior
        {
            get { return this._Behavior; }
            set
            { 
                if (this._Behavior == value)
                    return;
                this._Behavior = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Squadrons変更通知プロパティ
        private Squadron[] _Squadrons;

        public Squadron[] Squadrons
        {
            get { return this._Squadrons; }
            set
            { 
                if (this._Squadrons == value)
                    return;
                this._Squadrons = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public double AirSuperiority
        {
            get
            {
                var bonus = this.Squadrons
                    .Where(x => x.State == SquadronState.Deployed)
                    .MaxByViewRange()
                    .GetSurveillanceBonus(this.Behavior);
                
                return Math.Floor(this.Squadrons.Select(x => x.GetAirSuperiority(this.Behavior)).Sum() * bonus);
            }
        }

        public AirRegiment(kcsapi_air_base raw) : base(raw)
        {
            this.Name = raw.api_name;
            this.Distance = raw.api_distance.api_base + raw.api_distance.api_bonus;
            this.Behavior = (AirRegimentBehavior)raw.api_action_kind;
            this.Squadrons = raw.api_plane_info.Select(x => new Squadron(x)).ToArray();
            this.MapArea = KanColleClient.Current.Master.MapAreas[raw.api_area_id];
        }

        internal RelocatingSquadron SetOrReplaceSquadron(kcsapi_plane_info plane)
        {
            var i = plane.api_squadron_id - 1;
            var current = this.Squadrons[i];

            this.Squadrons[i] = new Squadron(plane);
            this.RaiseSquadronsUpdated();

            return current.State == SquadronState.Deployed ? new RelocatingSquadron(current, null) : null;
        }
        
        internal void ExchangeSquadrons(kcsapi_plane_info[] planes)
        {
            foreach (var plane in planes)
            {
                this.Squadrons[plane.api_squadron_id - 1] = new Squadron(plane);
            }

            this.RaiseSquadronsUpdated();
        }

        internal RelocatingSquadron UnsetSquadron(int id, SquadronState state)
        {
            this.Squadrons[id - 1].State = state;
            this.RaiseSquadronsUpdated();

            return new RelocatingSquadron(this.Squadrons[id - 1], this);
        }

        internal void Supply(string[] squadronId)
        {
            foreach (var id in squadronId)
            {
                var squadron = this.Squadrons[int.Parse(id) - 1];
                squadron.WorkingCount = squadron.DeploymentCount;
            }

            this.RaiseSquadronsUpdated();
        }

        private void RaiseSquadronsUpdated()
        {
            this.RaisePropertyChanged(nameof(this.Squadrons));
            this.RaisePropertyChanged(nameof(this.AirSuperiority));
        }

        public override string ToString()
        {
            return $"Name = {this.Name}, Distance = {this.Distance}, ActionType = {this.Behavior}{this.Squadrons.Select(x => "\n" + x.ToString())}";
        }
    }
}
