namespace MeinPortfolio.Models;

public class Project
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> TechStack { get; set; } = new();
    public string? CodeUrl { get; set; }
    public string? LiveUrl { get; set; }
}