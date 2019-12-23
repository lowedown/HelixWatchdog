using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using HelixWatchdog.Core.Services;
using HelixWatchdog.Core.Validators;

namespace HelixWatchdog
{
    class Program
    {
        private const string Watchdog = @"          ,--._______,-. 
       ,','  ,    .  ,_`-. 
      / /  ,' , _` ``. |  )       `-.. 
     (,';'""""`/ '""`-._ ` \/ ______    \\ 
       : ,o.-`- ,o.  )\` -'      `---.)) 
       : , d8b ^-.   '|   `.      `    `. 
       |/ __:_     `. |  ,  `       `    \ 
       | ( ,-.`-.    ;'  ;   `       :    ; 
       | |  ,   `.      /     ;      :    \ 
       ;-'`:::._,`.__),'             :     ; 
      / ,  `-   `--                  ;     | 
     /  \                   `       ,      | 
    (    `     :              :    ,\      | 
     \   `.    :     :        :  ,'  \    : 
      \    `|-- `     \ ,'    ,-'     :-.-'; 
      :     |`--.______;     |        :    : 
       :    /           |    |         |   \ 
       |    ;           ;    ;        /     ; 
     _/--' |   -hrr-   :`-- /         \_:_:_| 
   ,',','  |           |___ \ 
   `^._,--'           / , , .) 
                      `-._,-' ";


        class Options
        {
            [Option('s', "source", Required = true, HelpText = "Path to the helix 'src' directory")]
            public string Source { get; set; }

            [Option('p', "pattern", Required = true, HelpText = "Pipe separated file pattern i.e. *.config|*.cs|*.csproj")]
            public string Pattern { get; set; }

            [Option('n', "namespace", Required = true, HelpText = "Namespace prefix i.E. MyProject.Feature. ==> Prefix: MyProject")]
            public string Namespace { get; set; }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => Run(opts))
                .WithNotParsed<Options>((errs) => HandleParseError(errs));
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Invalid arguments. Refer to the help screen --help");
        }

        static void Run(Options opts)
        {
            Console.Write(Watchdog);
            Console.WriteLine("Helix Watchdog is sniffing your solution...");
            Console.WriteLine();

            var rootPath = opts.Source;
            var pattern = opts.Pattern;
            var namespacePrefix =opts.Namespace;

            if (!Directory.Exists(opts.Source))
            {
                Console.WriteLine($"Directory {opts.Source} does not exist.");
                Environment.Exit(-1);
            }

            var validators = new List<IHelixFileValidator>()
            {
                new FoundationReference(),
                new ProjectToFeatureReference(),
                new WebsiteToAnyReference()
            };

            var results = new FileScannerService(new FileSystemService(), new HelixFactory(), validators)
                .ScanFiles(rootPath, pattern, namespacePrefix);

            Console.WriteLine($"Number of modules sniffed: {results.Count}. Pattern: {pattern}");

            int errorCount = 0;
            foreach (var errorModule in results.Where(r => r.Files.Any(f => f.InvalidReferences.Count > 0)))
            {
                Console.WriteLine("--------------------------------------");
                Console.WriteLine($"{errorModule.Layer} > {errorModule.Name}");
                Console.WriteLine($"Files: {errorModule.Files.Count} References: {errorModule.Files.Sum(f => f.References.Count)}");
                Console.WriteLine("--------------------------------------");

                foreach (var file in errorModule.Files.Where(f => f.InvalidReferences.Count > 0))
                {
                    Console.WriteLine($"> {file.Name}");

                    foreach (var invalidRef in file.InvalidReferences)
                    {
                        errorCount++;
                        ConsoleColor originalColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine($"{invalidRef.Value} invalid references to: {invalidRef.Key}");

                        Console.ForegroundColor = originalColor;
                    }
                }

                Console.WriteLine();
            }

            if (errorCount == 0)
            {
                Console.WriteLine("...nothing suspicious found. Carry on.");
                Environment.Exit(0);
            }

            Environment.Exit(-1);
        }
    }
}
