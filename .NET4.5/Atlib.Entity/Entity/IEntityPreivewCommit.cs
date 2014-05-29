using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Entity
{
	public interface IEntityPreivewCommit
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="context"></param>
		void PreviewCommit(IEntityData target, DbContext context);
	}
}
