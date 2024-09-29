namespace EAD.Models;


public class AddRankingModel
{
    public string CustomerId { get; set; }
    public int Score { get; set; } // Ranking between 1-5
}