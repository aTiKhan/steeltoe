﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Steeltoe.Connector.Services;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.Oracle.Test
{
    public class OracleProviderConfigurerTest
    {
        [Fact]
        public void UpdateConfiguration_WithNullOracleServiceInfo_ReturnsExpected()
        {
            var configurer = new OracleProviderConfigurer();
            var config = new OracleProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                ServiceName = "orcl"
            };
            configurer.UpdateConfiguration(null, config);

            Assert.Equal("localhost", config.Server);
            Assert.Equal(1234, config.Port);
            Assert.Equal("username", config.Username);
            Assert.Equal("password", config.Password);
            Assert.Equal("orcl", config.ServiceName);
            Assert.Null(config.ConnectionString);
        }

        [Fact]
        public void UpdateConfiguration_WithOracleServiceInfo_ReturnsExpected()
        {
            var configurer = new OracleProviderConfigurer();
            var config = new OracleProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                ServiceName = "orcl"
            };
            var si = new OracleServiceInfo("MyId", "oracle://user:pwd@localhost:1521/orclpdb1");

            configurer.UpdateConfiguration(si, config);

            Assert.Equal("localhost", config.Server);
            Assert.Equal(1521, config.Port);
            Assert.Equal("user", config.Username);
            Assert.Equal("pwd", config.Password);
            Assert.Equal("orclpdb1", config.ServiceName);
        }

        [Fact]
        public void Configure_NoServiceInfo_ReturnsExpected()
        {
            var config = new OracleProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                ServiceName = "orcl"
            };

            var configurer = new OracleProviderConfigurer();
            var opts = configurer.Configure(null, config);
            var connectionString = string.Format("User Id={0};Password={1};Data Source={2}:{3}/{4};", config.Username, config.Password, config.Server, config.Port, config.ServiceName);
            Assert.Equal(connectionString, opts);
        }

        [Fact]
        public void Configure_ServiceInfoOveridesConfig_ReturnsExpected()
        {
            var config = new OracleProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                ServiceName = "orcl"
            };

            var configurer = new OracleProviderConfigurer();
            var si = new OracleServiceInfo("MyId", "oracle://user:pwd@localhost:1521/orclpdb1");

            _ = configurer.Configure(si, config);

            Assert.Equal("localhost", config.Server);
            Assert.Equal(1521, config.Port);
            Assert.Equal("user", config.Username);
            Assert.Equal("pwd", config.Password);
            Assert.Equal("orclpdb1", config.ServiceName);
        }
    }
}
