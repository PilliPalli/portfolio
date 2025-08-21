namespace MeinPortfolio.Services;

public class LocalizedFile
{
    public string German { get; set; }
    public string English { get; set; }

    public LocalizedFile(string german, string english)
    {
        German = german;
        English = english;
    }
}
