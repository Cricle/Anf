using Kw.Comic.Engine;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Consolat.Commands
{
    public class SearchCommand : AsyncCommand<SearchCommand.Settings>
    {
        public class Settings:CommandSettings
        {
            [CommandArgument(0,"[Some keyword]")]
            public string Keyword { get; set; }

            public int? Skip { get; set; }

            public int? Take { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var eng = AppEngine.GetRequiredService<SearchEngine>();
            AnsiConsole.Markup("[red]Gook luck[/]");
            await AnsiConsole.Status()
                .StartAsync($"Searching {settings.Keyword}", async ctx =>
                {
                    ctx.Spinner = Spinner.Known.Star;
                    try
                    {
                        var ds = await eng.SearchAsync(settings.Keyword, 0, 10);
                        ctx.Status = $"[blue]Filling data[/]";
                        var table = new Table();
                        var columns = new string[]
                        {
                            "Name",
                            "Author",
                            "Sources"
                        };
                        table.AddColumns(columns);
                        foreach (var item in ds.Snapshots)
                        {
                            var tb = new Table();
                            tb.AddColumns("SourceName","Link");
                            foreach (var lnk in item.Sources)
                            {                                
                                tb.AddRow(lnk.Name, lnk.TargetUrl);
                            }
                            
                            table.AddRow(new Markup($"[blue]{item.Name}[/]"),new Markup(item.Author), tb);
                        }
                        AnsiConsole.Render(table);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteException(ex);
                    }
                });
            return 0;
        }
    }
}
