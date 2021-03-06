﻿using System;
using System.Collections.Generic;
using System.IO;
using xNet;

namespace GiftCarder.Plugins
{
	public class PSettings
	{
		public string Name { get; set; }
		public string ModuleType { get; set; }
		public string CardDictionary { get; set; }
		public int LastSymReplace { get; set; }
		public string Url { get; set; }
		public string UrlTok { get; set; }
		public string UrlTokData { get; set; }
		public string UrlTokMethod { get; set; }
		public string UrlMethod { get; set; }
		public Dictionary<string,string> Tokens { get; set; }
		public Dictionary<string,string> Headers { get; set; }
		public string UrlData { get; set; }
		public string UseProxy { get; set; }
		public string Good { get; set; }
		public string Bad { get; set; }
		public string ParseBalance { get; set; }

		public string Captcha { get; set; }
		public string CaptchaServiseUrl { get; set; }
		public string CaptchaUrl { get; set; }
		public string ValidGiftCard { get; set; }
		public string Author { get; set; }
		public string Pin { get; set; }
		public string DeathByCaptchaAccount { get; set; }
		public static PSettings GetSettings(string file)
		{
			string s = File.ReadAllText(file);
			PSettings p = new PSettings
			{
				Name = s.GetData("Name"),
				CardDictionary = s.GetData("CardDictionary"),
				LastSymReplace = Int32.Parse(s.GetData("LastSymReplace")),
				ModuleType = s.GetData("ModuleType"),
				UrlTok = s.GetData("UrlTok"),
				UrlTokData = s.GetData("UrlTokData"),
				UseProxy = s.GetData("UseProxy"),
				UrlTokMethod = s.GetData("UrlTokMethod"),
				UrlData = s.GetData("UrlData"),
				Url = s.GetData("Url"),
				UrlMethod = s.GetData("UrlMethod"),
				Good = s.GetData("Good"),
				Bad = s.GetData("Bad"),
				Pin = s.GetData("Pin"),
				ValidGiftCard = s.GetData("ValidGiftCard"),
				ParseBalance = s.GetData("ParseBalance"),
				Author = s.GetData("Author"),
				Captcha = s.GetData("Captcha"),
				CaptchaUrl = s.GetData("CaptchaUrl"),
				DeathByCaptchaAccount = s.GetData("DeathByCaptchaAccount")
			};

			int i = 1;
			p.Tokens = new Dictionary<string, string>();
			p.Headers = new Dictionary<string, string>();
			string t = "";
			do
			{
				t = s.GetData("ParseTok" + i);
				if (t.Length > 3)
					p.Tokens.Add("ParseTok" + i, t);
				i++;
			} while (t != "");
			t = "";
			i = 1;
			do
			{
				t = s.GetData("ParseTokCookie" + i);
				if (t.Length > 3)
					p.Tokens.Add("ParseTokCookie" + i, t);
				i++;
			} while (t != "");
			t = "";
			i = 1;
			try
			{
				do
				{
					t = s.GetData("Header" + i);
					if (t.Length > 3)
						p.Headers[t.Split('|')[0]] = t.Split('|')[1];
					i++;
				} while (t != "");
			}
			catch { }
			return p;
		}
	}
	public static class Help
	{
		public static string GetData(this string source, string Tok)
		{
			return Parse(source, Tok);
		}

		private static string Parse(string s, string p)
		{
			return s.Substring($"<{p}>", $"</{p}>", 0);
		}
	}

}
