using System;
using System.Reflection;

namespace NSubstitute.Specs.Infrastructure
{
    public class TemporaryChange<T> : ITemporaryChange
    {
        public string MemberName { get; private set; }
        public T TemporaryValue { get; set; }
        public T OriginalValue { get; private set; }
        public Action<T> SetValue { get; set; }
        public bool IsConfigured { get; set; }

        public TemporaryChange(MemberInfo member, T originalValue)
        {
            MemberName = member.Name;
            SetValue = GetMemberSetter(member);
            OriginalValue = originalValue;            
        }

        private Action<T> GetMemberSetter(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                var field = member.DeclaringType.GetField(member.Name);
                return newValue => field.SetValue(member.DeclaringType, newValue);
            }

            if (member.MemberType == MemberTypes.Property)
            {
                var property = member.DeclaringType.GetProperty(member.Name);
                return newValue => property.SetValue(member.DeclaringType, newValue, new object[0]);
            }
            throw new TemporaryChangeNotConfiguredProperlyException("Could not set " + member.MemberType + " " + member.Name);
        }

        public void SetNewValue()
        {
            SetValue(TemporaryValue);
        }

        public void RestoreOriginalValue()
        {
            SetValue(OriginalValue);
        }
    }
}