using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using GiftCarder.Plugins;
using System.Security.Cryptography;

namespace GiftCarder
{
	class Program
	{
		static string ver = "1.0";
		static string status = string.Empty;
		static string name = string.Empty;
		private static Dictionary<string, IPlugin> PluginsDictionary;
		private static Dictionary<string, string> KeyDictionaty;
		public static bool IsT;
		static void Main(string[] args)
		{
			status = "Welcome";
			Console.Title = String.Format(Messages("maint"), messages.programname, status, ver);
			if (Process.GetProcessesByName("GiftCarder").Length > 1)
			{
				status = "Already Running";
				Console.Title = String.Format(Messages("maint"), messages.programname, status, ver);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("GiftCarder Already Running, Closing...");
				Console.ReadKey();
				Environment.Exit(0);
				return;
			}
			using (WebClient webClient = new WebClient())
			{
				try
				{
                    if (ver == webClient.DownloadString("http://giftcarder.pl/v.txt"))
                    {
                        status = "Connecting to server...";
                        Console.Title = String.Format(Messages("maint"), messages.programname, status, ver);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(Messages("tryingconnect"));
                        string res = webClient.DownloadString("http://giftcarder.pl/c.txt");
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (res.Equals("Thanks"))
                        {
                            IsT = false;
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(Messages("successconnect"));
                            status = "Connected";
                            Console.Title = String.Format(Messages("maint"), messages.programname, status, ver);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.OutputEncoding = Encoding.UTF8;
                            status = "Connected & Authentificated";
                            PluginLoader pLoader = new PluginLoader();
                            IsT = true;
                            var plugins = pLoader.GetPlugins();
                            PluginsDictionary = new Dictionary<string, IPlugin>();
                            foreach (var p in plugins)
                            {
                                PluginsDictionary[p.GetName()] = p;
                            }

                            Auth();
						}
						else
						{
							status = "Can't reach server";
							Console.Title = String.Format(Messages("maint"), messages.programname, status, ver);
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine(Messages("cantconnect"));
							Console.ReadKey();
							Environment.Exit(0);
						}
					}
					else
					{
						status = "Update Available";
						Console.Title = String.Format(Messages("maint"), messages.programname, status, ver);
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(Messages("updateava"));
						Console.ReadKey();
						Environment.Exit(0);
					}
				}
				catch (Exception ex)
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendLine(ex.Message);
					sb.AppendLine("-------------------------------------------------");
					sb.AppendLine(ex.Source);
					sb.AppendLine("-------------------------------------------------");
					sb.AppendLine(ex.StackTrace);
					sb.AppendLine("-------------------------------------------------");
					sb.AppendLine(ex.Data.ToString());
					File.WriteAllText("Exception.txt",sb.ToString());
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(String.Format(Messages("exception"), sb.ToString()));
					Console.ReadKey();
				}
			}
		}
		static public void Auth()
		{
			
			Console.Clear();
			Console.Title = String.Format(Messages("welcomeback"), messages.programname, status, ver);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(String.Format("Welcome to {0}", messages.programname));
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(Messages("module use"));
			Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(500);
            Console.WriteLine(Messages("line1"));
            int counter = 0;
			KeyDictionaty = new Dictionary<string, string>();
			foreach (var plug in PluginsDictionary)
			{
				counter++;
				
                Console.ForegroundColor = ConsoleColor.Yellow;
               if(plug.Value.GetSettings().Author == "")
                {
                    Console.WriteLine($"► {plug.Key} [{counter}]                                     ");
                }
                else{
                    Console.WriteLine($"► {plug.Key} by {plug.Value.GetSettings().Author} [{counter}]                                     ");
                }
				KeyDictionaty[counter.ToString()] = plug.Key;
				Thread.Sleep(30);
			}
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(Messages("line1"));
			Console.ForegroundColor = ConsoleColor.Yellow;
			Thread.Sleep(500);
			Console.WriteLine(Messages("select module"));
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(500);
            Console.WriteLine(Messages("line1"));
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Official GiftCarder discord: https://discord.gg/DCc8PwH & http://giftcarder.pl");
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Messages("line1"));
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Program Writen by xPolish aka NextGenZ");
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Messages("line1"));
            string str = Console.ReadLine();

			try
			{
				LoadModule(KeyDictionaty[str]);
			}
			catch
			{
				
			}

			Auth();
		}
		static public void LoadModule(string pName)
		{
            var p = PluginsDictionary[pName];
            Console.Clear();
			Console.Title = String.Format("{0} | STATUS: {1} | Module: {2} | {3}", messages.programname, status, pName, ver);
			Console.ForegroundColor = ConsoleColor.Green;
			if(p.GetSettings().Author == "")
            {
                Console.WriteLine($"You selected Module: {pName}");
            }
            else {
                Console.WriteLine($"You selected Module: {pName} by {p.GetSettings().Author}");
            }
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("--------------------------------------------------");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Thread.Sleep(500);
			var sig = "";
			if(p.GetSettings().ValidGiftCard=="")
			do
			{
				Console.WriteLine("► Please input a correct GiftCard to start Generating & Checking");
				sig = Console.ReadLine();
			} while (sig == "");
			var thr = "";
			int threads;
			do
			{
				Console.WriteLine("► Please input count of threads");
				thr = Console.ReadLine();
			} while (!int.TryParse(thr, out threads));
            Console.Clear();
            if (p.GetSettings().Author == "")
            {
                Console.WriteLine($"You started Carding: {pName}");
            }
            else
            {
                Console.WriteLine($"You started Carding: {pName} by {p.GetSettings().Author}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("--------------------------------------------------");
            p.Start(threads, p.GetSettings().ValidGiftCard);
            string var1 = "";
			do
			{
				var1 = Console.ReadLine();
				if (var1.Contains("stats"))
				{
					Console.WriteLine($"Stats: {p.GetStats()}");
					continue;
				}
				if (var1.Contains("clear"))
				{
					Console.Clear();
					continue;
				}
				if (var1.Contains("stop"))
				{
					p.Stop();
					Auth();
					break;
				}
				if (var1.Contains("help"))
				{
					Console.WriteLine("Commands: \r\nstats\r\nstop\r\nhelp\r\nclear");
					continue;
				}
				
			} while (true);
			
		}
		public static string Messages(string arg)
		{
			string res = "";
			if (arg.Equals("gen"))
			{
				res = "► Please input a correct GiftCard to start Generating & Checking";
			}
			if (arg.Equals("select module"))
			{
				res = "Please select Module number and input it down.    ";
			}
			if (arg.Equals("line1"))
			{
				res = "--------------------------------------------------";
			}
			if (arg.Equals("inmodule"))
			{
				res = "{0} | STATUS: {1} | Module: {2} | {3}";
			}
			if (arg.Equals("inchecking"))
			{
				res = "{0} | STATUS: {1} | Module: {2} | HITS: {3} | {4}";
			}
			if (arg.Equals("line2"))
			{
				res = "▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩▩";
			}
			if (arg.Equals("module use"))
			{
				res = "¿Which Module Do You Want To Use?";
			}
			if (arg.Equals("maint"))
			{
				res = "{0} | STATUS: {1} | {2}";
			}
			if (arg.Equals("invalidkey"))
			{
				res = "Invalid Key or HWID, Closing...";
			}
			if (arg.Equals("cantconnect"))
			{
				res = "Cannot connect to server, Closing...";
			}
			if (arg.Equals("providekey"))
			{
				res = "Please provide your key";
			}
			if (arg.Equals("exception"))
			{
				res = "Exception:\n{0}";
			}
			if (arg.Equals("updateava"))
			{
				res = "New Update available check giftcarder.pl to download, Closing...";
			}
			if (arg.Equals("successconnect"))
			{
				res = "Successfully connected to server.\n";
			}
			if (arg.Equals("tryingconnect"))
			{
				res = "Trying to connect to server...";
			}
			if (arg.Equals("welcomeback"))
			{
				res = "{0} | STATUS: {1} | {2}";
			}
			if (arg.Equals("checking"))
			{
				res = "You started Carding: {0}";
			}

			return res;
		}

	}
}
