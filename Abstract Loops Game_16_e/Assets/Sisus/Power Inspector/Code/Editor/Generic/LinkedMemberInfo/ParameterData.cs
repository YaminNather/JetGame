using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sisus
{
	public sealed class ParameterData : MemberData
	{
		private ParameterInfo parameterInfo;

		#if UNITY_EDITOR
		public override SerializedProperty SerializedProperty
		{
			get => null;

            set => throw new NotSupportedException();
        }
		#endif

		public override string Name => parameterInfo.Name;

        public override MemberTypes MemberType => MemberTypes.Custom;

        public override LinkedMemberType LinkedMemberType => LinkedMemberType.Parameter;

        public override MemberInfo MemberInfo => null;

        public override MemberInfo SecondMemberInfo => null;

        public override ICustomAttributeProvider AttributeProvider => parameterInfo;

        public override bool IsStatic =>
            //new test: true, because parent values are not actually needed for fetching value?
            true;

        public override Type Type => parameterInfo.ParameterType;

        public override bool CanRead => true;

        public override bool CanReadWithoutSideEffects => true;

        public override bool CanWrite => true;

        public void Setup(ParameterInfo setParameterInfo)
		{
			parameterInfo = setParameterInfo;
		}

		public override bool Equals(MemberData other)
		{
			var b = other as ParameterData;
			return b != null && parameterInfo.EqualTo(b.parameterInfo);
		}

		public override void GetValue(object fieldOwner, out object result)
		{
			result = ParameterValues.GetValue(parameterInfo);
		}
			
		public override void SetValue(ref object fieldOwner, object value)
		{
			ParameterValues.CacheValue(parameterInfo, value);
		}
			
		public override object[] GetAttributes(bool inherit = true)
		{
			return parameterInfo.GetCustomAttributes(inherit);
		}

		public override object DefaultValue()
		{
			var result = parameterInfo.DefaultValue;

			//if parameter is optional (!DBNull)
			//and an optional parameter is supplied (!Missing)
			if(result != DBNull.Value && result != Type.Missing)
			{
				return result;
			}

			//otherwise use Default value derived from type
			return Type.DefaultValue();
		}

		/// <inheritdoc/>
		public override void Dispose()
		{
			LinkedMemberInfoPool.Dispose(this);
		}
	}
}