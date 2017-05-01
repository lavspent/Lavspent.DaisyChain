///*
//    Copyright(c) 2017 Petter Labråten/LAVSPENT.NO. All rights reserved.

//    The MIT License(MIT)

//    Permission is hereby granted, free of charge, to any person obtaining a
//    copy of this software and associated documentation files (the "Software"),
//    to deal in the Software without restriction, including without limitation
//    the rights to use, copy, modify, merge, publish, distribute, sublicense,
//    and/or sell copies of the Software, and to permit persons to whom the
//    Software is furnished to do so, subject to the following conditions:

//    The above copyright notice and this permission notice shall be included in
//    all copies or substantial portions of the Software.

//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//    DEALINGS IN THE SOFTWARE.
//*/

//using System;
//using System.Linq;
//using System.Reflection;

//namespace Lavspent.DaisyChain.OneWire
//{
//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
//    public class OneWireDeviceFactoryAttribute : Attribute
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        private Type _factoryType;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="factoryType"></param>
//        public OneWireDeviceFactoryAttribute(Type factoryType)
//        {
//            _factoryType = factoryType;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public Type FactoryType
//        {
//            get
//            {
//                return this._factoryType;
//            }
//        }

//        /// <summary>
//        /// Get attributed factory for type T, or null if none exists.
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        public static IOneWireDeviceFactory<T> GetFactory<T>() where T : class, IOneWireDevice
//        {
//            IOneWireDeviceFactory<T> factory = null;

//            if (factory == null)
//                factory = GetFactoryFromClassAttribute<T>();

//            //if (factory == null)
//            //    factory = GetFactoryFromClass<T>();

//            // todo: check for static implementation of factory method?
//            //if (factory == null)
//            //    factory = GetStaticFactoryMethodFromClass<T>();

//            return factory;
//        }


//        // TODO: No need for generics here?
//        private static IOneWireDeviceFactory<T> GetFactoryFromClassAttribute<T>() where T : class, IOneWireDevice
//        {
//            // get factory class from attribute
//            var factoryAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<OneWireDeviceFactoryAttribute>();

//            if (factoryAttribute == null)
//                return null;

//            // validate factory
//            var factoryType = factoryAttribute.FactoryType;

//            if (!HasInterface(factoryType, typeof(IOneWireDeviceFactory<T>)))
//                throw new Exception($"Factory {factoryType.Name} does not implement {nameof(IOneWireDeviceFactory<T>)}.");

//            if (!HasTrivialContructor(factoryType))
//                throw new ArgumentException($"Factory {factoryType.Name} has no parameterless constructor.");

//            // create and return instance
//            return ((IOneWireDeviceFactory<T>)Activator.CreateInstance(factoryType));
//        }

//        // TODO: No need for generics here?
//        private static IOneWireDeviceFactory<T> GetFactoryFromClass<T>() where T : class, IOneWireDevice
//        {
//            var classType = typeof(T);

//            // check if class implement factory interface
//            if (!HasInterface(classType, typeof(IOneWireDeviceFactory<T>)))
//                return null;

//            // check if class implement trivial contructor
//            if (!HasTrivialContructor(classType))
//                return null;

//            // create and return instance
//            return ((IOneWireDeviceFactory<T>)Activator.CreateInstance(classType));
//        }

//        //// TODO: No need for generics here?
//        //private static IOneWireDeviceFactory<T> GetStaticFactoryMethodFromClass<T>() where T : class, IOneWireDevice
//        //{
//        //    var factoryInterfaceTypeInfo = typeof(IOneWireDeviceFactory<T>).GetTypeInfo();

//        //    // get description of method to search for fromfactory interface
//        //    MethodInfo factoryMethodInfo = null;
//        //    try
//        //    {
//        //        factoryMethodInfo = factoryInterfaceTypeInfo.DeclaredMethods.SingleOrDefault();
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        throw new Exception("Factory interface brakes rule of one single method. The I in SOLID.", e);
//        //    }

//        //    // check if we find any similar named static method on our type
//        //    var classTypeInfo = typeof(T).GetTypeInfo();
//        //    var staticMethodInfos = classTypeInfo.GetDeclaredMethods(factoryMethodInfo.Name).Where(m => m.IsStatic);

//        //    // check if any got the same set of parameters
//        //    var foundMethodInfo = staticMethodInfos.FirstOrDefault(
//        //        (mi) => AreMethodsInterchangable(factoryMethodInfo, mi));

//        //    return null;
//        //}

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="lhs"></param>
//        /// <param name="rhs"></param>
//        /// <returns></returns>
//        private static bool AreMethodsInterchangable(MethodInfo lhs, MethodInfo rhs)
//        {
//            var lhsParams = lhs.GetParameters().ToList();
//            var rhsParams = rhs.GetParameters().ToList();

//            // check count
//            if (lhsParams.Count != rhsParams.Count)
//                return false;

//            // check each parameter insequence
//            for (int i = 0; i < lhsParams.Count; i++)
//            {
//                if (!AreParametersInterchangable(lhsParams[i], rhsParams[i]))
//                    return false;
//            }

//            // check return parameter
//            if (!AreParametersInterchangable(lhs.ReturnParameter, rhs.ReturnParameter))
//                return false;

//            return true;
//        }

//        private static bool AreParametersInterchangable(ParameterInfo lhs, ParameterInfo rhs)
//        {
//            // TODO: This, for now.
//            if (!lhs.ParameterType.Equals(rhs.ParameterType))
//                return false;

//            return true;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="type"></param>
//        /// <returns></returns>
//        private static bool HasInterface(Type type, Type interfaceType)
//        {
//            // check if type implement factory interface
//            var implementedInterfaces = type.GetTypeInfo().ImplementedInterfaces;
//            return implementedInterfaces.Any((i) => i.Equals(interfaceType));
//        }


//        /// <summary>
//        /// Check is type has a parameterless constructor.
//        /// </summary>
//        /// <param name="type"></param>
//        /// <returns></returns>
//        private static bool HasTrivialContructor(Type type)
//        {
//            using (var contructorEnumerator = type.GetTypeInfo().DeclaredConstructors.GetEnumerator())
//            {
//                while (contructorEnumerator.MoveNext())
//                {
//                    var contructorInfo = contructorEnumerator.Current;
//                    if (contructorInfo.GetParameters().Length == 0)
//                        return true;
//                }
//            }

//            return false;
//        }
//    }
//}