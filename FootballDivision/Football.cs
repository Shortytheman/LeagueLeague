namespace Footballivision;

public class Football
{
    static List<League> Leagues  { get; set; }
    List<Team> Teams  { get; set; }

    static void Main(string[] args)
    {
        Football football = new Football();
        football.startup();
        football.makeSetup();
        football.MakeRoundsForLeague("LCK");
    }

    //Startmetode, opretter leagues og teams og smider dem i lister.
    void startup()
    {   //Hardcoding the teams
        Teams = new List<Team>();
        
        Teams.Add(new Team("ninjas in pyjamas","nip","gold"));
        Teams.Add(new Team("team solo mid","tsm","platinum"));
        Teams.Add(new Team("evil geniuses","eg","silver"));
        Teams.Add(new Team("100 thieves","100t","gold"));
        Teams.Add(new Team("flyquest","fq","silver"));
        Teams.Add(new Team("team liquid","tl","platinum"));
        Teams.Add(new Team("cloud 9","c9","gold"));
        Teams.Add(new Team("counter logic gaming","clg","silver"));
        Teams.Add(new Team("dignitas","dig","gold"));
        Teams.Add(new Team("immortal","imm","silver"));
        Teams.Add(new Team("fanatic","fnc","gold"));
        Teams.Add(new Team("g2","g2","platinum"));
        
        //Making the league
        Leagues = new List<League>();
        
        League l  = new League();
        l.name = "MSI";
        l.rounds = 22;
        l.teams = Teams;
        l.positionsToWorlds = 2;
        Leagues.Add(l);
        
        League l1  = new League();
        l1.name = "LCS";
        l1.rounds = 22;
        l1.teams = Teams;
        l1.positionsToWorlds = 3;
        Leagues.Add(l1);
        
        League l2  = new League();
        l2.name = "LCK";
        l2.rounds = 22;
        l2.teams = Teams;
        l2.positionsToWorlds = 1;
        Leagues.Add(l2);
    }

    //Tager data fra mine lister og smider dem i filer i CSV-format.
    void makeSetup()
    {
        using (StreamWriter setup = new StreamWriter("../../../files/setup.csv"))
        {
            for (int i = 0; i < Leagues.Count; i++)
            {
                //Skriver til filen "setup.csv" i formatet (LeagueName,positionsToWorlds,\n)
                setup.WriteLine(String.Join(",", Leagues[i].name,Leagues[i].positionsToWorlds + ","));
                
            }
            setup.Flush();
        }
        using (StreamWriter teams = new StreamWriter("../../../files/teams.csv"))
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                //Skriver til filen "Teams.csv" i formatet (Abbriviation, TeamName, Tier,\n)
                teams.WriteLine(String.Join(",", Teams[i].Abbreviation,Teams[i].Name,Teams[i].Tier + ","));
            }
            teams.Flush();
        }
    }

    void makeStats()
    {
        Random rnd = new Random();
        for (int i = 0; i < Teams.Count; i++)
        {
            var positionOnTable = i + 1;
            var lost = Teams[i].LeagueTeam.gamesLost = rnd.Next(5);
            var won = Teams[i].LeagueTeam.gamesWon = rnd.Next(5);
            var played = Teams[i].LeagueTeam.gamesPlayed = lost + won;
            var kills = Teams[i].LeagueTeam.kills = rnd.Next(25);
            var deaths = Teams[i].LeagueTeam.deaths = rnd.Next(25);
            var kd = Teams[i].LeagueTeam.kd = Math.Abs(deaths - kills); //Så den kun giver positivt tal.
            //3 point per game won.
            var pointsAchieved = Teams[i].LeagueTeam.pointsAchieved = (won * 3);
            var winningStreak = Teams[i].LeagueTeam.winningStreak = rnd.Next(won);
        }
    }

    
    void MakeRoundsForLeague(string league)
    {
        //try
        //{
            Random rnd = new Random();
            
            //Initial rounds, 0-21 så altså runde 1 til og med 22
            for (int i = 0; i < 22; i++)
            {
                var tempTeams = new List<Team>(Teams);

                using (StreamWriter w = new StreamWriter("../../../files/" + league + "/round-" + (i + 1) + ".csv"))
                {
                    for (int j = 0; j < 6; j++)
                    {
                        int randomTeam1 = rnd.Next(tempTeams.Count);
                        int randomTeam2 = rnd.Next(tempTeams.Count);
                        while (randomTeam1 == randomTeam2)
                        {
                            randomTeam2 = rnd.Next(tempTeams.Count);
                        }

                        int killsTeam1 = rnd.Next(25);
                        int killsTeam2 = rnd.Next(25);
                        while (killsTeam1 == killsTeam2)
                        {
                            killsTeam2 = rnd.Next(25);
                        }
                        w.WriteLine(tempTeams[randomTeam1].Abbreviation + "," + tempTeams[randomTeam2].Abbreviation + "," + killsTeam1 + "," + killsTeam2);
                        
                        if (randomTeam1 < randomTeam2){
                        tempTeams.RemoveAt(randomTeam1);
                        tempTeams.RemoveAt(randomTeam2 - 1);
                        }
                        else
                        {
                            tempTeams.RemoveAt(randomTeam1);
                            tempTeams.RemoveAt(randomTeam2);
                        }
                    }

                }
            }
        

        //Additional Rounds, altså Runde 23 til og med 33
        /*for (int i = 23; i < 34; i++)
        {
            using (StreamWriter w = new StreamWriter("../../../files/" + league + "/round-" + i + ".csv"))
            {
                w.WriteLine();
            }
        }
        */
        
        //}catch
        //{
    //    Console.WriteLine("There is no league with that name... try again with a valid league name!");
    //}

    }
}


//Klasser - Indtil videre kun 3. League, Team og LeagueTeam
class League
{
    public string name { get; set; }
    public int rounds  { get; set; }
    public List<Team> teams { get; set; }
    public List<Team> lowerTierBracket { get; set; }
    public List<Team> upperTierBracket { get; set; }
    public int positionsToWorlds { get; set; }
}

class Team
{
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public string Tier { get; set; }
    public LeagueTeam LeagueTeam { get; set; }

    public Team(string name, string abbreviation, string tier)
    {
        this.Name = name;
        this.Abbreviation = abbreviation;
        this.Tier = tier;
    }
}

class LeagueTeam
{
    public int positionOnTable { get; set; }
    public int gamesPlayed { get; set; }
    public int gamesWon { get; set; }
    public int gamesLost { get; set; }
    public int kills { get; set; }
    public int deaths { get; set; }
    //KillsDeaths
    public int kd { get; set; }
    public int pointsAchieved { get; set; }
    public int winningStreak { get; set; }
}

