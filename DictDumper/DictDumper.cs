using System;
using System.IO;
using System.Diagnostics;
using OpenDexterity.DictionaryApi;
using System.Linq;

namespace DictLibraryTester {
    class DictDumper {
        static void Pause() {
            //hanging until user presses something
            Console.WriteLine("Press a key to close this window...");
            Console.ReadKey(true);
        }

        static int Main(string[] args) {
            if (args.Length != 1) {
                Console.WriteLine($"Wrong number of arguments. Got {args.Length}, expected 1.");
                Pause();
                return 1;
            }

            //basic argument checking and all that
            string dictPath = Path.GetFullPath(args[0]);
            Console.WriteLine($"Using file {dictPath}");
            if (!File.Exists(dictPath)) {
                Console.WriteLine("Dictionary does not exist.");
                Pause();
                return 1;
            } else
                Console.WriteLine("Dictionary exists.");

            //constructor stuff
            Stopwatch s = new();
            s.Start();
            Dictionary dict = new(dictPath);
            s.Stop();
            Console.WriteLine($"Constructor ran for {s.ElapsedTicks} ticks. ({s.ElapsedMilliseconds} ms)");
            Console.WriteLine();

            //header
            Console.WriteLine("Extrapolated info from header:");
            Console.WriteLine($"File size: {dict.HeaderFileSize}");
            Console.WriteLine($"Block table offset: {dict.BlockTableOffset}");
            Console.WriteLine($"Block table length: {dict.BlockTableLength}");
            Console.WriteLine($"Unallocated block count: {dict.UnallocatedBlocks}");
            Console.WriteLine();

            //blocks
            var blk1 = dict.Blocks[0];
            var blk2 = dict.Blocks[1];
            var blk3 = dict.Blocks[2];
            Console.WriteLine($"Blocks array contains {dict.Blocks.Length} item(s).");
            Console.WriteLine($"Block 1: Type={blk1.Type}/{(ushort)blk1.Type}, Start={blk1.Start}, Size={blk1.Size}, Unused={blk1.Unused}, Used={blk1.Used}");
            Console.WriteLine($"Block 2: Type={blk2.Type}/{(ushort)blk2.Type}, Start={blk2.Start}, Size={blk2.Size}, Unused={blk2.Unused}, Used={blk2.Used}");
            Console.WriteLine($"Block 3: Type={blk3.Type}/{(ushort)blk3.Type}, Start={blk3.Start}, Size={blk3.Size}, Unused={blk3.Unused}, Used={blk3.Used}");
            Console.WriteLine();

            //modules
            Console.WriteLine($"Modules array contains {dict.Modules.Length} item(s).");
            Console.WriteLine($"Module table says there are {dict.UsedModules} used module(s), " +
                $"{dict.UnusedModules} unused module(s), and {dict.TotalModules} total.");

            //don't listen to visual studio, these casts are not redundant
            #pragma warning disable IDE0004
            double type1Percent = (double)dict.Modules.Where(x => x.Type == 1).Count() / (double)dict.TotalModules;
            #pragma warning restore IDE0004
            Console.WriteLine($"{type1Percent:P} of the modules are type 1.");
            Console.WriteLine();

            return 0;
        }
    }
}
