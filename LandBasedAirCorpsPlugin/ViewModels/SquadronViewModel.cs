using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LandBasedAirCorpsPlugin.Models;

namespace LandBasedAirCorpsPlugin.ViewModels
{
    public class SquadronViewModel
    {
        public Squadron Squadron { get; }

        public SquadronViewModel(Squadron squadron)
        {
            this.Squadron = squadron;
        }
    }
}
