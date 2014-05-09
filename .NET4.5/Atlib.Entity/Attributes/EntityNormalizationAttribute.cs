using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Attributes
{
	[AttributeUsage(AttributeTargets.Property,AllowMultiple=true)]
	public class EntityNormalizationAttribute : Attribute
	{
		public readonly Type Normalization;
		public EntityNormalizationAttribute(Type normalization)
		{
			this.Normalization = normalization;
		}
	}
}
