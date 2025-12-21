using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace CodingWithCalvin.CouchbaseExplorer.Services
{
    /// <summary>
    /// Service for persisting connection information in Visual Studio settings.
    /// Passwords are NOT stored here - they go in Windows Credential Manager.
    /// </summary>
    public class ConnectionSettingsService
    {
        private const string CollectionPath = "CouchbaseExplorer";
        private const string ConnectionsPropertyName = "Connections";

        private readonly WritableSettingsStore _settingsStore;

        public ConnectionSettingsService()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            _settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            EnsureCollectionExists();
        }

        private void EnsureCollectionExists()
        {
            if (!_settingsStore.CollectionExists(CollectionPath))
            {
                _settingsStore.CreateCollection(CollectionPath);
            }
        }

        public List<ConnectionInfo> LoadConnections()
        {
            try
            {
                if (!_settingsStore.PropertyExists(CollectionPath, ConnectionsPropertyName))
                {
                    return new List<ConnectionInfo>();
                }

                var json = _settingsStore.GetString(CollectionPath, ConnectionsPropertyName);

                if (string.IsNullOrEmpty(json))
                {
                    return new List<ConnectionInfo>();
                }

                return JsonSerializer.Deserialize<List<ConnectionInfo>>(json) ?? new List<ConnectionInfo>();
            }
            catch (Exception)
            {
                return new List<ConnectionInfo>();
            }
        }

        public void SaveConnections(List<ConnectionInfo> connections)
        {
            var json = JsonSerializer.Serialize(connections ?? new List<ConnectionInfo>());
            _settingsStore.SetString(CollectionPath, ConnectionsPropertyName, json);
        }

        public void AddConnection(ConnectionInfo connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var connections = LoadConnections();
            connections.Add(connection);
            SaveConnections(connections);
        }

        public void UpdateConnection(ConnectionInfo connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var connections = LoadConnections();
            var index = connections.FindIndex(c => c.Id == connection.Id);

            if (index >= 0)
            {
                connections[index] = connection;
                SaveConnections(connections);
            }
        }

        public void DeleteConnection(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
                return;

            var connections = LoadConnections();
            connections.RemoveAll(c => c.Id == connectionId);
            SaveConnections(connections);

            CredentialManagerService.DeletePassword(connectionId);
        }

        public ConnectionInfo GetConnection(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
                return null;

            var connections = LoadConnections();
            return connections.Find(c => c.Id == connectionId);
        }
    }
}
