using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AdbProxy.DeviceIntegrationTests
{
    internal class FakeServer : IServer
    {
        private readonly FakeFeatureCollection _features = new FakeFeatureCollection();

        private class FakeServerAddressesFeature : IServerAddressesFeature
        {
            internal string[] _addresses = [];

            public ICollection<string> Addresses => _addresses;

            public bool PreferHostingUrls { get; set; }
        }


        internal class FakeFeatureCollection : IFeatureCollection
        {
            private readonly ConcurrentDictionary<Type, object> _features = [];

            public FakeFeatureCollection()
            {
                Set(new FakeServerAddressesFeature());
            }

            public object? this[Type key] 
            { 
                get
                {
                    _features.TryGetValue(key, out var obj);
                    return obj;
                }
                set
                {
                    if (value is not null)
                    {
                        _features[key] = value;
                    }
                }
            }

            public bool IsReadOnly => true;

            public int Revision => 1;

            public TFeature? Get<TFeature>()
            {
                return default(TFeature);
            }

            public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
            {
                return _features.GetEnumerator();
            }

            public void Set<TFeature>(TFeature? instance)
            {
                if (instance is not null)
                {
                    _features[instance.GetType()] = instance;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _features.GetEnumerator();
            }
        }

        public IFeatureCollection Features => _features;

        public void Dispose()
        {
        }

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken) where TContext : notnull
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
