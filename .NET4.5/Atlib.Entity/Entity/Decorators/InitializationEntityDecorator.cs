using Atlib.Attributes;
using log4net;
using System;
using System.Text;
using System.Reflection;
using ImpromptuInterface;

namespace Atlib.Entity.Decorators
{
	/// <summary>
	///     EntityInitialization属性が付与してある場合、初期化処理を実行します。
	/// </summary>
	public class InitializationEntityDecorator : BusinessLogicDecorator
	{
		static ILog LOG = LogManager.GetLogger(typeof(InitializationEntityDecorator));

		public InitializationEntityDecorator(IEntityData data)
		{
			this._Entity = data;
		}

		public object[] Arguments { get; set; }

		public override void Operation()
		{
			base.Operation();

			// 初期化のみ呼び出す
			if (_Entity.Id == 0L)
			{
				var clazz = _Entity.GetType();

				var attribute = clazz.GetCustomAttribute<EntityInitializationAttribute>();
				if (attribute == null) return; // EntityInitialization属性が無い場合はそのまま終了。

				var validator = Activator.CreateInstance(attribute.Initializer);
				validator.ActLike<IEntityInitialization>().Initialization(this._Entity, this.Arguments);
			}
		}

		readonly IEntityData _Entity;
	}
}
