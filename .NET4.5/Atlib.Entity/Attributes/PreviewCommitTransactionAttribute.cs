using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlib.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PreviewCommitTransactionAttribute : Attribute
	{
		public readonly Type PreviewCommitTransactionClazz;

		public PreviewCommitTransactionAttribute(Type previewCommitTransactionClazz)
		{
			this.PreviewCommitTransactionClazz = previewCommitTransactionClazz;
		}
	}
}
