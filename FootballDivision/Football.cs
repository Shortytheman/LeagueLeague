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

    //Bruges ikke lige nu.
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
        Random rnd = new Random();

        //Initial rounds, 1-22 including the last one.
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

        try
        {

            for (int i = 0; i < 22; i++)
            {
                reader = new StreamReader("../../../files/" + league + "/round-" + (i + 1) + ".csv");
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

        Console.WriteLine("Top bracket: " + topBracket.Count);
        Console.WriteLine("Lower bracket: " + lowerBracket.Count);

        //Additional Rounds, 23-33 including the last one.
        for (int i = 23; i < 34; i++)
        {
            var tempTopBracket = new List<String>(topBracket);
            
            Console.WriteLine("Start of loop");
            using (StreamWriter top = new StreamWriter("../../../files/" + league + "/topBracket/round-" + i + ".csv"))
            {
                for (int j = 0; j < 2; j++)
                {
                    Console.WriteLine("Start of top writer");
                    int randomTeam1 = rnd.Next(tempTopBracket.Count);
                    int randomTeam2 = rnd.Next(tempTopBracket.Count);
                    Console.WriteLine("Randomteam1: " + randomTeam1 + " Randomteam2: " + randomTeam2);
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

                    Console.WriteLine("After top writer");

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
                   new StreamWriter("../../../files/" + league + "/lowerBracket/round-" + i + ".csv"))
            {
                Console.WriteLine("Start of lower writer");
                for (int j = 0; j < 2; j++)
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

                    Console.WriteLine("After lower writer");

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

