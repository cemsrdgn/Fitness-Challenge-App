public class Review
{
    public int Id { get; set; }
    public int ChallengeId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public string? UserId { get; set; } // Nullable yapıldı
}