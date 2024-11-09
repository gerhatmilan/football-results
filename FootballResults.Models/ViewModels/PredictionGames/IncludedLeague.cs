using FootballResults.DataAccess.Entities.Football;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballResults.Models.ViewModels.PredictionGames
{
    public class IncludedLeague
    {
        public League League { get; set; }
        public bool Included { get; set; }
    }
}
