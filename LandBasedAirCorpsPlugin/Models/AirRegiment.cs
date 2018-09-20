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

        public MapArea MapArea => KanColleClient.Current.Master.MapAreas[this.RawData.api_area_id];

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
                if (this.Behavior == AirRegimentBehavior.Defense)
                {
                    var bonus = this.Squadrons
                        .Where(x => x.State == SquadronState.Deployed)
                        .MaxByViewRange()
                        .GetSurveillanceBonus();

                    return Math.Floor(this.Squadrons.Select(x => x.AirSuperiorityAtDefense).Sum() * bonus);
                }

                return this.Squadrons.Select(x => x.AirSuperiorityAtSortie).Sum();
            }
        }

        public AirRegiment(kcsapi_air_base raw) : base(raw)
        {
            this.Name = raw.api_name;
            this.Distance = raw.api_distance;
            this.Behavior = (AirRegimentBehavior)raw.api_action_kind;
            this.Squadrons = raw.api_plane_info.Select(x => new Squadron(x)).ToArray();
        }

        internal void SetPlanes(kcsapi_plane_info[] planes)
        {
            foreach (var item in planes)
            {
                this.Squadrons[item.api_squadron_id - 1] = new Squadron(item);
            }

            this.RaiseSquadronsUpdated();
        }

        internal void UnsetPlane(int id, SquadronState state)
        {
            this.Squadrons[id - 1].State = state;
            this.RaiseSquadronsUpdated();
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
