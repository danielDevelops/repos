using danielDevelops.CommonInterfaces.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace danielDevelops.Infrastructure
{
    internal static class Extensions
    {
        public static string GetTableName<T>(this ICustomContext context) where T : class
        {
            var mapping = context.Model.FindEntityType(typeof(T)).Relational();
            var tableName = mapping.TableName;
            return tableName;
        }

        public static IQueryable<T> SkipAndTake<T>(
            this IQueryable<T> query,
            int skip = 0, int?
            take = null) where T : class, new()
        {
            if (take.HasValue)
                return query.Skip(skip).Take(take.Value);
            return query.Skip(skip);
        }

        public static IEnumerable<System.Linq.Expressions.Expression<Func<T, object>>> GetPropertiesFromParams<T>(
            params System.Linq.Expressions.Expression<Func<T, object>>[] props) where T : class, new()
        {
            foreach (var item in props)
            {
                yield return item;
            }
        }

        public static IEnumerable<System.Data.SqlClient.SqlParameter> CreateParameterList(params object[] parameterObjects)
        {
            var parms = new List<System.Data.SqlClient.SqlParameter>();
            var counter = 0;
            foreach (var item in parameterObjects)
            {
                var parm = new System.Data.SqlClient.SqlParameter();
                if (item == null)
                    throw new NullReferenceException("The underlying object parameter is null.  The best thing is I now know why MS has had such a tough time with this one...");
                var type = item.GetType();
                var typeString = type.ToString();
                if (typeString.Contains('.'))
                    typeString = typeString.Substring(typeString.LastIndexOf('.') + 1);
                switch (typeString.ToLower())
                {
                    case "string":
                        parm.DbType = System.Data.DbType.String;
                        break;
                    case "int16":
                    case "int32":
                    case "int":
                        parm.DbType = System.Data.DbType.Int32;
                        break;
                    case "datetime":
                        parm.DbType = System.Data.DbType.DateTime;
                        break;
                    case "boolean":
                        parm.DbType = System.Data.DbType.Boolean;
                        break;
                    case "double":
                        parm.DbType = System.Data.DbType.Double;
                        break;
                    default:
                        throw new NotImplementedException($"The type {type} has not been implemented in this call.  Please use the command ExecuteStoredProcedure and pass SQL parameters.");
                }
                var parmName = $"linq_{counter}";
                parm.ParameterName = parmName;
                parm.Value = item;
                parm.Direction = System.Data.ParameterDirection.Input;
                parms.Add(parm);
                counter++;
            }
            return parms;
        }

        public static void UpdateAllPropertiesToValue<TEntity, TEntityValue>(this ICustomContext context, TEntity entity, string propertyName, TEntityValue value) where TEntity : class
        {
            if (entity.GetType().GetProperty(propertyName) != null)
            {
                var propertyType = entity.GetType().GetProperty(propertyName).PropertyType;
                propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                if (propertyType != value.GetType())
                    throw new InvalidCastException($"The type doesn't match the property that is being inserted for {entity.GetType()} - {propertyName}.  {propertyType.ToString()} != {value.GetType().ToString()}");
                entity.GetType().GetProperty(propertyName).SetValue(entity, value);
                if (context.Entry(entity).State != EntityState.Detached)
                    context.Entry(entity).Property(propertyName).IsModified = true;
            }
            foreach (var property in entity.GetType().GetProperties())
            {
                if (property.PropertyType.IsClass && property.PropertyType.IsByRef)
                    UpdateAllPropertiesToValue(context, property, propertyName, value);
                if (property.PropertyType.IsInterface && property.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null)
                {
                    var propValue = entity.GetType().GetProperty(property.Name).GetValue(entity);
                    if (propValue == null)
                        continue;
                    foreach (var propValueCollectionItem in (System.Collections.IEnumerable)propValue)
                    {
                        UpdateAllPropertiesToValue(context, propValueCollectionItem, propertyName, value);
                    }
                }
            }
        }
        public static void SetFieldsAsNotModified<TEntity>(this ICustomContext context, TEntity entity, string[] excludedProperties = null) where TEntity : class
        {
            foreach (var property in entity.GetType().GetProperties())
            {
                if ((property.PropertyType.IsClass || property.PropertyType.IsPrimitive || property.PropertyType.IsValueType) &&
                    excludedProperties.Contains(property.Name))
                    context.Entry(entity).Property(property.Name).IsModified = false;

                else if (property.PropertyType.IsInterface && property.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null)
                {
                    var propValue = entity.GetType().GetProperty(property.Name).GetValue(entity);
                    if (propValue == null)
                        continue;
                    foreach (var propValueCollectionItem in (System.Collections.IEnumerable)propValue)
                    {
                        SetFieldsAsNotModified(context, propValueCollectionItem);
                    }
                }
            }
        }

    }
}
