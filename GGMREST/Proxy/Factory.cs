using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using GGMREST.Proxy.Exception;

namespace GGMREST.Proxy
{
    public class Factory
    {
        private ModuleBuilder _moduleBuilder;
        private string _baseUrl;
        private IEnumerable<KeyValuePair<string, string>> _defaultHeaders = new Dictionary<string, string>();
        
        public Factory(string baseUrl)
        {
            _baseUrl = baseUrl;
            Type factoryType = typeof(Factory);
            AssemblyName assemblyName = new AssemblyName($"{factoryType.Namespace}.{factoryType.Name}_{Guid.NewGuid()}");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName,
                AssemblyBuilderAccess.Run);
            _moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
        }

        public Factory SetDefaultHeaders(IEnumerable<KeyValuePair<string, string>> defaultHeaders)
        {
            _defaultHeaders = defaultHeaders;
            return this;
        }

        public Type CreateType(Type interfaceType)
        {
            TypeBuilder typeBuilder = _moduleBuilder.DefineType($"{typeof(Factory).Name}+{interfaceType.FullName}", TypeAttributes.Public);
            
            typeBuilder.SetParent(typeof(DynamicProxy));
            typeBuilder.AddInterfaceImplementation(interfaceType);
            var implementedMethods = interfaceType.GetMethods();
            foreach (var implementedMethod in implementedMethods)
            {
                Type returnType = implementedMethod.ReturnType;
                if(returnType.GetGenericTypeDefinition() != typeof(Task<>))
                    throw new WrongReturnTypeException(returnType);
                    
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(implementedMethod.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual, implementedMethod.ReturnType,
                    implementedMethod.GetParameters().Select(pi => pi.ParameterType).ToArray());
                ILGenerator ilGenerator = methodBuilder.GetILGenerator();
                LocalBuilder typeLocalBuilder = ilGenerator.DeclareLocal(typeof(Type), true);
                LocalBuilder paramsLocalBuilder = ilGenerator.DeclareLocal(typeof(List<object>), true);
                LocalBuilder resultTypeLocalBuilder = ilGenerator.DeclareLocal(typeof(object), true);
                LocalBuilder retLocalBuilder = ilGenerator.DeclareLocal(typeof(bool), true);
                
                //C#: Type.GetTypeFromHandle(interfaceType)
                ilGenerator.Emit(OpCodes.Ldtoken, implementedMethod.DeclaringType);
                Expression<Action<RuntimeTypeHandle>> ilGetTypeFromHandle = handle => Type.GetTypeFromHandle(handle);
                ilGenerator.EmitCall(OpCodes.Call, (ilGetTypeFromHandle.Body as MethodCallExpression).Method, null);
                ilGenerator.Emit(OpCodes.Stloc_0);

                //C#: params = new List<object>()
                ilGenerator.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(Type.EmptyTypes));
                ilGenerator.Emit(OpCodes.Stloc_1);
                //C#: params.Add(param[i])
                ParameterInfo[] parameterInfos = implementedMethod.GetParameters();
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    ilGenerator.Emit(OpCodes.Ldloc_1);
                    ilGenerator.Emit(OpCodes.Ldarg, i + 1);
                    if (parameterInfos[i].ParameterType.IsValueType)
                        ilGenerator.Emit(OpCodes.Box, parameterInfos[i].ParameterType);
                    ilGenerator.EmitCall(OpCodes.Callvirt, typeof(List<object>).GetMethod("Add"), null);
                }
                //C#: ret = DynamicProxy.TryInvokeMember(interfaceType, propertyName, params, out result)
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldstr, implementedMethod.Name);
                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.EmitCall(OpCodes.Callvirt, typeof(List<object>).GetMethod("ToArray"), null);
                ilGenerator.Emit(OpCodes.Ldloca_S, 2);
                object dummyOut;
                Expression<Action<DynamicProxy>> exp = (o => o.TryInvokeMember(null, null, null, out dummyOut));
                MethodCallExpression mcexp = (MethodCallExpression) exp.Body;
                ilGenerator.EmitCall(OpCodes.Callvirt, mcexp.Method, null);
                ilGenerator.Emit(OpCodes.Stloc_3);

                if (implementedMethod.ReturnType != typeof(void))
                {
                    ilGenerator.Emit(OpCodes.Ldloc_2);
                    //C#: if(ret == ValueType && ret == null){
                    //    ret = Activator.CreateInstance(returnType) }
                    if (implementedMethod.ReturnType.IsValueType)
                    {
                        Label retisnull = ilGenerator.DefineLabel();
                        Label endofif = ilGenerator.DefineLabel();

                        ilGenerator.Emit(OpCodes.Ldnull);
                        ilGenerator.Emit(OpCodes.Ceq);
                        ilGenerator.Emit(OpCodes.Brtrue_S, retisnull);
                        ilGenerator.Emit(OpCodes.Ldloc_2);
                        ilGenerator.Emit(OpCodes.Unbox_Any, implementedMethod.ReturnType);
                        ilGenerator.Emit(OpCodes.Br_S, endofif);
                        ilGenerator.MarkLabel(retisnull);
                        ilGenerator.Emit(OpCodes.Ldtoken, implementedMethod.ReturnType);
                        ilGetTypeFromHandle = handle => Type.GetTypeFromHandle(handle);
                        ilGenerator.EmitCall(OpCodes.Call, (ilGetTypeFromHandle.Body as MethodCallExpression).Method,
                            null);
                        Expression<Action<object>> ilCreateInstace = o => Activator.CreateInstance(o.GetType());
                        ilGenerator.EmitCall(OpCodes.Call, (ilCreateInstace.Body as MethodCallExpression).Method, null);
                        ilGenerator.Emit(OpCodes.Unbox_Any, implementedMethod.ReturnType);
                        ilGenerator.MarkLabel(endofif);
                    }
                }
                //C#: return ret
                ilGenerator.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(methodBuilder, implementedMethod);
            }
            //typeBuilder.DefineMethodOverride();
            return typeBuilder.CreateType();
        }

        public object Create(Type interfaceType)
        {
            object instance = Activator.CreateInstance(CreateType(interfaceType));
            ((DynamicProxy) instance).BaseUrl = _baseUrl;
            ((DynamicProxy) instance).DefaultHeader = _defaultHeaders;
            return instance;
        }

        public T Create<T>() where T : class
        {
            return Create(typeof(T)) as T;
        }
    }
}