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
		static string status = string.Empty;
		static string name = string.Empty;
		private static Dictionary<string, IPlugin> PluginsDictionary;
		private static Dictionary<string, string> KeyDictionaty;
		public static bool IsT;
		static void Main(string[] args)
		{
			status = "Welcome";
			Console.Title = String.Format(messages.maint, messages.programname, status, messages.ver);
			if (Process.GetProcessesByName("GiftCarder").Length > 1)
			{
				status = "Already Running";
				Console.Title = String.Format(messages.maint, messages.programname, status, messages.ver);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(messages.already_running);
				Console.ReadKey();
				Environment.Exit(0);
				return;
			}
			using (WebClient webClient = new WebClient())
			{
				try
				{
                    if (messages.ver == webClient.DownloadString("http://giftcarder.pl/v.txt"))
                    {
                        status = "Connecting to server...";
                        Console.Title = String.Format(messages.maint, messages.programname, status, messages.ver);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(messages.trying_connect);
                        string res = webClient.DownloadString("http://giftcarder.pl/c.txt");
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (res.Equals("Thanks"))
                        {
                            IsT = false;
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(messages.success_connect);
                            status = "Connected";
                            Console.Title = String.Format(messages.maint, messages.programname, status, messages.ver);
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
							Console.Title = String.Format(messages.maint, messages.programname, status, messages.ver);
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine(messages.cant_connect);
							Console.ReadKey();
							Environment.Exit(0);
						}
					}
					else
					{
						status = "Update Available";
						Console.Title = String.Format(messages.maint, messages.programname, status, messages.ver);
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(messages.update_available);
						Console.ReadKey();
						Environment.Exit(0);
					}
				}
				catch (Exception ex)
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendLine(ex.Message);
					sb.AppendLine(messages.line);
					sb.AppendLine(ex.Source);
                    sb.AppendLine(messages.line);
                    sb.AppendLine(ex.StackTrace);
                    sb.AppendLine(messages.line);
                    sb.AppendLine(ex.Data.ToString());
					File.WriteAllText("Exception.txt",sb.ToString());
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(String.Format(messages.exception, sb.ToString()));
					Console.ReadKey();
				}
			}
		}
		static public void Auth()
		{
			
			Console.Clear();
			Console.Title = String.Format(messages.welcome_back, messages.programname, status, messages.ver);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(String.Format(messages.welcome_to, messages.programname));
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(messages.module_use);
			Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(500);
            Console.WriteLine(messages.line);
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
			Console.WriteLine(messages.line);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Thread.Sleep(500);
			Console.WriteLine(messages.select_module);
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(500);
            Console.WriteLine(messages.line);
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(messages.official_giftcarder);
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(messages.line);
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(messages.author);
            Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(messages.line);
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
			Console.Title = String.Format(messages.inmodule, messages.programname, status, pName, messages.ver);
			Console.ForegroundColor = ConsoleColor.Green;
			if(p.GetSettings().Author == "")
            {
                Console.WriteLine($"You selected Module: {pName}");
            }
            else {
                Console.WriteLine($"You selected Module: {pName} by {p.GetSettings().Author}");
            }
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(messages.line);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Thread.Sleep(500);
			var sig = "";
			if(p.GetSettings().ValidGiftCard=="")
			do
			{
				Console.WriteLine(messages.gen);
				sig = Console.ReadLine();
			} while (sig == "");
			var thr = "";
			int threads;
			do
			{
				Console.WriteLine(messages.count_threads);
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
            Console.WriteLine(messages.line);
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
					Console.WriteLine(messages.commands);
					continue;
				}
				
			} while (true);
			
		}

	}
}
