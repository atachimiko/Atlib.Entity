using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Entity
{
	public interface IEntityInitialization
	{
		void Initialization(IEntityData entity, params object[] args);
	}
}
