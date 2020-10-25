using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus
{
	public sealed class CollectionResizerData : MemberData
	{
		private const int CollectionIndexValue = -2;

		private GetSize getSize;
		private SetSize setSize;
		private Type type;
		
		#if UNITY_EDITOR
		private SerializedProperty serializedProperty;
		#endif

		#if UNITY_EDITOR
		public override SerializedProperty SerializedProperty
		{
			get => serializedProperty;

            set => serializedProperty = value;
        }
		#endif

		public override MulticastDelegate GetDelegate => getSize;

        public override MulticastDelegate SetDelegate => setSize;

        public override int CollectionIndex => CollectionIndexValue;

        public override MemberTypes MemberType => MemberTypes.Method;

        public override LinkedMemberType LinkedMemberType => LinkedMemberType.CollectionResizer;

        public override string Name => getSize != null ? StringUtils.ToString(getSize) : setSize != null ? StringUtils.ToString(setSize) : "";

        /// <inheritdoc />
		public override bool IsStatic =>
            // Even if getSize / setSize actually refer to static methods,
            // we want want to treat the CollectionResizerData as non-static,
            // so that GetValue is called with fieldOwner value containing
            // the value of the collection.
            false;

        public override bool Equals(MemberData other)
		{
			return false;
		}

		public override MemberInfo MemberInfo => getSize != null ? getSize.Method : null;

        public override MemberInfo SecondMemberInfo => setSize != null ? setSize.Method : null;

        public override Type Type => type;

        public override bool CanRead => getSize != null;

        public override bool CanReadWithoutSideEffects => getSize != null; //For now will assume no side effects. Could allow specifying this via parameter later if needed

        public override bool CanWrite => setSize != null;

        public void Setup([NotNull]Type inType, [NotNull]GetSize inGetSize, SetSize inSetSize)
		{
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(inType != null);
			Debug.Assert(inGetSize != null);
			#endif

			type = inType;
			getSize = inGetSize;
			setSize = inSetSize;
		}
			
		public override void GetValue(object fieldOwner, out object result)
		{
			try
			{
				result = getSize(fieldOwner);
			}
			catch(Exception e)
			{
				Debug.LogError(GetType().Name + ".GetValue(" + StringUtils.ToString(fieldOwner) + ") with getSize="+StringUtils.ToString(getSize)+"\n"+e);
				result = Type.DefaultValue();
			}
		}
			
		public override void SetValue(ref object fieldOwner, object value)
		{
			try
			{
				setSize(ref fieldOwner, value);
			}
			catch(Exception e)
			{
				Debug.LogError(GetType().Name + ".SetValue("+ StringUtils.ToString(fieldOwner) + ", "+StringUtils.ToString(value)+") with setSize="+StringUtils.ToString(getSize)+"\n" + e);
			}
		}
			
		public override object[] GetAttributes(bool inherit = true)
		{
			return ArrayPool<object>.ZeroSizeArray;
		}

		#if UNITY_EDITOR
		public override SerializedProperty TryBuildSerializedProperty(SerializedObject serializedObject, SerializedProperty parentProperty)
		{
			return parentProperty == null ? null : parentProperty.FindPropertyRelative("Array.size");
		}
		#endif

		/// <inheritdoc/>
		public override void Dispose()
		{
			#if UNITY_EDITOR
			serializedProperty = null;
			#endif
			LinkedMemberInfoPool.Dispose(this);
		}
	}
}