using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace TheBox
{
    /// <summary>
    /// Summary description for FootballService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class FootballService : System.Web.Services.WebService
    {

        public class FootballTeam
        {
            public string Name { get; set; }
            public string City { get; set; }
            public short Created { get; set; }
        }

        List<FootballTeam> FootballTeams = new List<FootballTeam>
{
new FootballTeam{Name = "Liverpool", City = "Liverpool", Created = 1892},
new FootballTeam{Name = "Everton", City = "Liverpool", Created = 1878},
new FootballTeam{Name = "Man Utd", City = "Manchester", Created = 1878},
new FootballTeam{Name = "Arsenal", City = "London", Created = 1886},
new FootballTeam{Name = "Tottenham", City = "London", Created = 1882}
};

        [WebMethod]
        public List<FootballTeam> GetTeams()
        {
            return FootballTeams;
        }
    }
}
