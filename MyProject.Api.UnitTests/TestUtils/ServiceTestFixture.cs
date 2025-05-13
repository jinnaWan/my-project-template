using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyProject.Api.UnitTests.TestUtils
{
    /// <summary>
    /// A test fixture that can automatically create service instances with mocked dependencies
    /// </summary>
    /// <typeparam name="TService">The type of service to create</typeparam>
    public class ServiceTestFixture<TService> : TestFixtureBase where TService : class
    {
        private TService _service;
        private bool _serviceCreated;

        /// <summary>
        /// Gets the service instance with all dependencies automatically mocked
        /// </summary>
        public TService Service
        {
            get
            {
                if (!_serviceCreated)
                {
                    _service = CreateService();
                    _serviceCreated = true;
                }
                return _service;
            }
        }

        /// <summary>
        /// Creates a service instance by automatically resolving and mocking its dependencies
        /// </summary>
        private TService CreateService()
        {
            // Find the constructor with the most parameters
            var constructorInfo = typeof(TService)
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (constructorInfo == null)
            {
                throw new InvalidOperationException($"No constructor found for {typeof(TService).Name}");
            }

            // For each parameter, get or create a mock
            var parameters = constructorInfo.GetParameters();
            var parameterInstances = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                
                // Use GetMockObject instead of reflection to avoid ambiguous matches
                parameterInstances[i] = GetMockObject(parameterType);
            }

            // Create the service instance
            return (TService)constructorInfo.Invoke(parameterInstances);
        }

        /// <summary>
        /// Gets a mock object for the specified type without using reflection
        /// </summary>
        private object GetMockObject(Type type)
        {
            // Create a generic method that will call GetMock<T>().Object
            var method = typeof(ServiceTestFixture<TService>)
                .GetMethod(nameof(GetTypedMockObject), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(type);
            
            return method.Invoke(this, null);
        }

        /// <summary>
        /// Gets a strongly-typed mock object for the specified type
        /// </summary>
        private T GetTypedMockObject<T>() where T : class
        {
            return GetMock<T>().Object;
        }
    }
} 