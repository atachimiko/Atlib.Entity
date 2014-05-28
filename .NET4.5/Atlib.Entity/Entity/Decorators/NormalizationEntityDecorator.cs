using Atlib.Attributes;
using log4net;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImpromptuInterface;

namespace Atlib.Entity.Decorators
{
	/// <summary>
	///     
	/// </summary>
	public class NormalizationEntityDecorator : BusinessLogicDecorator
	{
		static ILog LOG = LogManager.GetLogger(typeof(NormalizationEntityDecorator));

		public NormalizationEntityDecorator(IEntityData data)
		{
			this._Entity = data;
		}

		public object[] Arguments { get; set; }

		public override void Operation()
		{
			base.Operation();

			var clazz = _Entity.GetType();
			foreach (var prop in clazz.GetProperties())
			{
				var attr = prop.GetCustomAttribute<EntityNormalizationAttribute>();
				if (attr != null)
				{
					var normalizer = Activator.CreateInstance(attr.Normalization);
					normalizer.ActLike<IEntityNormalization>().Normalization(this._Entity, prop, Arguments);
				}
			}
		}

		readonly IEntityData _Entity;
	}
}
