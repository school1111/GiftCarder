using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GiftCarder.Plugins
{
	
	public class PluginLoader
	{
		public List<IPlugin> GetPlugins(string path = "Modules")
		{

			List<IPlugin> plug = new List<IPlugin>();
			var files = Directory.GetFiles(path, "*.ini");
			if(Program.IsT)
			foreach (var file in files)
			{
					try
					{
						var plugin = new Container(PSettings.GetSettings(file));
						plug.Add(plugin);
					}
					catch { }
			}
			return plug;
		}

		
	}
}
