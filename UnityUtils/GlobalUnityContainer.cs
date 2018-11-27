using Unity;
using Unity.Interception.Utilities;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;

namespace UnityUtils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GlobalUnityContainer
    {
        private static IUnityContainer _container;
        
        static GlobalUnityContainer()
        {
            _container = new UnityContainer();
        }

        public static void ReplaceContainer(IUnityContainer parentContainer)
        {
            lock (_container)
            {
                _container = parentContainer;
            }
        }

        public static void InitContainer(IUnityContainer parentContainer)
        {
            lock (_container)
            {
                _container = parentContainer.CreateChildContainer();
            }
        }

        public static void InitContainer(ITypesRegistrator typesRegistrator)
        {
            lock (_container)
            {
                typesRegistrator.RegisterAll(_container);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns></returns>
        public static void Register<TFrom, TTo>() where TTo : TFrom
        {
            lock (_container)
            {
                _container.RegisterType<TFrom, TTo>(new TransientLifetimeManager());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns></returns>
        public static void Register<TFrom, TTo>(LifetimeManager lifetimeManager = null, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            lock (_container)
            {
                if (lifetimeManager == null)
                    _container.RegisterType<TFrom, TTo>(injectionMembers);
                else
                    _container.RegisterType<TFrom, TTo>(lifetimeManager, injectionMembers);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns></returns>
        public static void Register<TFrom, TTo>(string name = null, LifetimeManager lifetimeManager = null, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            lock (_container)
            {
                if (lifetimeManager == null)
                    _container.RegisterType<TFrom, TTo>(name, injectionMembers);
                else
                    _container.RegisterType<TFrom, TTo>(name, lifetimeManager, injectionMembers);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static void Register(Type from, Type to, string name = null, LifetimeManager lifetimeManager = null, params InjectionMember[] injectionMembers)
        {
            lock (_container)
            {
                _container.RegisterType(from, to, name, lifetimeManager, injectionMembers);
            }
        }

        public static void RegisterInstance<T>(T instance, LifetimeManager lifetimeManager = null, string name = null)
        {
            lock (_container)
            {
                if (lifetimeManager == null)
                    _container.RegisterInstance(name, instance);
                else
                {
                    _container.RegisterInstance(name, instance, lifetimeManager);
                }
            }
        }

        /// <summary>
        /// Возвращает обьект типа Т через параметризированный конструктор
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructorParameters"> значения параметров конструктора "{parameterName,parameterValue}"</param>
        /// <returns></returns>
        public static T Resolve<T>(params KeyValuePair<string, object>[] constructorParameters)
        {
            return Resolve<T>(null, constructorParameters);
        }

        /// <summary>
        /// Возвращает обьект типа Т через параметризированный конструктор. если тип не зарегестрирован, возвращает false без исключения.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="constructorParameters"> значения параметров конструктора "{parameterName,parameterValue}"</param>
        /// <returns></returns>
        public static bool TryResolve<T>(out T instance, string name = null, params KeyValuePair<string, object>[] constructorParameters)
        {
            lock (_container)
            {
                instance = default(T);
                if (!_container.IsRegistered<T>(name))
                    return false;
                instance = Resolve<T>(name, constructorParameters);
                return true;
            }
        }

        /// <summary>
        /// Зарегистрирован ли тип в контейнере
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsRegistered<T>()
        {
            lock (_container)
            {
                return _container.IsRegistered<T>();
            }
        }

        /// <summary>
        /// Возвращает именованый обьект типа Т через параметризированный конструктор
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName"></param>
        /// <param name="constructorParameters"> значения параметров конструктора "{parameterName,parameterValue}"</param>
        /// <returns></returns>
        public static T Resolve<T>(string objectName, params KeyValuePair<string, object>[] constructorParameters)
        {
            lock (_container)
            {
                var resolverOverride = new List<ResolverOverride>();
                constructorParameters.ForEach(v => resolverOverride.Add(new ParameterOverride(v.Key, v.Value)));
                if (objectName == null ? _container.IsRegistered<T>() : _container.IsRegistered<T>(objectName))
                    return objectName == null
                        ? _container.Resolve<T>(resolverOverride.ToArray())
                        : _container.Resolve<T>(objectName, resolverOverride.ToArray());
                throw new Exception(string.Format("Тип {0} не зарегестрирован в контейнере", typeof (T)));
            }
        }

        /// <summary>
        /// Возвращает именованый обьект типа Т через параметризированный конструктор
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructorParameters"> значения параметров конструктора "{parameterName,parameterValue}"</param>
        /// <returns></returns>
        public static IEnumerable<T> ResolveAll<T>(params KeyValuePair<string, object>[] constructorParameters)
        {
            lock (_container)
            {
                var resolverOverride = new List<ResolverOverride>();
                constructorParameters.ForEach(v => resolverOverride.Add(new ParameterOverride(v.Key, v.Value)));
                var retList = _container.ResolveAll<T>(resolverOverride.ToArray()).ToList();
                if (retList.Count == 0)
                    throw new Exception(string.Format("Тип {0} не зарегестрирован в контейнере", typeof (T)));
                return retList;
            }
        }
    }
}
