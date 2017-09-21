using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftCarder.Plugins
{

	public interface IPlugin
	{
		void Start(int count, string s);
		void Stop();
		void Check();
		string GetStats();
		string GetName();
		PSettings GetSettings();
	}
}
