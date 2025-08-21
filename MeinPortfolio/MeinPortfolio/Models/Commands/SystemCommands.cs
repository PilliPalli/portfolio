using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MeinPortfolio.Models.Commands
{
    public class ClearCommand : BaseCommand
    {
        public override string Name => "clear";
        public override string Description => "Clear the terminal screen";
        public override string Usage => "clear";

        public override Task<string> ExecuteAsync(string[] args)
        {
            return Task.FromResult("clear");
        }

    }
    
    public class DateCommand : BaseCommand
    {
        public override string Name => "date";
        public override string Description => "Display the current date and time";
        public override string Usage => "date";

        public override Task<string> ExecuteAsync(string[] args)
        {
            return Task.FromResult(System.DateTime.Now.ToString("F"));
        }
    }

    public class WhoamiCommand : BaseCommand
    {
        public override string Name => "whoami";
        public override string Description => "Display information about the portfolio owner";
        public override string Usage => "whoami";

        public override Task<string> ExecuteAsync(string[] args)
        {
            return Task.FromResult("Moritz Nicola Kreis\n");
        }
    }
}
