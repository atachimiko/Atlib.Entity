using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class EntityInitializationAttribute : Attribute
	{
		public readonly Type Initializer;
		public EntityInitializationAttribute(Type initializer)
		{
			this.Initializer = initializer;
		}
	}
}
