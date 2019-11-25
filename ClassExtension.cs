using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mapper
{
    public static class ClassExtension
    {
        public static void CopyPropertiesToWithException<TSource, TProperty>(this TSource fromRecordBO, object toRecordDTO, Expression<Func<TSource, TProperty>> excludeProperty) where TSource : class
        {
            if (fromRecordBO == null || toRecordDTO == null)
                return;

            List<PropertyInfo> fromPI = fromRecordBO.GetType().GetProperties().ToList();
            if (excludeProperty != null)
            {
                var member = excludeProperty.Body as MemberExpression;
                var property = member.Member as PropertyInfo;
                fromPI = ExcludePropertiesToBeSetted(fromRecordBO, property);
            }

            SetProperties(fromRecordBO, toRecordDTO, fromPI);
        }

        private static void SetProperties(object fromRecordBO, object toRecordDTO, List<PropertyInfo> fromPI)
        {
            foreach (PropertyInfo fromField in fromPI)
            {
                PropertyInfo[] ToPI = toRecordDTO.GetType().GetProperties();

                foreach (PropertyInfo toField in ToPI)
                {
                    if (fromField.Name == toField.Name && toField.CanWrite)
                    {
                        object fromFieldValue = fromField.GetValue(fromRecordBO, null);

                        if (FieldCanBeSetted(fromField, toField, fromRecordBO))
                        {
                            toField.SetValue(toRecordDTO, ReflectionUtil.ChangeType(fromFieldValue, toField.PropertyType), null);
                        }
                        break;
                    }
                }
            }
        }


        private static bool FieldCanBeSetted(PropertyInfo fromField, PropertyInfo toField, object fromRecordBO)
        {
            object fromFieldValue = fromField.GetValue(fromRecordBO, null);

            return ReflectionUtil.IsConversaoPossivel(fromField, toField, fromFieldValue) ||
                            (fromField.Name.ToLower() == "codigo" &&
                            (fromRecordBO.GetType().Name.ToLower().IndexOf("basededados") != -1 ||
                            fromRecordBO.GetType().Name.ToLower().IndexOf("projeto") != -1));
        }

        private static List<PropertyInfo> ExcludePropertiesToBeSetted(object fromRecordBO, PropertyInfo excludeProperty)
        {
            List<PropertyInfo> fromPI = fromRecordBO.GetType().GetProperties().ToList();
            if (excludeProperty != null)
            {
                if (excludeProperty != null)
                {
                    fromPI.RemoveAll(info => info.Name.ToLower() == excludeProp.Name.ToLower());
                }
            }

            return fromPI;
        }
    }
}
