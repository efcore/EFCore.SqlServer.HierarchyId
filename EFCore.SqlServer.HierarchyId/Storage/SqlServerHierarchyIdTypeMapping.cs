using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microsoft.EntityFrameworkCore.SqlServer.Storage
{
    internal class SqlServerHierarchyIdTypeMapping : RelationalTypeMapping
    {
        private static readonly MethodInfo _getSqlBytes
            = typeof(SqlDataReader).GetRuntimeMethod(nameof(SqlDataReader.GetSqlBytes), new[] { typeof(int) });

        private static Action<DbParameter, SqlDbType> _sqlDbTypeSetter;
        private static Action<DbParameter, string> _udtTypeNameSetter;

        protected virtual ValueConverter<HierarchyId, SqlBytes> ValueConverter { get; }

        public SqlServerHierarchyIdTypeMapping(string storeType, Type clrType)
            : base(CreateRelationalTypeMappingParameters(storeType, clrType))
        {
            ValueConverter = new SqlServerHierarchyIdValueConverter();
        }

        private static RelationalTypeMappingParameters CreateRelationalTypeMappingParameters(string storeType, Type clrType)
        {
            return new RelationalTypeMappingParameters(
                new CoreTypeMappingParameters(
                    clrType),
                storeType);
        }

        // needed to implement Clone
        protected SqlServerHierarchyIdTypeMapping(RelationalTypeMappingParameters parameters, ValueConverter<HierarchyId, SqlBytes> converter)
            : base(parameters)
        {
            ValueConverter = converter;
        }

        /// <summary>
        ///     Creates a copy of this mapping.
        /// </summary>
        /// <param name="parameters"> The parameters for this mapping. </param>
        /// <returns> The newly created mapping. </returns>
        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        {
            return new SqlServerHierarchyIdTypeMapping(parameters, ValueConverter);
        }

        /// <summary>
        ///     Configures type information of a <see cref="DbParameter" />.
        /// </summary>
        /// <param name="parameter"> The parameter to be configured. </param>
        protected override void ConfigureParameter(DbParameter parameter)
        {
            var type = parameter.GetType();
            LazyInitializer.EnsureInitialized(ref _sqlDbTypeSetter, () => CreateSqlDbTypeAccessor(type));
            LazyInitializer.EnsureInitialized(ref _udtTypeNameSetter, () => CreateUdtTypeNameAccessor(type));

            if (parameter.Value == DBNull.Value)
            {
                parameter.Value = SqlBytes.Null;
            }

            _sqlDbTypeSetter(parameter, SqlDbType.Udt);
            _udtTypeNameSetter(parameter, StoreType);
        }

        /// <summary>
        ///     The method to use when reading values of the given type. The method must be defined
        ///     on <see cref="DbDataReader" /> or one of its subclasses.
        /// </summary>
        /// <returns> The method to use to read the value. </returns>
        public override MethodInfo GetDataReaderMethod()
        {
            return _getSqlBytes;
        }

        /// <summary>
        ///     Creates a an expression tree that can be used to generate code for the literal value.
        ///     Currently, only very basic expressions such as constructor calls and factory methods taking
        ///     simple constants are supported.
        /// </summary>
        /// <param name="value"> The value for which a literal is needed. </param>
        /// <returns> An expression tree that can be used to generate code for the literal value. </returns>
        public override Expression GenerateCodeLiteral(object value)
        {
            return Expression.Call(
                typeof(HierarchyId).GetRuntimeMethod("Parse", new[] { typeof(string) }),
                Expression.Constant(value.ToString(), typeof(string))
            );
        }

        /// <summary>
        ///     Generates the SQL representation of a non-null literal value.
        /// </summary>
        /// <param name="value">The literal value.</param>
        /// <returns>
        ///     The generated string.
        /// </returns>
        protected override string GenerateNonNullSqlLiteral(object value)
        {
            if (value is HierarchyId hierarchyId)
                return $"'{hierarchyId}'";

            value = (Converter ?? ValueConverter).ConvertFromProvider(value);
            return $"'{value}'";
        }

        /// <summary>
        ///     Creates a <see cref="DbParameter" /> with the appropriate type information configured.
        /// </summary>
        /// <param name="command"> The command the parameter should be created on. </param>
        /// <param name="name"> The name of the parameter. </param>
        /// <param name="value"> The value to be assigned to the parameter. </param>
        /// <param name="nullable"> A value indicating whether the parameter should be a nullable type. </param>
        /// <returns> The newly created parameter. </returns>
        public override DbParameter CreateParameter(DbCommand command, string name, object value, bool? nullable = null)
        {
            var parameter = command.CreateParameter();
            parameter.Direction = ParameterDirection.Input;
            parameter.ParameterName = name;

            if (Converter != null)
            {
                value = Converter.ConvertToProvider(value);
            }

            parameter.Value = value == null
                ? DBNull.Value
                : ValueConverter.ConvertToProvider(value);

            if (nullable.HasValue)
            {
                parameter.IsNullable = nullable.Value;
            }

            ConfigureParameter(parameter);

            return parameter;
        }

        /// <summary>
        ///     Gets a custom expression tree for the code to convert from the database value
        ///     to the model value.
        /// </summary>
        /// <param name="expression"> The input expression, containing the database value. </param>
        /// <returns> The expression with conversion added. </returns>
        public override Expression CustomizeDataReaderExpression(Expression expression)
        {
            if (expression.Type != ValueConverter.ProviderClrType)
            {
                expression = Expression.Convert(expression, ValueConverter.ProviderClrType);
            }

            return ReplacingExpressionVisitor.Replace(
                ValueConverter.ConvertFromProviderExpression.Parameters.Single(),
                expression,
                ValueConverter.ConvertFromProviderExpression.Body);
        }

        private static Action<DbParameter, SqlDbType> CreateSqlDbTypeAccessor(Type paramType)
        {
            var paramParam = Expression.Parameter(typeof(DbParameter), "parameter");
            var valueParam = Expression.Parameter(typeof(SqlDbType), "value");

            return Expression.Lambda<Action<DbParameter, SqlDbType>>(
                Expression.Call(
                    Expression.Convert(paramParam, paramType),
                    paramType.GetProperty("SqlDbType").SetMethod,
                    valueParam),
                paramParam,
                valueParam).Compile();
        }

        private static Action<DbParameter, string> CreateUdtTypeNameAccessor(Type paramType)
        {
            var paramParam = Expression.Parameter(typeof(DbParameter), "parameter");
            var valueParam = Expression.Parameter(typeof(string), "value");

            return Expression.Lambda<Action<DbParameter, string>>(
                Expression.Call(
                    Expression.Convert(paramParam, paramType),
                    paramType.GetProperty("UdtTypeName").SetMethod,
                    valueParam),
                paramParam,
                valueParam).Compile();
        }
    }
}
