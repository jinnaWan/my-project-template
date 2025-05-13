using Moq;
using System;
using System.Collections.Generic;

namespace MyProject.Api.UnitTests.TestUtils
{
    /// <summary>
    /// Base class for all test fixtures that provides common functionality
    /// </summary>
    public abstract class TestFixtureBase : IDisposable
    {
        protected readonly Dictionary<Type, object> Mocks = new Dictionary<Type, object>();

        /// <summary>
        /// Gets or creates a mock for the specified type
        /// </summary>
        public Mock<T> GetMock<T>() where T : class
        {
            var type = typeof(T);
            if (!Mocks.ContainsKey(type))
            {
                Mocks[type] = new Mock<T>();
            }
            return (Mock<T>)Mocks[type];
        }

        /// <summary>
        /// Resets all registered mocks
        /// </summary>
        public void ResetMocks()
        {
            foreach (var mock in Mocks.Values)
            {
                var mockObj = mock as Mock;
                mockObj?.Reset();
            }
        }

        /// <summary>
        /// Disposes of resources
        /// </summary>
        public virtual void Dispose()
        {
            // Clean up resources if needed
            Mocks.Clear();
        }
    }
} 