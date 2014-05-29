using EntityFramework.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ImpromptuInterface;
using System.Data.Entity;
using Atlib;
using Atlib.Entity.Decorators;
using Atlib.Entity.Infrastructures;
using Atlib.Attributes;

namespace Atlib.Entity
{
	public class EntityModelBase<T> : DynamicObject, INotifyPropertyChanged
		where T : class ,IEntityData
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///     エンティティとそのエンティティを参照・保存するために使用するコンストラクタ
		/// </summary>
		/// <param name="entity" type="T">
		///     <para>
		///         非セッション
		///     </para>
		/// </param>
		/// <param name="context" type="System.Data.Entity.DbContext">
		///     <para>
		///         
		///     </para>
		/// </param>
		public EntityModelBase(T entity, DbContext context)
		{
			Guard.NotNull(entity, "entity");
			Guard.NotNull(context, "context");

			this._Context = context;
			this.ModelInstance = entity;

			if (entity.Id != 0L)
			{
				// entityが非セッションでないと、複数のセッションに所属するエンティティになる場合があるため、例外がスローされる。
				this._Context.Set<T>().Attach(entity);
			}

			InConstractor(entity);
		}

		/// <summary>
		/// モデルをプロキシ経由でアクセスする
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <returns></returns>
		public TInterface Proxy<TInterface>()
			where TInterface : class
		{
			return this.ActLike<TInterface>();
		}

		public void Save()
		{			
			Save(this._Context);
		}

		/// <summary>
		///    エンティティの検証を実行する
		/// </summary>
		/// <returns></returns>
		public bool Validation(IEntityData entity, params object[] args)
		{
			try
			{
				var decorator = new ValidationEntityDecorator(entity);
				decorator.Arguments = args;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		protected void Save(DbContext ctx)
		{
			Guard.NotNull(ctx, "コンテキスト");

			bool saveFailed;

			do
			{
				saveFailed = false; 

				try
				{
					var @adp = new DbContextAdapter(ctx);
					var @uow = new UnitOfWork(@adp);
					var @repo = new AuditableRepository<T>(new EntityFramework.Patterns.Repository<T>(@adp));

					// この呼び出しでは、新規の場合に外部参照のエンティティをctxコンテキストで再設定してください。
					// でないと、外部参照しているエンティティが新規で登録されてしまいます。
					this.OnPreviewCommit(ctx);

					if (this.ModelInstance.Id == 0)
					{
						@repo.Insert(this.ModelInstance);
					}
					else
					{
						@repo.Update(this.ModelInstance);
					}
					@uow.Commit();
				}
				catch (DbUpdateConcurrencyException ex)
				{
					saveFailed = true;

					var entry = ex.Entries.Single();
					entry.OriginalValues.SetValues(entry.GetDatabaseValues());
				}
			} while (saveFailed);
		}

		public void SetPropertyRelation(string keyPropertyName, string propertyName)
		{
			if (!propertyRelationDictionary.ContainsKey(keyPropertyName))
			{
				propertyRelationDictionary[keyPropertyName] = new List<string>();
			}

			propertyRelationDictionary[keyPropertyName].Add(propertyName);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = GetProperty(_modelType, binder.Name);
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			SetProperty(_modelType, binder.Name, value);
			OnPropertyChanged(binder.Name);
			if (propertyRelationDictionary.ContainsKey(binder.Name))
			{
				propertyRelationDictionary[binder.Name].ForEach(propertyName => OnPropertyChanged(propertyName));
			}
			return true;
		}

		public long Id
		{
			get { return _modelObject.Id; }
		}

		public T ModelInstance
		{
			protected set
			{
				_modelObject = value;
			}
			get
			{
				return _modelObject;
			}
		}

		/// <summary>
		/// コンテキストがNullの場合もあります。また、コンテキストが無効となっている場合もあります。
		/// </summary>
		protected DbContext _Context = null;

		protected T _modelObject = default(T);
		protected Type _modelType = null;

		/// <summary>
		/// エンティティの保存時にコミット呼び出し前に任意の処理を行う場合、
		/// このメソッドをオーバーライドしてください。
		/// </summary>
		protected virtual void OnPreviewCommit(DbContext context)
		{
			var clazz = typeof(T);
			var attribute = clazz.GetCustomAttribute<PreviewCommitTransactionAttribute>();
			if (attribute == null) return; // EntityInitialization属性が無い場合はそのまま終了。

			var transactioner = Activator.CreateInstance(attribute.PreviewCommitTransactionClazz);
			transactioner.ActLike<IEntityPreivewCommit>().PreviewCommit(this.ModelInstance, this._Context);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;
			if (handler != null)
			{
				var e = new PropertyChangedEventArgs(propertyName);
				handler(this, e);
			}
		}
		WeakEventListener<PropertyChangedEventManager, PropertyChangedEventArgs> _listener = null;
		private static Dictionary<Type, Dictionary<string, Func<object, object>>> getFuncDictionary = new Dictionary<Type, Dictionary<string, Func<object, object>>>();

		//OnPropertyChangedを伝播させるための辞書
		private Dictionary<string, List<string>> propertyRelationDictionary = new Dictionary<string, List<string>>();
		private static Dictionary<Type, Dictionary<string, Action<object, object>>> setFuncDictionary = new Dictionary<Type, Dictionary<string, Action<object, object>>>();

		private object GetProperty(Type targetType, string propertyName)
		{
			Func<object, object> func;

			if (!getFuncDictionary.ContainsKey(targetType))
			{
				getFuncDictionary[targetType] = new Dictionary<string, Func<object, object>>();
			}

			if (!getFuncDictionary[targetType].ContainsKey(propertyName))
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
				PropertyInfo pi = targetType.GetProperty(propertyName, flags);

				if (pi == null && targetType.BaseType != null)
				{
					return GetProperty(targetType.BaseType, propertyName);
				}
				else
				{
					Type delegateType = Expression.GetFuncType(targetType, pi.PropertyType);

					ParameterExpression pe = Expression.Parameter(typeof(object), "_modelObject");
					// http://neue.cc/2011/04/20_317.html
					var right = Expression.Lambda<Func<object, object>>(
							Expression.Convert(
								Expression.PropertyOrField(
									Expression.Convert(
										 pe
										, targetType)
									, propertyName
								)
								, typeof(object))
							, pe
						);

					// var lambaText = right.ToString(); // _modelObject => Convert(Convert(_modelObject).B)

					// 「(object target) => (object)((T)target).PropertyName」のようなFuncがコンパイルされる
					func = right.Compile();

					getFuncDictionary[targetType][propertyName] = func;

					object result = func(_modelObject);
					return result;
				}
			}
			else
			{
				func = getFuncDictionary[targetType][propertyName];

				object result = func(_modelObject);
				return result;
			}
		}

		private void InConstractor(T entity)
		{
			this._modelType = typeof(T);

			if (ModelInstance is INotifyPropertyChanged)
			{
				// ModelのPropertyChangedイベントをフックする。
				_listener = new WeakEventListener<PropertyChangedEventManager, PropertyChangedEventArgs>(
					(s, e) =>
					{
						OnPropertyChanged(e.PropertyName);
					});
				PropertyChangedEventManager.AddListener((INotifyPropertyChanged)_modelObject, _listener, string.Empty);
			}
		}

		private void SetProperty(Type targetType, string propertyName,object value)
		{
			Action<object, object> action;

			if (!setFuncDictionary.ContainsKey(targetType))
			{
				setFuncDictionary[targetType] = new Dictionary<string, Action<object, object>>();
			}

			if (!setFuncDictionary[targetType].ContainsKey(propertyName))
			{
				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
				PropertyInfo pi = targetType.GetProperty(propertyName, flags);

				if (pi == null && targetType.BaseType != null)
				{
					SetProperty(targetType.BaseType, propertyName, value);
				}
				else
				{

					Type delegateType = Expression.GetFuncType(targetType, pi.PropertyType);

					ParameterExpression modelParameter = Expression.Parameter(typeof(object), "_modelObject");
					ParameterExpression valueParameter = Expression.Parameter(typeof(object), "value");

					var typeAs = Expression.TypeAs(modelParameter, targetType);
					var property = Expression.Property(
								Expression.TypeAs(modelParameter, targetType), pi.GetSetMethod()
							);

					Expression<Action<object, object>> e = Expression.Lambda<Action<object, object>>(
						Expression.Assign(
							Expression.Property(
								Expression.TypeAs(modelParameter, targetType), pi.GetSetMethod()
							),
							Expression.ConvertChecked(valueParameter, pi.PropertyType)
						),
						modelParameter,
						valueParameter
					);

					//(_modelObject,value) => (_modelObject as Model).X = Convert(value)
					//のようなActionがコンパイルされる。
					//Convertはキャストの事。
					action = e.Compile();

					setFuncDictionary[targetType][propertyName] = action;

					action(_modelObject, value);
				}
			}
			else
			{
				action = setFuncDictionary[targetType][propertyName];

				action(_modelObject, value);
			}

			
		}
	}


	public class WeakEventListener<TManager, TEventArgs> : System.Windows.IWeakEventListener
		where TManager : System.Windows.WeakEventManager
		where TEventArgs : EventArgs
	{
		public WeakEventListener(EventHandler<TEventArgs> handler)
		{
			_hander = handler;
		}
		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TManager) && e is TEventArgs)
			{
				_hander(sender, e as TEventArgs);
				return true;
			}
			else
				return false;
		}
		private EventHandler<TEventArgs> _hander;
		~WeakEventListener()
		{

		}
	}
}
