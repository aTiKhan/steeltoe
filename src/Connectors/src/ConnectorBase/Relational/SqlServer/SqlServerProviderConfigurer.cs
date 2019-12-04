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
using System;
using System.Net;

namespace Steeltoe.CloudFoundry.Connector.SqlServer
{
    public class SqlServerProviderConfigurer
    {
        public string Configure(SqlServerServiceInfo si, SqlServerProviderConnectorOptions configuration)
        {
            UpdateConfiguration(si, configuration);
            return configuration.ToString();
        }

        public void UpdateConfiguration(SqlServerServiceInfo si, SqlServerProviderConnectorOptions configuration)
        {
            if (si == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(si.Uri))
            {
                configuration.Port = si.Port;
                configuration.Server = si.Host;
                if (!string.IsNullOrEmpty(si.Path))
                {
                    configuration.Database = si.Path;
                }

                if (si.Query != null)
                {
                    foreach (var piece in si.Query.Split('&'))
                    {
                        var kvp = piece.Split('=');
                        if (kvp[0].EndsWith("database", StringComparison.InvariantCultureIgnoreCase) || kvp[0].EndsWith("databaseName", StringComparison.InvariantCultureIgnoreCase))
                        {
                            configuration.Database = kvp[1];
                        }
                        else if (kvp[0].EndsWith("instancename", StringComparison.InvariantCultureIgnoreCase))
                        {
                            configuration.InstanceName = kvp[1];
                        }
                        else if (kvp[0].StartsWith("hostnameincertificate", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // adding this key could result in "System.ArgumentException : Keyword not supported: 'hostnameincertificate'" later
                        }
                        else
                        {
                            configuration.Options.Add(kvp[0], kvp[1]);
                        }
                    }
                }

                if (configuration.UrlEncodedCredentials)
                {
                    configuration.Username = WebUtility.UrlDecode(si.UserName);
                    configuration.Password = WebUtility.UrlDecode(si.Password);
                }
                else
                {
                    configuration.Username = si.UserName;
                    configuration.Password = si.Password;
                }
            }
        }
    }
}
