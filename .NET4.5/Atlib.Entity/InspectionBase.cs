using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib
{
	public abstract class InspectionBase
	{
		public string LastInspectionMessage { get; private set; }

		protected bool ErrorThrow(string message)
		{
			this.LastInspectionMessage = message;
			return false;
		}
	}
}
