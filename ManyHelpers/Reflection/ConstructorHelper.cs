using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManyHelpers.Reflection {
    public class ConstructorHelper {
        public delegate object ObjectActivator(params object[] args);

        public ObjectActivator CreateConstructor<T>(Type[] arguments) => CreateConstructor(typeof(T), arguments);

        public ObjectActivator CreateConstructor(Type type, Type[] arguments) {
            var constructor = type.GetConstructor(arguments);
            var activator = typeof(ObjectActivator);
            ParameterInfo[] paramsInfo = constructor.GetParameters();
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");
            Expression[] argsExp = new Expression[paramsInfo.Length];

            for (int i = 0; i < paramsInfo.Length; i++) {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }

            NewExpression newExp = Expression.New(constructor, argsExp);
            LambdaExpression lambda = Expression.Lambda(activator, newExp, param);
            return (ObjectActivator)lambda.Compile();
        }

        public Delegate CreateConstructorFroGenericClass(Type type, Type[] argumentsType, Type[] arguments) {
            Type constructed = type.MakeGenericType(argumentsType);
            return CreateConstructor(constructed, arguments);
        }
    }
}
