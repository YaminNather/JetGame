﻿#define USE_IL_FOR_GET_AND_SET

//#define USE_THREADING
#define SAFE_MODE

//#define DEBUG_GET_FOR_COMPONENT
//#define DEBUG_GET_FOR_ASSET
//#define DEBUG_GET_FOR_FIELD
//#define DEBUG_GET_FOR_PROPERTY_DRAWER
//#define DEBUG_GET_FOR_DECORATOR_DRAWER

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

#if !NET_STANDARD_2_0
using Sisus.Vexe.FastReflection;
#endif

using Sisus.Compatibility;
using Sisus.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus
{
	/// <summary>
	/// Class that is responsible for determining which drawers should be used for which Unity Object targets and class members.
	/// </summary>
	[Serializable]
	public abstract class DrawerProviderBase : IDrawerProvider
	{
		private static readonly HashSet<Type> decoratorDrawerDrawerBlacklist = new HashSet<Type>() { typeof(PHeaderAttribute), typeof(PRangeAttribute), typeof(PSpaceAttribute) };
		private static bool usingDeserializedDrawers = true;

		private readonly Dictionary<Type, Dictionary<string, Type>> DrawersByInterfaceTypeThenName = new Dictionary<Type, Dictionary<string, Type>>();

		[NotNull]
		public readonly DrawerProviderData drawersFor;

		

		public bool IsReady
		{
			get;
			private set;
		}

		[NotNull]
		public Type GameObjectDrawer
		{
			get
			{
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(drawersFor != null);
				#endif

				if(InspectorUtility.ActiveInspector.Preferences.EnableCategorizedComponents)
				{
					return drawersFor.gameObjectCategorized;
				}
				return drawersFor.gameObject;
			}
		}

		protected virtual HashSet<Type> DecoratorDrawerDrawerBlacklist => decoratorDrawerDrawerBlacklist;

        public DrawerProviderBase()
		{
			#if DEV_MODE
			Debug.Log(GetType().Name + " constructor");
			#endif
			drawersFor = new DrawerProviderData();
			BuildDictionariesThreaded(drawersFor);
			usingDeserializedDrawers = false;
			IsReady = true;
		}

		/// <summary>
		/// Prewarms some commonly used drawersfor smoother user experience when selecting targets with these drawer for the first time.
		/// </summary>
		public void Prewarm(IInspector inspector)
		{
			//if cache already has stuff, skip this step
			if(DrawerPool.Count < 7 && (InspectorUtility.ActiveInspector == null || InspectorUtility.ActiveInspector.State.drawers.Length == 0)) 
			{
				var tempGameObject = new GameObject
				{
					hideFlags = HideFlags.HideAndDontSave
				};
			
				var gameObjectDrawer = GetForGameObject(inspector, tempGameObject, null);

				// make sure that all members are prewarmed even if drawers are folded
				var members = gameObjectDrawer.MembersBuilt;
				for(int n = members.Length - 1; n >= 0; n--)
				{
					var component = members[n] as IComponentDrawer;
					if(component != null)
					{
						var nestedMembers = component.MembersBuilt;
						for(int m = nestedMembers.Length - 1; m >= 0; m--)
						{
							var nestedParent = nestedMembers[m] as IParentDrawer;
							if(nestedParent != null)
							{
								var nestedNestedMembers = nestedParent.MembersBuilt;
							}
						}
					}
				}

				gameObjectDrawer.Dispose();
				Platform.Active.Destroy(tempGameObject);

				#if DEV_MODE
				Debug.Assert(DrawerPool.Count >= 7, "DrawerPool had only "+ DrawerPool.Count+" items after GameObjectDrawer.Prewarm was called.");
				#endif
			}

			var menu = Menu.Create();
			for(int n = 0; n < 50; n++)
			{
				menu.AddDisabled(StringUtils.ToString(n));
			}
			menu.Dispose();
		}

		/// <inheritdoc/>
		[CanBeNull]
		public Type GetDrawerTypeByName([NotNull]string drawerName)
		{
			return GetDrawerTypeByName(drawerName, typeof(IDrawer));
		}

		/// <inheritdoc/>
		[CanBeNull]
		public Type GetComponentDrawerTypeByName([NotNull]string drawerName)
		{
			switch(drawerName)
			{
				case "IComponentDrawer":
					return typeof(IComponentDrawer);
				case "IEditorlessComponentDrawer":
					return typeof(IEditorlessComponentDrawer);
				case "ICustomEditorComponentDrawer":
					return typeof(ICustomEditorComponentDrawer);
				default:
					return GetDrawerTypeByName(drawerName, typeof(IComponentDrawer));
			}
		}

		/// <inheritdoc/>
		[CanBeNull]
		public Type GetFieldDrawerTypeByName([NotNull]string drawerName)
		{
			switch(drawerName)
			{
				case "IFieldDrawer":
					return typeof(IFieldDrawer);
				case "IPropertyDrawerDrawer":
					return typeof(IPropertyDrawerDrawer);
				case "IDecoratorDrawerDrawer":
					return typeof(IDecoratorDrawerDrawer);
				default:
					return GetDrawerTypeByName(drawerName, typeof(IFieldDrawer));
			}
		}

		/// <inheritdoc/>
		[CanBeNull]
		private Type GetDrawerTypeByName([NotNull]string drawerName, Type interfaceType)
		{
			Dictionary<string, Type> drawersByName;
			if(!DrawersByInterfaceTypeThenName.TryGetValue(interfaceType, out drawersByName))
			{
				var drawers = TypeExtensions.GetImplementingNonUnityObjectClassTypes(interfaceType, false);
				int count = drawers.Length;
				drawersByName = new Dictionary<string, Type>(count);

				for(int n = count - 1; n >= 0; n--)
				{
					var drawerType = drawers[n];
					drawersByName[drawerType.Name] = drawerType;
					drawersByName[drawerType.FullName] = drawerType;
				}

				DrawersByInterfaceTypeThenName.Add(interfaceType, drawersByName);
			}

			Type result;
			drawersByName.TryGetValue(drawerName, out result);
			return result;
		}


		/// <inheritdoc/>
		[NotNull]
		public IGameObjectDrawer GetForGameObject([NotNull]IInspector inspector, [CanBeNull]GameObject target, [NotNull]IParentDrawer parent)
		{
			return GetForGameObject(GetDrawerTypeForGameObject(target), inspector, target, parent);
		}

		/// <inheritdoc/>
		[NotNull]
		public IGameObjectDrawer GetForGameObjects([NotNull]IInspector inspector, [NotNullOrEmpty]GameObject[] targets, [NotNull]IParentDrawer parent)
		{
			return GetForGameObjects(GetDrawerTypeForGameObjects(targets), inspector, targets, parent);
		}

		/// <inheritdoc/>
		[NotNull]
		public Type GetDrawerTypeForGameObject([NotNull]GameObject target)
		{
			if(drawersFor.gameObjectByComponent.Count > 0)
			{
				var components = target.GetComponents<Component>();
				for(int n = components.Length - 1; n >= 0; n--)
				{
					var component = components[n];
					Type gameObjectDrawerForComponent;
					if(component != null && drawersFor.gameObjectByComponent.TryGetValue(component.GetType(), out gameObjectDrawerForComponent))
					{
						return gameObjectDrawerForComponent;
					}
				}
			}
			return GameObjectDrawer;
		}

		/// <inheritdoc/>
		[NotNull]
		public Type GetDrawerTypeForGameObjects([NotNullOrEmpty]GameObject[] targets)
		{
			return GetDrawerTypeForGameObject(targets[0]);
		}

		/// <summary> Gets drawer instance for drawing Gameobject data inside an inspector. </summary>
		/// <param name="drawerType"> The type of the drawer to create. This cannot be null and the class must implement IEditorlessComponentDrawer. </param>
		/// <param name="inspector"> The inspector which will contain the created drawer. This cannot be null. </param>
		/// <param name="target"> Target GameObject. This cannot be null. </param>
		/// <param name="parent"> The parent drawer for the created drawer. This can be null. </param>
		/// <returns> Instance of drawer that implements IGameObjectDrawer. This will never be null. </returns>
		[NotNull]
		public IGameObjectDrawer GetForGameObject([NotNull]Type drawerType, [NotNull]IInspector inspector, [CanBeNull]GameObject target, [NotNull]IParentDrawer parent)
		{
			var created = GetOrCreateInstance<IGameObjectDrawer>(drawerType);
			created.Setup(target, parent, inspector);
			created.LateSetup();
			return created;
		}

		/// <summary> Gets drawer instance for drawing Gameobject data inside an inspector. </summary>
		/// <param name="drawerType"> The type of the drawer to create. This cannot be null and the class must implement IEditorlessComponentDrawer. </param>
		/// <param name="inspector"> The inspector which will contain the created drawer. This cannot be null. </param>
		/// <param name="targets"> Target GameObjects. This cannot be null. </param>
		/// <param name="parent"> The parent drawer for the created drawer. This can be null. </param>
		/// <returns> Instance of drawer that implements IGameObjectDrawer. This will never be null. </returns>
		[NotNull]
		public IGameObjectDrawer GetForGameObjects(Type drawerType, [NotNull]IInspector inspector, [CanBeNull]GameObject[] targets, [NotNull]IParentDrawer parent)
		{
			var created = GetOrCreateInstance<IGameObjectDrawer>(drawerType);
			created.Setup(targets, parent, inspector);
			created.LateSetup();
			return created;
		}

		/// <inheritdoc/>
		[NotNull]
		public IComponentDrawer GetForComponent(IInspector inspector, Component target, IParentDrawer parent)
		{
			return GetForComponents(inspector, ArrayPool<Component>.CreateWithContent(target), parent);
		}

		/// <inheritdoc/>
		[NotNull]
		public IComponentDrawer GetForComponents(IInspector inspector, Component[] targets, IParentDrawer parent)
		{
			var firstTarget = targets[0];
			if(firstTarget == null)
			{
				return MissingScriptDrawer.Create(parent, inspector);
			}
			
			Type customEditorType;
			var drawerType = GetDrawerTypeForComponent(firstTarget.GetType(), out customEditorType);
			
			if(typeof(ICustomEditorComponentDrawer).IsAssignableFrom(drawerType))
			{
				return GetForComponents(drawerType, customEditorType, targets, parent, inspector);
			}

			return GetForComponents(drawerType, targets, parent, inspector);
		}

		/// <inheritdoc/>
		[NotNull]
		public IEditorlessComponentDrawer GetForComponents(Type drawerType, Component[] targets, IParentDrawer parent, IInspector inspector)
		{
			var created = GetOrCreateInstance<IEditorlessComponentDrawer>(drawerType);

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(created.GetType() == drawerType);
			#endif

			created.SetupInterface(targets, parent, inspector);
			created.LateSetup();
			return created;
		}

		/// <inheritdoc/>
		[NotNull]
		public ICustomEditorComponentDrawer GetForComponents([NotNull]Type drawerType, [CanBeNull]Type customEditorType, Component[] targets, IParentDrawer parent, IInspector inspector)
		{
			var created = GetOrCreateInstance<ICustomEditorComponentDrawer>(drawerType);
			created.SetupInterface(customEditorType, targets, parent, inspector);
			created.LateSetup();
			return created;
		}

		/// <inheritdoc/>
		[NotNull]
		public IDrawer GetForAsset([NotNull]IInspector inspector, [NotNull]Object target, [CanBeNull]IParentDrawer parent)
		{
			return GetForAssets(inspector, ArrayPool<Object>.CreateWithContent(target), parent);
		}

		/// <inheritdoc/>
		[NotNull]
		public IAssetDrawer GetForAssets([NotNull]IInspector inspector, [NotNull]Object[] targets, [CanBeNull]IParentDrawer parent)
		{
			#if UNITY_EDITOR
			Object[] assetImporters;
			Type customEditorType;
			var drawerType = GetDrawerTypeForAssets(targets, out assetImporters, out customEditorType);
			if(typeof(ICustomEditorAssetDrawer).IsAssignableFrom(drawerType))
			{
				#if DEV_MODE && DEBUG_GET_FOR_ASSET
				Debug.Log("GetForAssetWithEditor("+StringUtils.TypeToString(targets[0])+"): "+drawerType.Name+" with assetImporters="+StringUtils.TypesToString(assetImporters)+", customEditorType=" + StringUtils.ToString(customEditorType));
				#endif
				return GetForAssetWithEditor(drawerType, customEditorType, targets, assetImporters, parent, inspector);
			}
			#else
			var drawerType = GetDrawerTypeForAssets(targets);
			#endif
			
			#if DEV_MODE || SAFE_MODE
			if(!typeof(IEditorlessAssetDrawer).IsAssignableFrom(drawerType))
			{
				#if DEV_MODE
				Debug.LogError("DrawerProvider.GetForAsset returned drawerType "+drawerType.Name+" which did not implement ICustomEditorAssetDrawer nor IEditorlessAssetDrawer");
				#endif
				drawerType = drawersFor.assetDefault;
			}
			#endif

			return GetForAssetsWithoutEditor(drawerType, targets, parent, inspector);
		}


		#if UNITY_EDITOR
		/// <inheritdoc/>
		[NotNull]
		public ICustomEditorAssetDrawer GetForAssetWithEditor([NotNull]Type drawerType, [CanBeNull]Type customEditorType, [NotNull]Object[] targets, [CanBeNull]Object[] assetImporters, [CanBeNull]IParentDrawer parent, [NotNull]IInspector inspector)
		{
			var created = GetOrCreateInstance<ICustomEditorAssetDrawer>(drawerType);
			created.SetupInterface(targets, assetImporters, customEditorType, parent, inspector);
			created.LateSetup();
			return created;
		}
		#endif

		/// <inheritdoc/>
		[NotNull]
		public IEditorlessAssetDrawer GetForAssetsWithoutEditor(Type drawerType, Object[] targets, IParentDrawer parent, IInspector inspector)
		{
			var created = GetOrCreateInstance<IEditorlessAssetDrawer>(drawerType);
			created.SetupInterface(targets, parent, inspector);
			created.LateSetup();
			return created;
		}

		/// <inheritdoc/>
		public bool TryGetForDecoratorDrawer([NotNull]PropertyAttribute fieldAttribute, [NotNull]Type propertyAttributeType, [CanBeNull]IParentDrawer parent, [NotNull]LinkedMemberInfo attributeTarget, [CanBeNull]out IDecoratorDrawerDrawer result)
		{
			var drawerType = GetDrawerTypeForDecoratorDrawerAttribute(propertyAttributeType);

			if(drawerType != null)
			{
				#if DEV_MODE && (DEBUG_GET_FOR_FIELD || DEBUG_GET_FOR_DECORATOR_DRAWER)
				Debug.Log("<color=green>DecoratorDrawer drawer</color> for field " + attributeTarget.Name + " and attribute " + fieldAttribute.GetType().Name + ": " + drawerType.Name);
				#endif

				result = GetOrCreateInstance<IDecoratorDrawerDrawer>(drawerType);
				#if UNITY_EDITOR
				Type decoratorDrawerType;
				if(CustomEditorUtility.TryGetDecoratorDrawerType(propertyAttributeType, out decoratorDrawerType))
				{
					result.SetupInterface(fieldAttribute, decoratorDrawerType, parent, attributeTarget);
					result.LateSetup();
					return true;
				}
				#endif

				if(!result.RequiresDecoratorDrawerType)
				{
					result.SetupInterface(fieldAttribute, null, parent, attributeTarget);
					result.LateSetup();
					return true;
				}

				#if DEV_MODE
				Debug.LogError(result.GetType().Name + " was returned for propertyAttribute "+propertyAttributeType.Name+" but drawer requires a decoratorDrawerType which was not found.");
				#endif
			}

			#if DEV_MODE && (DEBUG_GET_FOR_FIELD || DEBUG_GET_FOR_DECORATOR_DRAWER)
			Debug.Log("<color=green>DecoratorDrawer drawer</color> for field " + attributeTarget.Name + " and attribute " + fieldAttribute.GetType().Name + ": "+StringUtils.Null);
			#endif

			result = null;
			return false;
		}

		/// <inheritdoc/>
		[CanBeNull]
		public IDrawer GetForFields(object[] values, IParentDrawer parent, GUIContent label = null, bool readOnly = false)
		{
			return GetForField(values, values[0].GetType(), null, parent, label, readOnly);
		}

		/// <inheritdoc/>
		[NotNull]
		public IDrawer GetForMethod(LinkedMemberInfo memberInfo, IParentDrawer parent, GUIContent label = null, bool readOnly = false)
		{
			return GetForField(GetDrawerTypeForMethod(memberInfo.MethodInfo), null, DrawerUtility.GetType<object>(memberInfo, null), memberInfo, parent, label, readOnly);
		}

		/// <summary> Gets drawer type for given field type. Does not take into consideration possible property attributes. </summary>
		/// <param name="fieldType"> Type of the field for which we are trying to find the Drawer. </param>
		/// <returns> Type that implements IFieldDrawer. </returns>
		[NotNull]
		protected virtual Type GetDrawerTypeForMethod([NotNull]MethodInfo methodInfo)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(methodInfo != null);
			#endif

			var attributes = methodInfo.GetCustomAttributes(false);
			int attributeCount = attributes.Length;
			if(attributeCount > 0)
			{
				Type drawerType;
				if(UseDrawerAttributeUtility.TryGetCustomDrawerForClassMember(this, methodInfo, out drawerType))
				{
					return drawerType;
				}

				PluginAttributeConverterProvider.ConvertAll(ref attributes);
				for(int n = 0; n < attributeCount; n++)
				{
					var attribute = attributes[n];
					Dictionary<Type, Type> drawersByFieldType;
					if(drawersFor.drawersByAttributeType.TryGetValue(attribute.GetType(), out drawersByFieldType))
					{
						foreach(var drawerForAttribute in drawersByFieldType.Values)
						{
							return drawerForAttribute;
						}
					}
				}
			}

			return typeof(MethodDrawer);
		}

		/// <inheritdoc/>
		[CanBeNull]
		public IDrawer GetForField(LinkedMemberInfo memberInfo, IParentDrawer parent, GUIContent label = null, bool readOnly = false)
		{
			return GetForField(memberInfo.GetValue(0), memberInfo.Type, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		[NotNull]
		public IDrawer GetForField([CanBeNull]object value, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			return GetForField(value, DrawerUtility.GetType(memberInfo, value), memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		[NotNull]
		public IDrawer GetForField([CanBeNull]object value, [NotNull]Type fieldType, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(fieldType != null, "CreateDefaultDrawer - null type!");
			Debug.Assert(memberInfo != null || value != null || fieldType.IsUnityObject() || Nullable.GetUnderlyingType(fieldType) != null, "CreateDefaultDrawer - both fieldInfo and value null!");
			if(memberInfo != null) { Debug.Assert(memberInfo.Type.IsAssignableFrom(fieldType), "CreateDefaultDrawer - memberInfo.Type "+StringUtils.ToString(memberInfo.Type)+" was not assignable from type "+StringUtils.ToString(fieldType) +"!"); }
			#endif

			#if DEV_MODE && DEBUG_GET_FOR_FIELD
			Debug.Log("GetForField("+StringUtils.ToStringSansNamespace(fieldType)+", "+StringUtils.ToString(label)+ ") with value="+StringUtils.ToString(value)+"...");
			#endif

			Type drawerType;

			if(memberInfo != null)
			{
				if(memberInfo.MemberInfo != null && UseDrawerAttributeUtility.TryGetCustomDrawerForClassMember(this, memberInfo.MemberInfo, out drawerType))
				{
					if(typeof(IPropertyDrawerDrawer).IsAssignableFrom(drawerType))
					{
						var definingPropertyAttribute = GetAttributeThatDefinesDrawer(memberInfo, drawerType) as Attribute;
						if(definingPropertyAttribute != null)
						{
							return GetForPropertyDrawer(drawerType, definingPropertyAttribute, value, memberInfo, parent, label, readOnly);
						}
					}
					return GetForField(drawerType, value, fieldType, memberInfo, parent, label, readOnly);
				}

				var attributes = memberInfo.GetAttributes<PropertyAttribute>();
				int attributeCount = attributes.Length;
				if(attributeCount > 0)
				{
					// E.g. List<string> => List<>.
					Type fieldTypeOrGenericTypeDefinition;
					if(fieldType.IsGenericType && !fieldType.IsGenericTypeDefinition)
					{
						fieldTypeOrGenericTypeDefinition = fieldType.GetGenericTypeDefinition();
					}
					else
					{
						fieldTypeOrGenericTypeDefinition = fieldType;
					}

					bool isCollection = memberInfo.IsCollection;

					for(int n = 0; n < attributeCount; n++)
					{
						var fieldAttribute = attributes[n];

						drawerType = GetDrawerTypeForPropertyAttribute(fieldAttribute.GetType(), fieldTypeOrGenericTypeDefinition);
						if(drawerType != null) 
						{
							#if DEV_MODE && (DEBUG_GET_FOR_FIELD || DEBUG_GET_FOR_PROPERTY_DRAWER)
							Debug.Log("<color=green>PropertyDrawer drawer</color> for field "+StringUtils.ToStringSansNamespace(fieldType)+" and attribute "+fieldAttribute.GetType().Name+": "+drawerType.Name);
							#endif

							return GetForPropertyDrawer(drawerType, fieldAttribute, value, memberInfo, parent, label, readOnly);
						}
					}

					for(int n = 0; n < attributeCount; n++)
					{
						var fieldAttribute = attributes[n];
						for(var baseType = fieldType.BaseType; baseType != null; baseType = baseType.BaseType)
						{
							// E.g. List<string> => List<>.
							if(baseType.IsGenericType && !baseType.IsGenericTypeDefinition)
							{
								fieldTypeOrGenericTypeDefinition = baseType.GetGenericTypeDefinition();
							}
							else
							{
								fieldTypeOrGenericTypeDefinition = baseType;
							}

							drawerType = GetDrawerTypeForPropertyAttribute(fieldAttribute.GetType(), fieldTypeOrGenericTypeDefinition);
							if(drawerType != null)
							{
								#if DEV_MODE && (DEBUG_GET_FOR_FIELD || DEBUG_GET_FOR_PROPERTY_DRAWER)
								Debug.Log("<color=green>PropertyDrawer drawer</color> for field "+fieldType.Name+" and attribute "+fieldAttribute.GetType().Name+": "+drawerType.Name);
								#endif

								return GetForPropertyDrawer(drawerType, fieldAttribute, value, memberInfo, parent, label, readOnly);
							}
						}
					}
				}
			}

			drawerType = GetDrawerTypeForFieldWithPropertyDrawer(fieldType, memberInfo);
			if(drawerType != null)
			{
				#if DEV_MODE && (DEBUG_GET_FOR_FIELD || DEBUG_GET_FOR_PROPERTY_DRAWER)
				Debug.Log("<color=green>PropertyDrawer drawer</color> for field "+StringUtils.ToStringSansNamespace(fieldType)+": "+StringUtils.ToStringSansNamespace(drawerType));
				#endif

				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(typeof(IPropertyDrawerDrawer).IsAssignableFrom(drawerType), drawerType.Name + " does not implement IPropertyDrawerDrawer");
				#endif

				return GetForPropertyDrawer(drawerType, null, value, memberInfo, parent, label, readOnly);
			}

			drawerType = GetDrawerTypeForField(fieldType);

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(!drawersFor.propertyDrawerDrawersRequiringSerializedProperty.Contains(drawerType), drawerType);
			#endif

			#if DEV_MODE && DEBUG_GET_FOR_FIELD
			Debug.Log("<color=green>Field drawer</color> for field "+StringUtils.ToStringSansNamespace(fieldType)+": "+StringUtils.ToStringSansNamespace(drawerType)+"\nmemberInfo="+(memberInfo == null ? StringUtils.Null : memberInfo.LinkedMemberType.ToString())+", IsCollection="+(memberInfo != null && memberInfo.IsCollection ? StringUtils.True : StringUtils.False)+", Attributes="+(memberInfo == null ? "n/a" : StringUtils.ToString(memberInfo.GetAttributes<PropertyAttribute>())));
			#endif

			return GetForField(drawerType, value, fieldType, memberInfo, parent, label, readOnly);
		}

		/// <summary>
		/// Tries to find Attribute that has been applied to this LinkedMemberInfo that has the highest level of responsiblilty for determining how the class member should be drawn in the inspector.
		/// <returns> Attribute that targets this LinkedMemberInfo. Null if none were found. </returns>
		[CanBeNull]
		public IUseDrawer GetAttributeThatDefinesDrawer([NotNull]LinkedMemberInfo memberInfo, Type drawerType)
		{
			var attributes = memberInfo.GetAttributes(false, true);
			for(int n = 0, count = attributes.Length; n < count; n++)
			{
				var useDrawer = attributes[n] as IUseDrawer;
				if(useDrawer != null && useDrawer.GetDrawerType(memberInfo.Type, GetClassDrawerType(memberInfo.Type), this) == drawerType)
				{
					return useDrawer;
				}
			}

			attributes = memberInfo.Type.GetCustomAttributes(false);
			PluginAttributeConverterProvider.ConvertAll(ref attributes);
			for(int n = 0, count = attributes.Length; n < count; n++)
			{
				var useDrawer = attributes[n] as IUseDrawer;
				if(useDrawer != null && useDrawer.GetDrawerType(memberInfo.Type, GetClassDrawerType(memberInfo.Type), this) == drawerType)
				{
					return useDrawer;
				}
			}
			return null;
		}

		/// <inheritdoc/>
		[NotNull]
		public IDrawer GetForPropertyDrawer([NotNull]Attribute fieldAttribute, [CanBeNull]object value, [NotNull]Type fieldType, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(fieldAttribute != null);
			Debug.Assert(!fieldType.IsCollection());
			#endif

			// Convert generic types to their generic type definitions.
			// E.g. List<string> to List<>.
			if(fieldType.IsGenericType && !fieldType.IsGenericTypeDefinition)
			{
				#if DEV_MODE
				Debug.Log(StringUtils.ToColorizedString("GetForPropertyDrawer(", label, "): converting generic field type ", fieldType, " to type definition ", fieldType.GetGenericTypeDefinition()));
				#endif

				fieldType = fieldType.GetGenericTypeDefinition();
			}
			
			var drawerType = GetDrawerTypeForPropertyAttribute(fieldAttribute.GetType(), fieldType);
			if(drawerType != null)
			{
				return GetForPropertyDrawer(drawerType, fieldAttribute, value, memberInfo, parent, label, readOnly);
			}

			drawerType = GetDrawerTypeForField(fieldType);
			return GetForField(drawerType, value, fieldType, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		[NotNull]
		public IFieldDrawer GetForField(Type drawerType, [CanBeNull]object value, [NotNull]Type valueType, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			var created = GetOrCreateInstance<IFieldDrawer>(drawerType);

			#if DEV_MODE && PI_ASSERTATIONS
			if(created == null) { Debug.LogError("GetOrCreateInstance " + drawerType.Name + " result was null!"); }
			#endif

			created.SetupInterface(value, valueType, memberInfo, parent, label, GetIsReadOnly(memberInfo, readOnly));
			created.LateSetup();
			return created;
		}

		/// <inheritdoc/>
		[NotNull]
		public IFieldDrawer GetForProperty([NotNull]Type valueType, [NotNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			var drawerType = GetDrawerTypeForProperty(memberInfo.PropertyInfo);
			object value;
			if(!memberInfo.CanRead || drawerType == typeof(PropertyDrawer) || memberInfo.PropertyInfo.GetIndexParameters().Length > 0)
			{
				value = memberInfo.Type.DefaultValue();
			}
			else
			{
				using(var logCatcher = new LogCatcher())
				{
					try
					{
						value = memberInfo.GetValue(0);
						if(logCatcher.HasMessage && logCatcher.LogType != LogType.Log)
						{
							return PropertyDrawer.Create(memberInfo, parent, label, readOnly, logCatcher.Message, logCatcher.LogType);
						}
					}
					catch(Exception e)
					{
						Debug.LogError(memberInfo.Name + " " + e);

						// If an exception is encountered while getting value of property, then convert to using PropertyDrawer
						// so that value getter is only called when user manually clicks it.
						return PropertyDrawer.Create(memberInfo, parent, label, readOnly, e.ToString(), LogType.Exception);
					}
				}
			}
			return GetForField(drawerType, value, valueType, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		[NotNull]
		public IFieldDrawer GetForProperty([CanBeNull]object value, [NotNull]Type valueType, [NotNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			return GetForField(GetDrawerTypeForProperty(memberInfo.PropertyInfo), value, valueType, memberInfo, parent, label, readOnly);
		}

		/// <inheritdoc/>
		[NotNull]
		public IFieldDrawer GetForProperty(Type drawerType, [CanBeNull]object value, [NotNull]Type valueType, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			return GetForField(drawerType, value, valueType, memberInfo, parent, label, readOnly);
		}

		public Type GetDrawerTypeForProperty(PropertyInfo propertyInfo)
		{
			Type drawerType;
			if(UseDrawerAttributeUtility.TryGetCustomDrawerForClassMember(this, propertyInfo, out drawerType))
			{
				return drawerType;
			}

			if(propertyInfo.ShowInspectorViewableAsNormalField())
			{
				return GetDrawerTypeForField(propertyInfo.PropertyType);
			}
			else
			{
				return typeof(PropertyDrawer);
			}
		}

		protected bool GetIsReadOnly([CanBeNull]LinkedMemberInfo memberInfo, bool defaultValue)
		{
			if(memberInfo != null && memberInfo.GetAttribute<ReadOnlyAttribute>() != null)
			{
				return true;
			}
			return defaultValue;
		}

		/// <inheritdoc/>
		[NotNull]
		public IPropertyDrawerDrawer GetForPropertyDrawer(Type drawerType, [CanBeNull]Attribute fieldAttribute, [CanBeNull]object value, [CanBeNull]LinkedMemberInfo memberInfo, [CanBeNull]IParentDrawer parent, [CanBeNull]GUIContent label, bool readOnly)
		{
			var created = GetOrCreateInstance<IPropertyDrawerDrawer>(drawerType);
			created.SetupInterface(fieldAttribute, value, memberInfo, parent, label, GetIsReadOnly(memberInfo, readOnly));
			created.LateSetup();
			return created;
		}

		/// <inheritdoc/>
		[NotNull]
		public T GetOrCreateInstance<T>(Type drawerType) where T : IDrawer
		{
			#if DEV_MODE
			Debug.Assert(typeof(IDrawer).IsAssignableFrom(drawerType));
			Debug.Assert(typeof(T).IsAssignableFrom(drawerType));
			#endif

			IDrawer created;
			if(DrawerPool.TryGet(drawerType, out created))
			{
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(created.GetType() == drawerType);
				#endif

				return (T)created;
			}
			object instance;
			try
			{
				#if !NET_STANDARD_2_0 && USE_IL_FOR_GET_AND_SET
				instance = drawerType.DelegateForCtor().DynamicInvoke();
				#else
				instance = Activator.CreateInstance(drawerType);
				#endif
			}
			catch(MissingMethodException e)
			{
				Debug.LogException(e);
				instance = drawerType.DefaultValue();
			}

			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(instance.GetType() == drawerType);
			#endif

			return (T)instance;
		}

		/// <inheritdoc/>
		[NotNull]
		public Type GetClassDrawerType([NotNull]Type classType)
		{
			if(classType.IsComponent())
			{
				Type ignoredEditorType;
				return GetDrawerTypeForComponent(classType, out ignoredEditorType);
			}

			if(classType.IsUnityObject())
			{
				return GetDrawerTypeForAsset(classType, "");
			}

			return GetClassMemberDrawerType(classType);
		}

		/// <inheritdoc/>
		[NotNull]
		public Type GetClassMemberDrawerType([NotNull]Type fieldType)
		{
			return GetDrawerTypeForField(fieldType);
		}

		/// <inheritdoc/>
		public void Cleanup(Object[] inspected)
		{
			#if DEV_MODE
			Debug.Log(GetType().Name + ".Cleanup("+inspected.Length+") called with usingDeserializedDrawers="+usingDeserializedDrawers);
			#endif

			if(!usingDeserializedDrawers)
			{
				return;
			}

			if(inspected.Length == 0)
			{
				return;
			}

			var target = inspected[0];
			if(target == null)
			{
				return;;
			}

			var gameObject = target as GameObject;
			if(gameObject == null)
			{
				Type editorType;
				var type = target.GetType();

				var drawer = GetDrawerTypeForAsset(target);
				if(drawer == null)
				{
					if(drawersFor.assets.ContainsKey(type))
					{
						if(CustomEditorUtility.TryGetCustomEditorType(type, out editorType))
						{
							#if DEV_MODE
							Debug.Log("Replaced null drawer for "+type+" with CustomEditorAssetDrawer");
							#endif
							drawersFor.assets[type] = typeof(CustomEditorAssetDrawer);
						}
						else
						{
							#if DEV_MODE
							Debug.Log("Replaced null drawer for "+type+" with AssetDrawer");
							#endif
							drawersFor.assets[type] = typeof(AssetDrawer);
						}
						return;
					}

					foreach(var drawerByExtension in drawersFor.assetsByExtension)
					{
						if(drawerByExtension.Value == drawer)
						{
							if(CustomEditorUtility.TryGetCustomEditorType(type, out editorType))
							{
								#if DEV_MODE
								Debug.Log("Replaced drawer "+drawer.Name + " for extension \""+drawerByExtension.Key+"\" and type "+type.Name + " with CustomEditorAssetDrawer.");
								#endif
								drawersFor.assetsByExtension[drawerByExtension.Key] = typeof(CustomEditorAssetDrawer);
							}
							else
							{
								#if DEV_MODE
								Debug.Log("Replaced drawer "+drawer.Name + " for extension \""+drawerByExtension.Key+"\" and type "+type.Name + " with AssetDrawer.");
								#endif
								drawersFor.assetsByExtension[drawerByExtension.Key] = typeof(AssetDrawer);
							}
							return;
						}
					}
					return;
				}

				if(typeof(ICustomEditorAssetDrawer).IsAssignableFrom(drawer))
				{
					if(!CustomEditorUtility.TryGetCustomEditorType(type, out editorType))
					{
						#if DEV_MODE
						Debug.Log("Replaced drawer "+drawer.Name + " for "+type.Name + " with AssetDrawer");
						#endif
						drawersFor.assets[type] = typeof(AssetDrawer);
					}
					return;
				}

				if(CustomEditorUtility.TryGetCustomEditorType(type, out editorType))
				{
					#if DEV_MODE
					Debug.Log("Replaced drawer "+drawer.Name + " for "+type.Name + " with CustomEditorAssetDrawer");
					#endif
					drawersFor.assets[type] = typeof(CustomEditorAssetDrawer);
				}
				return;
			}

			var components = gameObject.GetComponents(typeof(Component));
			foreach(var component in components)
			{
				if(component == null || component is Transform)
				{
					continue;
				}

				var type = component.GetType();
				Type editorType;

				var drawer = GetDrawerTypeForComponent(type, out editorType);
				if(drawer == null)
				{
					if(CustomEditorUtility.TryGetCustomEditorType(type, out editorType))
					{
						#if DEV_MODE
						Debug.Log("Replaced null drawer for "+type.Name + " with CustomEditorComponentDrawer");
						#endif
						drawersFor.components[type] = typeof(CustomEditorComponentDrawer);
					}
					else
					{
						#if DEV_MODE
						Debug.Log("Replaced null drawer for "+type.Name + " with ComponentDrawer");
						#endif
						drawersFor.components[type] = typeof(ComponentDrawer);
					}
					continue;
				}

				if(typeof(ICustomEditorComponentDrawer).IsAssignableFrom(drawer))
				{
					if(!CustomEditorUtility.TryGetCustomEditorType(type, out editorType))
					{
						#if DEV_MODE
						Debug.Log("Replaced drawer "+drawer.Name+" for "+type.Name + " with ComponentDrawer");
						#endif
						drawersFor.components[type] = typeof(ComponentDrawer);
					}
					continue;
				}

				if(CustomEditorUtility.TryGetCustomEditorType(type, out editorType))
				{
					#if DEV_MODE
					Debug.Log("Replaced drawer "+drawer.Name+" for "+type.Name + " with CustomEditorComponentDrawer");
					#endif
					drawersFor.components[type] = typeof(CustomEditorComponentDrawer);
				}
			}
		}

		/// <summary> Gets Drawer type for given Component type. </summary>
		/// <param name="componentType"> Type of the Component for which we are trying to find the Drawer. </param>
		/// <param name="customEditorType"> Type of the CustomEditor that should be used for the Component. </param>
		/// <returns> Type that implements IComponentDrawer. </returns>
		[NotNull]
		protected virtual Type GetDrawerTypeForComponent([NotNull]Type componentType, out Type customEditorType)
		{
			Type drawerType;
			if(UseDrawerAttributeUtility.TryGetCustomDrawerForClass(this, componentType, out drawerType))
			{
				#if DEV_MODE && DEBUG_GET_FOR_COMPONENT
				Debug.Log("GetDrawerTypeForComponent(" + StringUtils.ToString(componentType) + "): " + StringUtils.ToString(drawerType));
				#endif

				customEditorType = null;
				return drawerType;
			}
			
			if(drawersFor.components.TryGetValue(componentType, out drawerType))
			{
				#if DEV_MODE && DEBUG_GET_FOR_COMPONENT
				Debug.Log("GetDrawerTypeForComponent(" + StringUtils.ToString(componentType) + "): " + StringUtils.ToString(drawerType));
				#endif

				customEditorType = null;
				return drawerType;
			}

			#if UNITY_EDITOR
			if(Platform.EditorMode)
			{
				if(PluginCompatibilityUtility.UseCompatibilityModeForDisplayingTarget(componentType))
				{
					#if DEV_MODE && DEBUG_GET_FOR_COMPONENT
					Debug.Log("GetDrawerTypeForComponent(" + StringUtils.ToString(componentType)+ "): CustomEditorComponentDrawer because UseCompatibilityModeForDisplayingTarget returned true.");
					#endif

					customEditorType = null;
					return drawersFor.customEditorComponentDefault;
				}

				if(CustomEditorUtility.TryGetCustomEditorType(componentType, out customEditorType))
				{
					#if DEV_MODE && DEBUG_GET_FOR_COMPONENT
					Debug.Log("GetDrawerTypeForComponent(" + StringUtils.ToString(componentType)+ "): CustomEditorComponentDrawer because TryGetCustomEditorType returned true.");
					#endif

					return drawersFor.customEditorComponentDefault;
				}
			}
			#endif

			// support editors targeting generic classes
			if(componentType.IsGenericType && !componentType.IsGenericTypeDefinition)
			{
				#if DEV_MODE && DEBUG_GET_FOR_COMPONENT
				Debug.Log("GetDrawerTypeForComponent(" + StringUtils.ToString(componentType)+ "): fetching for type definition "+StringUtils.ToString(componentType.GetGenericTypeDefinition())+"...");
				#endif
				return GetDrawerTypeForComponent(componentType.GetGenericTypeDefinition(), out customEditorType);
			}

			#if DEV_MODE && DEBUG_GET_FOR_COMPONENT
			Debug.Log("GetDrawerTypeForComponent(" + StringUtils.ToString(componentType)+ "): returning fallback drawer ComponentDrawer");
			#endif

			customEditorType = null;
			return drawersFor.componentDefault;
		}

		#if UNITY_EDITOR
		[NotNull]
		protected virtual Type GetDrawerTypeForAssets([NotNull]Object[] assets, [CanBeNull]out Object[] assetImporters, [CanBeNull]out Type customEditorType)
		{
			assetImporters = null;
			customEditorType = null;

			// handle mismatching types
			if(!assets.AllSameType())
			{
				return typeof(MultipleAssetDrawer);
			}
			
			if(AssetImporters.TryGet(assets, ref assetImporters))
			{
				var assetImporterType = assetImporters[0].GetType();
				if(!CustomEditorUtility.TryGetCustomEditorType(assetImporterType, out customEditorType))
				{
					#if DEV_MODE
					Debug.LogWarning("GetDrawerTypeForAssets("+assets[0].GetType().Name+") Setting assetImporters of type "+ assetImporterType.Name+" to null because could not find CustomEditor type for them.");
					#endif

					assetImporters = null;
					CustomEditorUtility.TryGetCustomEditorType(assets[0].GetType(), out customEditorType);
				}
			}
			else
			{
				CustomEditorUtility.TryGetCustomEditorType(assets[0].GetType(), out customEditorType);
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(assetImporters == null);
				#endif
			}

			return GetDrawerTypeForAsset(assets[0]);
		}
		#else
		[NotNull]
		protected virtual Type GetDrawerTypeForAssets([NotNull]Object[] assets)
		{
			return GetDrawerTypeForAsset(assets[0]);
		}
		#endif

		[NotNull]
		protected virtual Type GetDrawerTypeForAsset([NotNull]Object asset)
		{
			#if UNITY_EDITOR
			#if DEV_MODE && DEBUG_GET_FOR_ASSET
			if(!AssetDatabase.IsMainAsset(asset)) { Debug.Log(asset.GetType().Name + " is not main asset @ "+AssetDatabase.GetAssetPath(asset));}
			#endif
			// Only provide local path if subject is main asset at path. This avoid problem where wrong drawer is returned based on file extension of main asset.
			string localPath = AssetDatabase.IsMainAsset(asset) ? AssetDatabase.GetAssetPath(asset) : "";
			return GetDrawerTypeForAsset(asset.GetType(), localPath);
			#else
			return GetDrawerTypeForAsset(asset.GetType(), "");
			#endif
		}

		/// <summary>
		/// Gets Drawer type for given asset type that is not a directory.
		/// </summary>
		/// <param name="assetType">
		/// Type of the UnityEngine.Object-derived asset for which we are trying to find the Drawer.
		/// This should NOT be directory.
		/// </param>
		/// <param name="localPath">
		/// The path to the target asset relative to the application data path.
		/// Empty if target is not an asset saved on the disk (e.g. ScriptableObject instance),
		/// or if target is not the main asset at its path.
		/// </param>
		/// <returns> Type that implements IAssetDrawer. </returns>
		[NotNull]
		protected virtual Type GetDrawerTypeForAsset([NotNull]Type assetType, [NotNull]string localPath)
		{
			Type drawerType;

			if(UseDrawerAttributeUtility.TryGetCustomDrawerForClass(this, assetType, out drawerType))
			{
				#if DEV_MODE && DEBUG_GET_FOR_COMPONENT
				Debug.Log("GetDrawerTypeForAsset(" + StringUtils.ToString(assetType) + "): " + StringUtils.ToString(drawerType));
				#endif

				assetType = null;
				return drawerType;
			}

			if(localPath.Length > 0)
			{
				#if UNITY_EDITOR
				string fullPath = FileUtility.LocalToFullPath(localPath);
				if(Directory.Exists(fullPath))
				{
					return typeof(FolderDrawer);
				}
				#endif

				string fileExtension = Path.GetExtension(localPath).ToLowerInvariant();
				if(fileExtension.Length > 0)
				{
					if(drawersFor.assetsByExtension.TryGetValue(fileExtension, out drawerType))
					{
						#if DEV_MODE && DEBUG_GET_FOR_ASSET
						Debug.Log("GetDrawerTypeForAsset(" + StringUtils.ToString(assetType)+ "): "+ drawerType.Name+ " via extension \"" + fileExtension+"\" with localPath=\"" + localPath + "\"");
						#endif

						return drawerType;
					}
				}
			}

			if(drawersFor.assets.TryGetValue(assetType, out drawerType))
			{
				#if DEV_MODE && DEBUG_GET_FOR_ASSET
				Debug.Log("GetDrawerTypeForAsset(" + StringUtils.ToString(assetType)+ "): "+ drawerType.Name+" via asset type with localPath=\"" + localPath + "\"");
				#endif

				return drawerType;
			}
			
			#if UNITY_EDITOR
			// ScriptableObjects use AssetDrawers unless using Compatibility mode, in which case always use CustomEditorAssetDrawer
			if(assetType.IsScriptableObject() && !PluginCompatibilityUtility.UseCompatibilityModeForDisplayingTarget(assetType))
			{
				#if DEV_MODE && DEBUG_GET_FOR_ASSET
				Debug.Log("GetDrawerTypeForAsset(" + StringUtils.ToString(assetType)+ "): AssetDrawer as fallback with localPath=\"" + localPath + "\"");
				#endif

				return drawersFor.assetDefault;
			}

			// support editors targeting generic classes
			if(assetType.IsGenericType && !assetType.IsGenericTypeDefinition)
			{
				return GetDrawerTypeForAsset(assetType.GetGenericTypeDefinition(), localPath);
			}

			#if DEV_MODE && DEBUG_GET_FOR_ASSET
			Debug.Log("GetDrawerTypeForAsset(" + StringUtils.ToString(assetType)+ "): CustomEditorAssetDrawer as fallback with localPath=\"" + localPath + "\"");
			#endif

			return drawersFor.customEditorAssetDefault;
			#else
			return drawersFor.assetDefault;
			#endif
		}

		/// <summary> Gets drawer type for given field type. Does not take into consideration possible property attributes. </summary>
		/// <param name="fieldType"> Type of the field for which we are trying to find the Drawer. </param>
		/// <returns> Type that implements IFieldDrawer. </returns>
		[NotNull]
		protected virtual Type GetDrawerTypeForField(Type fieldType)
		{
			Type drawerType;

			if(fieldType.IsArray)
			{
				switch(fieldType.GetArrayRank())
				{
					case 1:
						break;
					case 2:
						return typeof(Array2DDrawer);
					case 3:
						return typeof(Array3DDrawer);
					default:
						#if DEV_MODE
						Debug.LogWarning("Arrays with rank "+fieldType.GetArrayRank()+" are not supported by Power Inspector.");
						#endif
						return typeof(DataSetDrawer);
				}
				return typeof(ArrayDrawer);
			}

			// Drawer for generic types are fetched using their generic type definition.
			// E.g. List<T> instead of List<string>.
			Type fieldTypeOrGenericTypeDefinition;
			if(fieldType.IsGenericType && !fieldType.IsGenericTypeDefinition)
			{
				fieldTypeOrGenericTypeDefinition = fieldType.GetGenericTypeDefinition();
			}
			else
			{
				fieldTypeOrGenericTypeDefinition = fieldType;
			}

			var fieldDrawers = drawersFor.fields;

			if(fieldDrawers.TryGetValue(fieldTypeOrGenericTypeDefinition, out drawerType))
			{
				if(drawerType.IsGenericTypeDefinition)
				{
					return MakeGenericDrawerType(drawerType, fieldType);
				}

				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(!drawersFor.propertyDrawerDrawersRequiringSerializedProperty.Contains(drawerType), drawerType);
				#endif

				return drawerType.IsGenericTypeDefinition ? MakeGenericDrawerType(drawerType, fieldType) : drawerType;
			}

			if(fieldType.IsAbstract)
			{
				return typeof(AbstractDrawer);
			}
			
			return typeof(DataSetDrawer);
		}

		private Type MakeGenericDrawerType(Type drawerGenericTypeDefinition, Type targetType)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			if(!drawerGenericTypeDefinition.IsGenericTypeDefinition)
			{
				Debug.LogError(StringUtils.ToStringSansNamespace(drawerGenericTypeDefinition));
			}
			else if(drawerGenericTypeDefinition.GetGenericArguments().Length != 1)
			{
				Debug.LogError(StringUtils.ToStringSansNamespace(drawerGenericTypeDefinition)+".GetGenericArguments: "+StringUtils.ToString(drawerGenericTypeDefinition.GetGenericArguments()));
			}
			if(targetType.IsGenericParameter)
			{
				Debug.LogError(StringUtils.ToStringSansNamespace(targetType));
			}
			#endif

			var genericTypeArgument = targetType.GetGenericArgumentOrInheritedGenericArgument();
			if(genericTypeArgument != null)
			{
				var genericDrawerType = drawerGenericTypeDefinition.MakeGenericType(genericTypeArgument);
				#if DEV_MODE
				Debug.Log("Returning generic drawers "+StringUtils.ToStringSansNamespace(genericDrawerType)+" for fieldType "+StringUtils.ToStringSansNamespace(targetType));
				#endif
				return genericDrawerType;
			}
			#if DEV_MODE
			Debug.LogError("Drawer "+StringUtils.ToStringSansNamespace(drawerGenericTypeDefinition)+" was generic type definition, but could not get generic type argument from fieldType "+StringUtils.ToStringSansNamespace(targetType));
			#endif
			return typeof(DataSetDrawer);
		}

		/// <summary>
		/// Gets Drawer type for field with a PropertyAttribute that is backed by a PropertyDrawer.
		/// Returns null if PropertyAttribute is not backed by a PropertyDrawer (e.g. if has a DecoratorDrawer).
		/// </summary>
		/// <param name="attributeType"> Type of the PropertyAttribute on the field. </param>
		/// <param name="fieldType"> Type of the field. </param>
		/// <returns> Drawer type for representing field with PropertyAttribute. </returns>
		[CanBeNull]
		protected virtual Type GetDrawerTypeForPropertyAttribute([NotNull]Type attributeType, [NotNull]Type fieldType)
		{
			// Attributes of arrays are passed on to members instead of affecting the collections themselves
			if(fieldType.IsArray)
			{
				#if DEV_MODE
				Debug.LogWarning("GetDrawerTypeForPropertyAttribute("+ StringUtils.ToStringSansNamespace(attributeType)+", "+StringUtils.ToStringSansNamespace(fieldType) + ") - returning null because Attributes of arrays are passed on to members instead of affecting the collections themselves.");
				#endif
				return null;
			}
			
			if(fieldType.IsGenericTypeDefinition)
			{
				// Attributes of lists and hash sets are also passed on to members instead of affecting the collections themselves
				if(fieldType == Types.List || fieldType == Types.HashSet)
				{
					#if DEV_MODE
					Debug.LogWarning("GetDrawerTypeForPropertyAttribute(" + StringUtils.ToStringSansNamespace(attributeType) + ", " + StringUtils.ToStringSansNamespace(fieldType) + ") - returning null because Attributes of arrays are passed on to members instead of affecting the collections themselves.");
					#endif
					return null;
				}
			}
			else if(fieldType.IsGenericType)
			{
				var baseType = fieldType.GetGenericTypeDefinition();

				// Attributes of lists and hash sets are also passed on to members instead of affecting the collections themselves
				if(baseType == Types.List || baseType == Types.HashSet)
				{
					#if DEV_MODE
					Debug.LogWarning("GetDrawerTypeForPropertyAttribute(" + StringUtils.ToStringSansNamespace(attributeType) + ", " + StringUtils.ToStringSansNamespace(fieldType) + ") - returning null because Attributes of arrays are passed on to members instead of affecting the collections themselves.");
					#endif
					return null;
				}
			}

			Dictionary<Type, Type> drawerTypesByFieldType;
			if(drawersFor.drawersByAttributeType.TryGetValue(attributeType, out drawerTypesByFieldType))
			{
				#if DEV_MODE && DEBUG_GET_FOR_PROPERTY_DRAWER
				Debug.Log("GetDrawerTypeForPropertyAttribute found "+ drawerTypesByFieldType.Count+" options for field " + fieldType.Name+" and attribute "+ attributeType.Name+": "+StringUtils.ToString(drawerTypesByFieldType));
				#endif
				for(var type = fieldType; type != null; type = type.BaseType)
				{
					Type drawerType;
					if(drawerTypesByFieldType.TryGetValue(type, out drawerType))
					{
						#if DEV_MODE && DEBUG_GET_FOR_PROPERTY_DRAWER
						Debug.Log("GetDrawerTypeForPropertyAttribute for field " + fieldType.Name+" and attribute "+ attributeType.Name+" via base type "+ type.Name+" : "+drawerType.Name);
						#endif
						return drawerType;
					}
				}
				#if DEV_MODE && DEBUG_GET_FOR_PROPERTY_DRAWER
				Debug.LogWarning("Failed to get attribute-based drawers for fieldType "+fieldType+" with attributeType "+attributeType);
				#endif
			}

			// support PropertyDrawers targeting generic attributes
			if(attributeType.IsGenericType && !attributeType.IsGenericTypeDefinition)
			{
				return GetDrawerTypeForPropertyAttribute(attributeType.GetGenericTypeDefinition(), fieldType);
			}

			// support PropertyDrawers targeting generic field types
			if(fieldType.IsGenericType && !fieldType.IsGenericTypeDefinition)
			{
				return GetDrawerTypeForPropertyAttribute(attributeType, fieldType.GetGenericTypeDefinition());
			}

			return null;
		}
		
		/// <summary>
		/// Returns drawer type for field that is backed by a PropertyDrawer, or null if given field is not backed by a PropertyDrawer.
		/// </summary>
		/// <param name="fieldType"> Type of the class member. </param>
		/// <param name="memberInfo"> LinkedMemberInfo for the class member, or null if drawers are not backed by a field or property. </param>
		/// <returns> Drawer type for representing field of given type. </returns>
		[CanBeNull]
		protected virtual Type GetDrawerTypeForFieldWithPropertyDrawer(Type fieldType, [CanBeNull]LinkedMemberInfo memberInfo)
		{
			Type drawerType;
			if(drawersFor.propertyDrawerDrawersByFieldType.TryGetValue(fieldType, out drawerType))
			{
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(typeof(IPropertyDrawerDrawer).IsAssignableFrom(drawerType), drawerType.Name + " does not implement IPropertyDrawerDrawer. fieldType="+ fieldType.Name+", memberInfo="+(memberInfo == null ? "null" : memberInfo.ToString()));
				#endif

				#if UNITY_EDITOR
				if(drawersFor.propertyDrawerDrawersRequiringSerializedProperty.Contains(drawerType))
				{
					if(memberInfo == null || memberInfo.SerializedProperty == null)
					{
						#if DEV_MODE
						if(memberInfo != null) { Debug.LogWarning("IGNORING GetDrawerTypeForPropertyAttribute for field " + fieldType.Name+": "+drawerType.Name+", because requires a SerializedProperty. memberInfo="+memberInfo+", SerializedProperty="+(memberInfo.SerializedProperty == null ? "null" : memberInfo.SerializedProperty.propertyPath)); }
						else { Debug.LogWarning("IGNORING GetDrawerTypeForPropertyAttribute for field " + fieldType.Name+": "+drawerType.Name+", because requires a SerializedProperty. memberInfo="+StringUtils.Null); }
						#endif

						// Just return null?
						// Or should we return some Drawer specifically designed to handle these cases in a safe way?
						return null;
					}
				}
				#endif

				#if DEV_MODE && DEBUG_GET_FOR_PROPERTY_DRAWER
				Debug.Log("GetDrawerTypeForPropertyAttribute for field " + fieldType.Name+": "+drawerType.Name);
				#endif
				return drawerType;
			}
			return null;
		}

		/// <summary>
		/// Gets Drawer type for PropertyAttribute that is backed by a DecoratorDrawer.
		/// Returns null if PropertyAttribute is not backed by a DecoratorDrawer (e.g. if has a PropertyDrawer).
		/// </summary>
		/// <param name="attributeType"> Type of the PropertyAttribute on the field. </param>
		/// <returns> Drawer type for decorator attribute. </returns>
		[CanBeNull]
		protected virtual Type GetDrawerTypeForDecoratorDrawerAttribute([NotNull]Type attributeType)
		{
			Type drawerType;
			if(drawersFor.decoratorDrawers.TryGetValue(attributeType, out drawerType))
			{
				return drawerType;
			}
			return null;
		}
		
		protected abstract void BuildDictionariesThreaded(DrawerProviderData drawerProviderData);
	}
}