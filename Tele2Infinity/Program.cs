using ByteSizeLib;
using Newtonsoft.Json;

namespace Tele2Infinity
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.Title = "Tele2Infinity";
                Console.WriteLine("-> Tele2Infinity <-");
                Console.WriteLine("-> To Infinity and Beyond!");
                Console.WriteLine();
                await MainLoop();
            }
            catch (Exception e)
            {
                await File.WriteAllTextAsync("exception.txt", e.ToString());
                throw;
            }
        }

        private static async Task MainLoop()
        {
            var config = await LoadConfig();
            var tele2 = new Tele2();

            string bearerToken;
            if (!File.Exists("bearer.txt"))
            {
                Console.WriteLine("No bearer token found yet.");
                Console.WriteLine("Please enter your email/username:");
                var username = Console.ReadLine();
                Console.WriteLine("Please enter your password:");
                var password = GetPassword();
                bearerToken = await tele2.Login(username, password);
                await File.WriteAllTextAsync("bearer.txt", bearerToken);
                Console.WriteLine("Bearer token saved!");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Loaded bearer token from bearer.txt!");
                bearerToken = await File.ReadAllTextAsync("bearer.txt");
                tele2.SetBearer(bearerToken);
            }

            Console.Write("-> Loading profile.. ");
            await tele2.LoadProfile();
            Console.WriteLine($"OK! [{tele2.CustomerId}]");
            Console.WriteLine($"-> Running for #{tele2.Phone}");
            Console.WriteLine($"-> Activating {config.TargetBundleCode} when less then {config.BundleThresholdInMB}MB remaining in bundle");

            // ReSharper disable once InconsistentNaming
            var internetSpeedInMB = Math.Round(config.InternetSpeedInMbps / 8, 2);

            Console.WriteLine($"-> Using {config.InternetSpeedInMbps}Mbps ({internetSpeedInMB}MB/s) as internet speed");
            Console.WriteLine();

            double lastMbMeasure = 0;
            var addEvenMoreTime = false;
            while (true)
            {
                Console.Write($"Retrieving bundleinfo @ {DateTime.Now.ToShortTimeString()}.. ");
                var bundleResult = await tele2.GetBundleStatus();
                var availableBundles = await tele2.GetAvailableRoamingBundles();
                Console.WriteLine("OK!");
                
                var remaining = bundleResult?.Bundles.SelectMany(x => x.Buckets).Sum(x => x.RemainingValue) ?? 0;
                var mbsRemaining = ByteSize.FromKiloBytes(remaining).MegaBytes;
                var secondsRemaining = mbsRemaining / internetSpeedInMB; // Wait time in worst case internet drain.
                var waitTime = TimeSpan.FromSeconds(secondsRemaining);

                var bundleTarget = availableBundles?.Bundles.FirstOrDefault(x => x.BundleCode == config.TargetBundleCode);
                Console.WriteLine($"Remaining daily data: {mbsRemaining}MB");
                Console.WriteLine("Bundle available yet? " + (bundleTarget == null ? "No" : "Yes: " + bundleTarget.BuyingCode));

                if (bundleTarget != null && mbsRemaining < config.BundleThresholdInMB)
                {
                    Console.Write($"Buying {bundleTarget.BuyingCode}.. ");
                    await tele2.BuyRoamingBundle(bundleTarget.BuyingCode);
                    Console.WriteLine("OK!");
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    continue;
                }

                if (Math.Abs(lastMbMeasure - mbsRemaining) < 10 && mbsRemaining > 10)
                {
                    var extraSleep = TimeSpan.FromMilliseconds(mbsRemaining * 100 + 2 * 60 * 1000);
                    if (addEvenMoreTime)
                    {
                        extraSleep *= 2;
                        if (extraSleep.TotalMinutes < 10)
                        {
                            extraSleep = TimeSpan.FromMinutes(10);
                        }
                    }

                    waitTime = waitTime.Add(extraSleep);
                    addEvenMoreTime = true;
                    Console.WriteLine($"Almost no data use, adding some extra sleep. ({Math.Round(extraSleep.TotalMinutes, 1)} minutes)");
                }
                else
                {
                    addEvenMoreTime = false;
                }

                Console.WriteLine($"Sleeping for {Math.Round(waitTime.TotalMinutes, 1)} minutes.. (Until {DateTime.Now.Add(waitTime).ToShortTimeString()})");
#if DEBUG
                await Task.Delay(5000);
#else
                await Task.Delay(waitTime);
#endif
                Console.WriteLine();

                lastMbMeasure = mbsRemaining;
            }
        }

        private static string GetPassword()
        {
            var pwd = "";
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length <= 0) continue;

                    pwd = pwd.Substring(0, pwd.Length - 1);
                    Console.Write("\b \b");
                }
                else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                {
                    pwd += i.KeyChar;
                    Console.Write("*");
                }
            }
            return pwd;
        }

        private static async Task<Tele2Config> LoadConfig()
        {
            if (!File.Exists("config.json"))
            {
                await File.WriteAllTextAsync("config.json", JsonConvert.SerializeObject(new Tele2Config(), Formatting.Indented));
            }
            var configJson = await File.ReadAllTextAsync("config.json");
            var config = JsonConvert.DeserializeObject<Tele2Config>(configJson) ?? new Tele2Config();

            return config;
        }
    }
}