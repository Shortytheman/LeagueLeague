namespace Footballivision;

public class Football
{
    static List<League> Leagues { get; set; }
    List<Team> Teams { get; set; }
    
    static Dictionary<string, TeamData> teamData = new Dictionary<string, TeamData>();
    
    private String directory = "../../../files/";

    static void Main(string[] args)
    {
        Football football = new Football();
        football.startup();
        football.makeSetup();
        football.MakeRoundsForLeague("LCK");
        football.showstats("LCK");
        List<TeamData> teamdata = new List<TeamData>(teamData.Values);
        football.OutputTable(teamdata);
    }

    //Startmetode, opretter leagues og teams og smider dem i lister.
    void startup()
    {
        //Hardcoding the teams
        Teams = new List<Team>();

        Teams.Add(new Team("ninjas in pyjamas", "nip", "gold"));
        Teams.Add(new Team("team solo mid", "tsm", "platinum"));
        Teams.Add(new Team("evil geniuses", "eg", "silver"));
        Teams.Add(new Team("100 thieves", "100t", "gold"));
        Teams.Add(new Team("flyquest", "fq", "silver"));
        Teams.Add(new Team("team liquid", "tl", "platinum"));
        Teams.Add(new Team("cloud 9", "c9", "gold"));
        Teams.Add(new Team("counter logic gaming", "clg", "silver"));
        Teams.Add(new Team("dignitas", "dig", "gold"));
        Teams.Add(new Team("immortal", "imm", "silver"));
        Teams.Add(new Team("fanatic", "fnc", "gold"));
        Teams.Add(new Team("g2", "g2", "platinum"));

        //Making the league
        Leagues = new List<League>();

        League l = new League();
        l.name = "MSI";
        l.rounds = 22;
        l.teams = Teams;
        l.positionsToWorlds = 2;
        Leagues.Add(l);

        League l1 = new League();
        l1.name = "LCS";
        l1.rounds = 22;
        l1.teams = Teams;
        l1.positionsToWorlds = 3;
        Leagues.Add(l1);

        League l2 = new League();
        l2.name = "LCK";
        l2.rounds = 22;
        l2.teams = Teams;
        l2.positionsToWorlds = 1;
        Leagues.Add(l2);
    }

    //Tager data fra mine lister og smider dem i filer i CSV-format.
    void makeSetup()
    {
        using (StreamWriter setup = new StreamWriter(directory + "setup.csv"))
        {
            for (int i = 0; i < Leagues.Count; i++)
            {
                //Skriver til filen "setup.csv" i formatet (LeagueName,positionsToWorlds,\n)
                setup.WriteLine(String.Join(",", Leagues[i].name, Leagues[i].positionsToWorlds + ","));

            }

            setup.Flush();
        }

        using (StreamWriter teams = new StreamWriter(directory + "teams.csv"))
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                //Skriver til filen "Teams.csv" i formatet (Abbriviation, TeamName, Tier,\n)
                teams.WriteLine(String.Join(",", Teams[i].Abbreviation, Teams[i].Name, Teams[i].Tier + ","));
            }

            teams.Flush();
        }
    }

    void MakeRoundsForLeague(string league)
    {
        Random rnd = new Random();

        //Initial rounds, 1-22 including the last one.
        for (int i = 0; i < 22; i++)
        {
            var tempTeams = new List<Team>(Teams);

            using (StreamWriter w = new StreamWriter(directory + league + "/round-" + (i + 1) + ".csv"))
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

                    w.WriteLine(tempTeams[randomTeam1].Abbreviation + ";" + tempTeams[randomTeam2].Abbreviation + ";" +
                                killsTeam1 + ";" + killsTeam2);

                    if (randomTeam1 < randomTeam2)
                    {
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


        Dictionary<string, int> score = new Dictionary<string, int>();
        StreamReader reader = null;

        String[] files = Directory.GetFiles(directory + "/" + league);

        try
        {

            for (int i = 0; i < files.Length; i++)
            {
                reader = new StreamReader(directory + league + "/round-" + (i + 1) + ".csv");
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var words = line.Split(";");
                    if (!score.ContainsKey(words[0]))
                    {
                        score.Add(words[0], 0);
                    }

                    if (!score.ContainsKey(words[1]))
                    {
                        score.Add(words[1], 0);
                    }

                    if (Convert.ToInt32(words[2]) > Convert.ToInt32(words[3]))
                    {
                        score[words[0]] = score[words[0]] + 1;
                    }
                    else
                    {
                        score[words[1]] = score[words[1]] + 1;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("That round doesn't exist. Error: " + e);
        }


        //Print team, number of wins to console.
        for (int i = 0; i < score.Count; i++)
        {
            Console.WriteLine("Key: {0}, Value: {1}",
                score.ElementAt(i).Key,
                score.ElementAt(i).Value);
        }

        List<String> topBracket = new List<string>();
        List<String> lowerBracket = new List<string>();

        for (int i = 0; i < 6; i++)
        {
            //kvp = Key Value Pair
            var highestWins = score.MaxBy(kvp => kvp.Value);
            var highestWinsTeam = highestWins.Key;
            var wins = highestWins.Value;
            topBracket.Add(highestWinsTeam);
            score.Remove(highestWinsTeam);
        }

        foreach (var teams in score)
        {
            lowerBracket.Add(teams.Key);
        }

        //Additional Rounds, 23-33 including the last one.
        for (int i = 23; i < 34; i++)
        {
            var tempTopBracket = new List<String>(topBracket);

            
            using (StreamWriter top = new StreamWriter(directory + league + "/topBracket/round-" + i + ".csv"))
            {
                for (int j = 0; j < 3; j++)
                {
                    
                    int randomTeam1 = rnd.Next(tempTopBracket.Count);
                    int randomTeam2 = rnd.Next(tempTopBracket.Count);
                    while (randomTeam1 == randomTeam2)
                    {
                        randomTeam2 = rnd.Next(tempTopBracket.Count);
                    }

                    int killsTeam1 = rnd.Next(25);
                    int killsTeam2 = rnd.Next(25);
                    while (killsTeam1 == killsTeam2)
                    {
                        killsTeam2 = rnd.Next(25);
                    }

                    top.WriteLine(tempTopBracket[randomTeam1] + ";" + tempTopBracket[randomTeam2] + ";" +
                                  killsTeam1 + ";" + killsTeam2);
                    

                    if (randomTeam1 < randomTeam2)
                    {
                        tempTopBracket.RemoveAt(randomTeam1);
                        tempTopBracket.RemoveAt(randomTeam2 - 1);
                    }
                    else
                    {
                        tempTopBracket.RemoveAt(randomTeam1);
                        tempTopBracket.RemoveAt(randomTeam2);
                    }
                }
            }
        }

        for (int i = 23; i < 34; i++)
        {
            var tempLowerBracket = new List<String>(lowerBracket);
            using (StreamWriter lower =
                   new StreamWriter(directory + league + "/lowerBracket/round-" + i + ".csv"))
            {
                for (int j = 0; j < 3; j++)
                {
                    int randomTeam1 = rnd.Next(tempLowerBracket.Count);
                    int randomTeam2 = rnd.Next(tempLowerBracket.Count);
                    while (randomTeam1 == randomTeam2)
                    {
                        randomTeam2 = rnd.Next(tempLowerBracket.Count);
                    }

                    int killsTeam1 = rnd.Next(25);
                    int killsTeam2 = rnd.Next(25);
                    while (killsTeam1 == killsTeam2)
                    {
                        killsTeam2 = rnd.Next(25);
                    }

                    lower.WriteLine(tempLowerBracket[randomTeam1] + ";" + tempLowerBracket[randomTeam2] + ";" +
                                    killsTeam1 + ";" + killsTeam2);
                    

                    if (randomTeam1 < randomTeam2)
                    {
                        tempLowerBracket.RemoveAt(randomTeam1);
                        tempLowerBracket.RemoveAt(randomTeam2 - 1);
                    }
                    else
                    {
                        tempLowerBracket.RemoveAt(randomTeam1);
                        tempLowerBracket.RemoveAt(randomTeam2);
                    }
                }
            }
        }
    }

    void showstats(String league)
    {
        // Get a list of all CSV files in the directory
        string[] fileNames = Directory.GetFiles(directory + league, "*.csv");

        // Initialize a dictionary to store team data
        teamData = new Dictionary<string, TeamData>();

        // Loop through each CSV file and parse its contents
        foreach (string fileName in fileNames)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // Split the line into fields
                    string[] fields = line.Split(';');

                    // Extract team names and scores
                    string team1 = fields[0];
                    string team2 = fields[1];
                    int team1Score = int.Parse(fields[2]);
                    int team2Score = int.Parse(fields[3]);

                    // Update team data for team 1
                    if (!teamData.ContainsKey(team1))
                    {
                        teamData[team1] = new TeamData(team1);
                    }

                    teamData[team1].GamesPlayed++;
                    teamData[team1].Kills += team1Score;
                    teamData[team1].Deaths += team2Score;
                    if (team1Score > team2Score)
                    {
                        teamData[team1].Wins++;
                        teamData[team1].CurrentStreak = "W" + teamData[team1].CurrentStreak;
                    }
                    else if (team1Score < team2Score)
                    {
                        teamData[team1].Losses++;
                        teamData[team1].CurrentStreak = "L" + teamData[team1].CurrentStreak;
                    }
                    else
                    {
                        teamData[team1].Draws++;
                        teamData[team1].CurrentStreak = "D" + teamData[team1].CurrentStreak;
                    }

                    teamData[team1].CurrentStreak = teamData[team1].CurrentStreak
                        .Substring(0, Math.Min(5, teamData[team1].CurrentStreak.Length));

                    // Update team data for team 2
                    if (!teamData.ContainsKey(team2))
                    {
                        teamData[team2] = new TeamData(team2);
                    }

                    teamData[team2].GamesPlayed++;
                    teamData[team2].Kills += team2Score;
                    teamData[team2].Deaths += team1Score;
                    if (team2Score > team1Score)
                    {
                        teamData[team2].Wins++;
                        teamData[team2].CurrentStreak = "W" + teamData[team2].CurrentStreak;
                    }
                    else if (team2Score < team1Score)
                    {
                        teamData[team2].Losses++;
                        teamData[team2].CurrentStreak = "L" + teamData[team2].CurrentStreak;
                    }
                    else
                    {
                        teamData[team2].Draws++;
                        teamData[team2].CurrentStreak = "D" + teamData[team2].CurrentStreak;
                    }

                    teamData[team2].CurrentStreak = teamData[team2].CurrentStreak
                        .Substring(0, Math.Min(5, teamData[team2].CurrentStreak.Length));
                }
            }
        }
    }
    
    

    public void OutputTable(List<TeamData> teamDataList)
    {
        Console.WriteLine("Pos {0,-20}   {1,-6} {2,-6}  {3,-6}  {4,-6}  {5,-6}  {6,-6}  {7,-6} {8,-6}", "Team", "W", "D", "L", "K", "D", "Kd", "Pts", "Streak");
        Console.WriteLine("==========================================================================");

        int pos = 1;
        foreach (TeamData teamData in teamDataList.OrderByDescending(team => team.Points).ThenByDescending(team => team.Kd))
        {
            string streak = teamData.CurrentStreak.Length > 5 ? teamData.CurrentStreak.Substring(teamData.CurrentStreak.Length - 5) : teamData.CurrentStreak;
            ConsoleColor streakColor = streak.Count(c => c == 'W') >= streak.Count(c => c == 'L') ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write("{0,-4} {1,-20} {2,-7} {3,-7} {4,-7} {5,-7} {6,-7} {7,-7} {8,-7}",
                pos, teamData.Name, teamData.Wins, teamData.Draws, teamData.Losses, teamData.Kills, teamData.Deaths, teamData.Kd, teamData.Points);
            Console.ForegroundColor = streakColor;
            Console.WriteLine(streak);
            Console.ResetColor();
            pos++;
        }
    }









//Klasser - Indtil videre kun 3. League, Team og LeagueTeam
    class League
    {
        public string name { get; set; }
        public int rounds { get; set; }
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

        public Team(string name, string abbreviation, string tier)
        {
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.Tier = tier;
        }
    }

    public class TeamData
    {
        public string Name { get; set; }
        public int GamesPlayed { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Kd => Kills - Deaths;
        public string CurrentStreak { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        
        public int Points
        {
            set { }
            get { return Wins * 3 + Draws; }
        }

        public void AddResult(int score1, int score2)
        {
            GamesPlayed++;
            Kills += score1;
            Deaths += score2;
            if (score1 > score2)
            {
                Points += 3;
                Wins++;
                CurrentStreak += "W";
            }
            else if (score1 == score2)
            {
                Points += 1;
                Draws++;
                CurrentStreak += "D";
            }
            else
            {
                Losses++;
                CurrentStreak += "L";
            }

            // Truncate the current streak to 5 characters
            if (CurrentStreak.Length > 5)
                CurrentStreak = CurrentStreak.Substring(1, 5);
        }
    

        
        public override string ToString()
        {
            return $"{Name} ({Points})";
        }

        public TeamData(string name)
        {
            Name = name;
            GamesPlayed = 0;
            Kills = 0;
            Deaths = 0;
            CurrentStreak = "-";
        }
    }
}

