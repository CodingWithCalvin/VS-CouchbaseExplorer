using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CodingWithCalvin.Otel4Vsix;
using Couchbase;
using Couchbase.Management.Buckets;

namespace CodingWithCalvin.CouchbaseExplorer.Services
{
    public class ClusterConnection : IDisposable
    {
        public ICluster Cluster { get; }
        public bool HasQueryService { get; private set; } = true;
        public bool HasKvService { get; private set; } = true;
        public List<string> AvailableServices { get; private set; } = new List<string> { "KV", "Query" };

        public ClusterConnection(ICluster cluster)
        {
            Cluster = cluster;
        }

        public void Dispose()
        {
            Cluster?.Dispose();
        }
    }

    public class BucketInfo
    {
        public string Name { get; set; }
        public BucketType BucketType { get; set; }
        public long RamQuotaMB { get; set; }
        public int NumReplicas { get; set; }
    }

    public class ScopeInfo
    {
        public string Name { get; set; }
        public List<CollectionInfo> Collections { get; set; } = new List<CollectionInfo>();
    }

    public class CollectionInfo
    {
        public string Name { get; set; }
        public string ScopeName { get; set; }
    }

    public static class CouchbaseService
    {
        private static readonly Dictionary<string, ClusterConnection> _connections = new Dictionary<string, ClusterConnection>();

        public static async Task<ClusterConnection> ConnectAsync(string connectionId, string connectionString, string username, string password, bool useSsl)
        {
            using var activity = VsixTelemetry.StartCommandActivity("CouchbaseService.ConnectAsync");

            try
            {
                                activity?.SetTag("connection.isCapella", connectionString.Contains(".cloud.couchbase.com"));

                // Enable TLS 1.2/1.3 explicitly for .NET Framework
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                if (_connections.TryGetValue(connectionId, out var existing))
                {
                    VsixTelemetry.LogInformation("Reusing existing connection");
                    return existing;
                }

                var options = new ClusterOptions
                {
                    UserName = username,
                    Password = password,
                    KvTimeout = TimeSpan.FromSeconds(10),
                    ManagementTimeout = TimeSpan.FromSeconds(10),
                    QueryTimeout = TimeSpan.FromSeconds(10)
                };

                // Check if this is a Capella connection
                var isCapella = connectionString.Contains(".cloud.couchbase.com");

                if (useSsl || isCapella)
                {
                    options.EnableTls = true;
                }

                // Capella-specific configuration
                if (isCapella)
                {
                    options.EnableDnsSrvResolution = true;
                    options.KvIgnoreRemoteCertificateNameMismatch = true;
                    options.HttpIgnoreRemoteCertificateMismatch = true;
                    options.ForceIPv4 = true;
                }

                // Build connection string with protocol
                var fullConnectionString = connectionString;
                if (!connectionString.StartsWith("couchbase://") && !connectionString.StartsWith("couchbases://"))
                {
                    fullConnectionString = useSsl ? $"couchbases://{connectionString}" : $"couchbase://{connectionString}";
                }

                VsixTelemetry.LogInformation("Connecting to Couchbase cluster");

                // Connect on background thread to avoid UI blocking
                var cluster = await Task.Run(async () =>
                {
                    return await Cluster.ConnectAsync(fullConnectionString, options);
                }).ConfigureAwait(true);

                var connection = new ClusterConnection(cluster);
                _connections[connectionId] = connection;

                VsixTelemetry.LogInformation("Successfully connected to Couchbase cluster");

                return connection;
            }
            catch (Exception ex)
            {
                activity?.RecordError(ex);
                VsixTelemetry.TrackException(ex, new Dictionary<string, object>
                {
                    { "operation.name", "ConnectAsync" },
                    { "connection.id", connectionId }
                });
                throw;
            }
        }

        public static async Task DisconnectAsync(string connectionId)
        {
            using var activity = VsixTelemetry.StartCommandActivity("CouchbaseService.DisconnectAsync");

                        if (_connections.TryGetValue(connectionId, out var connection))
            {
                _connections.Remove(connectionId);
                connection.Dispose();
                VsixTelemetry.LogInformation("Disconnected from Couchbase cluster");
            }
        }

        public static ClusterConnection GetConnection(string connectionId)
        {
            _connections.TryGetValue(connectionId, out var connection);
            return connection;
        }

        public static async Task<List<BucketInfo>> GetBucketsAsync(string connectionId)
        {
            using var activity = VsixTelemetry.StartCommandActivity("CouchbaseService.GetBucketsAsync");

            try
            {
                                var connection = GetConnection(connectionId);
                if (connection == null)
                {
                    throw new InvalidOperationException("Not connected to cluster");
                }

                var buckets = await connection.Cluster.Buckets.GetAllBucketsAsync();

                var result = buckets.Values.Select(b => new BucketInfo
                {
                    Name = b.Name,
                    BucketType = b.BucketType,
                    RamQuotaMB = (long)(b.RamQuotaMB),
                    NumReplicas = b.NumReplicas
                }).OrderBy(b => b.Name).ToList();

                activity?.SetTag("buckets.count", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                activity?.RecordError(ex);
                VsixTelemetry.TrackException(ex, new Dictionary<string, object>
                {
                    { "operation.name", "GetBucketsAsync" },
                    { "connection.id", connectionId }
                });
                throw;
            }
        }

        public static async Task<List<ScopeInfo>> GetScopesAsync(string connectionId, string bucketName)
        {
            using var activity = VsixTelemetry.StartCommandActivity("CouchbaseService.GetScopesAsync");

            try
            {
                                                var connection = GetConnection(connectionId);
                if (connection == null)
                {
                    throw new InvalidOperationException("Not connected to cluster");
                }

                var bucket = await connection.Cluster.BucketAsync(bucketName);
                var scopes = await bucket.Collections.GetAllScopesAsync();

                var result = scopes.Select(s => new ScopeInfo
                {
                    Name = s.Name,
                    Collections = s.Collections.Select(c => new CollectionInfo
                    {
                        Name = c.Name,
                        ScopeName = s.Name
                    }).OrderBy(c => c.Name).ToList()
                }).OrderBy(s => s.Name).ToList();

                activity?.SetTag("scopes.count", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                activity?.RecordError(ex);
                VsixTelemetry.TrackException(ex, new Dictionary<string, object>
                {
                    { "operation.name", "GetScopesAsync" },
                    { "connection.id", connectionId },
                    { "bucket.name", bucketName }
                });
                throw;
            }
        }

        public static async Task<List<CollectionInfo>> GetCollectionsAsync(string connectionId, string bucketName, string scopeName)
        {
            var scopes = await GetScopesAsync(connectionId, bucketName);
            var scope = scopes.FirstOrDefault(s => s.Name == scopeName);
            return scope?.Collections ?? new List<CollectionInfo>();
        }

        public static async Task<DocumentQueryResult> GetDocumentIdsAsync(string connectionId, string bucketName, string scopeName, string collectionName, int limit = 50, int offset = 0)
        {
            using var activity = VsixTelemetry.StartCommandActivity("CouchbaseService.GetDocumentIdsAsync");

            try
            {
                                                                                var connection = GetConnection(connectionId);
                if (connection == null)
                {
                    throw new InvalidOperationException("Not connected to cluster");
                }

                var query = $"SELECT META().id FROM `{bucketName}`.`{scopeName}`.`{collectionName}` ORDER BY META().id LIMIT {limit + 1} OFFSET {offset}";

                var result = await connection.Cluster.QueryAsync<DocumentIdResult>(query);
                var documentIds = new List<string>();

                await foreach (var row in result.Rows)
                {
                    documentIds.Add(row.Id);
                }

                // Check if there are more documents (we fetched limit+1 to check)
                var hasMore = documentIds.Count > limit;
                if (hasMore)
                {
                    documentIds.RemoveAt(documentIds.Count - 1);
                }

                activity?.SetTag("documents.count", documentIds.Count);
                activity?.SetTag("documents.hasMore", hasMore);

                return new DocumentQueryResult
                {
                    DocumentIds = documentIds,
                    HasMore = hasMore
                };
            }
            catch (Exception ex)
            {
                activity?.RecordError(ex);
                VsixTelemetry.TrackException(ex, new Dictionary<string, object>
                {
                    { "operation.name", "GetDocumentIdsAsync" },
                    { "connection.id", connectionId },
                    { "bucket.name", bucketName },
                    { "scope.name", scopeName },
                    { "collection.name", collectionName }
                });
                throw;
            }
        }

        public static async Task<DocumentContent> GetDocumentAsync(string connectionId, string bucketName, string scopeName, string collectionName, string documentId)
        {
            using var activity = VsixTelemetry.StartCommandActivity("CouchbaseService.GetDocumentAsync");

            try
            {
                                                                                                var connection = GetConnection(connectionId);
                if (connection == null)
                {
                    throw new InvalidOperationException("Not connected to cluster");
                }

                var bucket = await connection.Cluster.BucketAsync(bucketName);
                var scope = bucket.Scope(scopeName);
                var collection = scope.Collection(collectionName);

                var result = await collection.GetAsync(documentId);

                VsixTelemetry.LogInformation("Retrieved document");

                return new DocumentContent
                {
                    Id = documentId,
                    Content = result.ContentAs<object>(),
                    Cas = result.Cas
                };
            }
            catch (Exception ex)
            {
                activity?.RecordError(ex);
                VsixTelemetry.TrackException(ex, new Dictionary<string, object>
                {
                    { "operation.name", "GetDocumentAsync" },
                    { "connection.id", connectionId },
                    { "document.id", documentId }
                });
                throw;
            }
        }
    }

    public class DocumentIdResult
    {
        public string Id { get; set; }
    }

    public class DocumentQueryResult
    {
        public List<string> DocumentIds { get; set; } = new List<string>();
        public bool HasMore { get; set; }
    }

    public class DocumentContent
    {
        public string Id { get; set; }
        public object Content { get; set; }
        public ulong Cas { get; set; }
    }
}
