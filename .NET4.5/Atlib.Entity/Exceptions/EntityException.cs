using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Exceptions
{
	public class EntityException : Exception
	{
		public EntityException(string message)
			: base(message)
		{

		}

		public EntityException(string message, Exception innner)
			: base(message, innner)
		{

		}


	}
}
