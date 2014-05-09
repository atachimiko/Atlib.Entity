using System;
using System.Reflection;
using ImpromptuInterface;
using Atlib.Attributes;

namespace Atlib.Entity.Decorators
{
	/// <summary>
	///     エンティティのプロパティの検証を行うビジネスロジック
	/// </summary>
	public class ValidationEntityDecorator : BusinessLogicDecorator
	{
		public ValidationEntityDecorator(IEntityData entity)
		{
			this._Entity = entity;
		}

		public object[] Arguments { get; set; }

		public override void Operation()
		{
			base.Operation();

			var clazz = _Entity.GetType();

			var attribute = clazz.GetCustomAttribute<EntitySaveValidationAttribute>();
			if (attribute == null) return ;

			var validator = Activator.CreateInstance(attribute.Validator);
			if (!validator.ActLike<IEntityValidation>().Validation(this._Entity, this.Arguments))
			{
				throw new EntityValidationException();
			}
		}

		readonly IEntityData _Entity;
	}
}
