using System.Threading.Tasks;
using Portfolio.Services;

namespace Portfolio.Models.Commands
{
    public class CdCommand : BaseCommand
    {
        private readonly NavigationService _navigationService;

        public override string Name => "cd";
        public override string Description => "Change to a different section";
        public override string Usage => "cd [section]";

        public CdCommand(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override Task<string> ExecuteAsync(string[] args)
        {
            if (args.Length == 0)
            {
                return Task.FromResult($"Current location: {_navigationService.GetSectionPath()}\nUse 'cd [section]' to navigate to a different section.");
            }

            var section = args[0].ToLower();
            
            switch (section)
            {
                case "about":
                    _navigationService.NavigateTo(NavigationSection.About);
                    return Task.FromResult($"Navigated to about section.");
                
                case "projects":
                    _navigationService.NavigateTo(NavigationSection.Projects);
                    return Task.FromResult($"Navigated to projects section.");
                
                case "experience":
                    _navigationService.NavigateTo(NavigationSection.Experience);
                    return Task.FromResult($"Navigated to experience section.");
                
                case "contact":
                    _navigationService.NavigateTo(NavigationSection.Contact);
                    return Task.FromResult($"Navigated to contact section.");
                
                case "~":
                case "home":
                    _navigationService.GoHome();
                    return Task.FromResult($"Navigated to home section.");
                
                default:
                    return Task.FromResult($"Section '{section}' not found. Available sections: about, experience, projects, contact, home");
            }
        }
    }

    public class PwdCommand : BaseCommand
    {
        private readonly NavigationService _navigationService;

        public override string Name => "pwd";
        public override string Description => "Print current section path";
        public override string Usage => "pwd";

        public PwdCommand(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override Task<string> ExecuteAsync(string[] args)
        {
            return Task.FromResult(_navigationService.GetSectionPath());
        }
    }

    public class LsCommand : BaseCommand
    {
        private readonly NavigationService _navigationService;

        public override string Name => "ls";
        public override string Description => "List sections";
        public override string Usage => "ls";

        public LsCommand(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override Task<string> ExecuteAsync(string[] args)
        {
            var section = _navigationService.CurrentSection;
            
            return section switch
            {
                NavigationSection.Home => Task.FromResult("about    experience  projects   contact"),
                NavigationSection.About => Task.FromResult(string.Join("   ", VirtualFileSystem.GetFiles(section).Keys)),
                NavigationSection.Projects => Task.FromResult(string.Join("   ", VirtualFileSystem.GetFiles(section).Keys)),
                NavigationSection.Experience => Task.FromResult(string.Join("   ", VirtualFileSystem.GetFiles(section).Keys)),
                NavigationSection.Contact => Task.FromResult(string.Join("   ", VirtualFileSystem.GetFiles(section).Keys)),
                _ => Task.FromResult("Unknown section")
            };

        }
    }

    public class HomeCommand : BaseCommand
    {
        private readonly NavigationService _navigationService;

        public override string Name => "home";
        public override string Description => "Navigate to the home section";
        public override string Usage => "home";

        public HomeCommand(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override Task<string> ExecuteAsync(string[] args)
        {
            _navigationService.GoHome();
            return Task.FromResult("Navigated to Home section.");
        }
    }
    

    public class CatCommand : BaseCommand
    {
        private readonly NavigationService _navigationService;
        private readonly LanguageService _languageService;

        public override string Name => "cat";
        public override string Description => "Display file contents";
        public override string Usage => "cat <filename>";

        public CatCommand(NavigationService navigationService, LanguageService languageService)
        {
            _navigationService = navigationService;
            _languageService = languageService;
        }

        public override Task<string> ExecuteAsync(string[] args)
        {
            if (args.Length == 0)
            {
                return Task.FromResult("Usage: cat <filename>");
            }

            var filename = args[0];
            var section = _navigationService.CurrentSection;

            if (section == NavigationSection.Home)
            {
                return Task.FromResult("cat: cannot display directory contents. Use 'cd <section>' to navigate to a section first.");
            }

            var content = VirtualFileSystem.GetFileContent(section, filename, _languageService);
            if (content == null)
            {
                return Task.FromResult($"cat: {filename}: No such file or directory");
            }

            return Task.FromResult(content);
        }
    }

}
