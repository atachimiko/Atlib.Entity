using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Entity
{
	/// <summary>
	/// 
	/// </summary>
	public interface IEntityNormalization
	{
		void Normalization(IEntityData entity, PropertyInfo property, object[] arguments);
	}
}
