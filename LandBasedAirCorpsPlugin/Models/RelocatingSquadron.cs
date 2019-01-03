using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Livet;

namespace LandBasedAirCorpsPlugin.Models
{
    public class RelocatingSquadron : TimerNotifier
    {
        private const int duration = 12;

        #region Squadron 変更通知プロパティ

        private Squadron _Squadron;

        public Squadron Squadron
        {
            get
            { return this._Squadron; }
            set
            { 
                if (this._Squadron == value)
                    return;
                this._Squadron = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region RegimentName 変更通知プロパティ

        private string _RegimentName;

        public string RegimentName
        {
            get
            { return this._RegimentName; }
            set
            { 
                if (this._RegimentName == value)
                    return;
                this._RegimentName = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region MapAreaName 変更通知プロパティ

        private string _MapAreaName;

        public string MapAreaName
        {
            get
            { return this._MapAreaName; }
            set
            { 
                if (this._MapAreaName == value)
                    return;
                this._MapAreaName = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Remaining変更通知プロパティ
        private TimeSpan? _Remaining;

        public TimeSpan? Remaining
        {
            get { return this._Remaining; }
            set
            { 
                if (this._Remaining == value)
                    return;
                this._Remaining = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region CompleteTime変更通知プロパティ
        private DateTimeOffset? _CompleteTime;

        public DateTimeOffset? CompleteTime
        {
            get { return this._CompleteTime; }
            set
            { 
                if (this._CompleteTime == value)
                    return;
                this._CompleteTime = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public event EventHandler<RelocatingCompletedEventArgs> Completed;

        public RelocatingSquadron(Squadron squadron, AirRegiment regiment)
        {
            this.Squadron = squadron;
            this.Remaining = TimeSpan.FromMinutes(duration);
            this.CompleteTime = DateTime.Now + this.Remaining.Value;

            if (regiment != null)
            {
                this.RegimentName = regiment.Name;
                this.MapAreaName = regiment.MapArea.Id <= 7 ? regiment.MapArea.Name : "期間限定海域";
            }
        }

        protected override void Tick()
        {
            base.Tick();

            if (this.CompleteTime.HasValue)
            {
                var remaining = this.CompleteTime.Value - DateTime.Now;
                if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

                this.Remaining = remaining;

                if (this.Completed != null && remaining.Ticks == 0)
                {
                    this.Completed(this, new RelocatingCompletedEventArgs(this.Squadron, this.RegimentName, this.MapAreaName));
                    this.Dispose();
                }
            }
            else
            {
                this.Remaining = null;
            }
        }
    }
}
