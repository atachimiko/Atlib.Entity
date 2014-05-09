using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Attributes
{
	[AttributeUsage(AttributeTargets.Class)] 
	public class EntitySaveValidationAttribute : Attribute
	{
		public readonly Type Validator;
		public EntitySaveValidationAttribute(Type validator)
		{
			this.Validator = validator;
		}
	}
}
