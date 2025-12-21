using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using CodingWithCalvin.CouchbaseExplorer.Services;

namespace CodingWithCalvin.CouchbaseExplorer.ViewModels
{
    public enum TestConnectionStatus
    {
        NotTested,
        Testing,
        Success,
        Failed
    }

    public class ConnectionDialogViewModel : INotifyPropertyChanged
    {
        private string _connectionName;
        private string _host;
        private string _username;
        private string _password;
        private bool _useSsl;
        private bool _isCouchbaseServer = true;
        private bool _isCouchbaseCapella;

        private string _connectionNameError;
        private string _hostError;
        private string _usernameError;
        private string _passwordError;

        private TestConnectionStatus _testStatus = TestConnectionStatus.NotTested;
        private string _testErrorMessage;

        private readonly HashSet<string> _existingConnectionNames;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<bool?> RequestClose;

        public bool IsEditMode { get; }
        public string OriginalConnectionName { get; }

        public string DialogTitle => IsEditMode ? "Edit Couchbase Connection" : "Add Couchbase Connection";

        public string ConnectionName
        {
            get => _connectionName;
            set
            {
                if (SetProperty(ref _connectionName, value))
                {
                    ValidateConnectionName();
                    ResetTestStatus();
                }
            }
        }

        public string Host
        {
            get => _host;
            set
            {
                if (SetProperty(ref _host, value))
                {
                    ValidateHost();
                    DetectCapellaUrl();
                    ResetTestStatus();
                }
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    ValidateUsername();
                    ResetTestStatus();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ValidatePassword();
                    ResetTestStatus();
                }
            }
        }

        public bool UseSsl
        {
            get => _useSsl;
            set
            {
                if (SetProperty(ref _useSsl, value))
                {
                    ResetTestStatus();
                }
            }
        }

        public bool IsCouchbaseServer
        {
            get => _isCouchbaseServer;
            set
            {
                if (SetProperty(ref _isCouchbaseServer, value))
                {
                    if (value)
                    {
                        IsCouchbaseCapella = false;
                    }
                    OnPropertyChanged(nameof(HostHint));
                    OnPropertyChanged(nameof(IsSslEditable));
                    ResetTestStatus();
                }
            }
        }

        public bool IsCouchbaseCapella
        {
            get => _isCouchbaseCapella;
            set
            {
                if (SetProperty(ref _isCouchbaseCapella, value))
                {
                    if (value)
                    {
                        IsCouchbaseServer = false;
                        UseSsl = true; // Force SSL for Capella
                    }
                    OnPropertyChanged(nameof(HostHint));
                    OnPropertyChanged(nameof(IsSslEditable));
                    ResetTestStatus();
                }
            }
        }

        public bool IsSslEditable => !IsCouchbaseCapella;

        public string HostHint => IsCouchbaseCapella
            ? "(e.g., cb.xxxxx.cloud.couchbase.com)"
            : "(e.g., localhost or 192.168.1.100)";

        // Validation errors
        public string ConnectionNameError
        {
            get => _connectionNameError;
            set => SetProperty(ref _connectionNameError, value);
        }

        public string HostError
        {
            get => _hostError;
            set => SetProperty(ref _hostError, value);
        }

        public string UsernameError
        {
            get => _usernameError;
            set => SetProperty(ref _usernameError, value);
        }

        public string PasswordError
        {
            get => _passwordError;
            set => SetProperty(ref _passwordError, value);
        }

        // Test connection status
        public TestConnectionStatus TestStatus
        {
            get => _testStatus;
            set
            {
                if (SetProperty(ref _testStatus, value))
                {
                    OnPropertyChanged(nameof(TestStatusMessage));
                    OnPropertyChanged(nameof(TestStatusColor));
                }
            }
        }

        public string TestStatusMessage
        {
            get
            {
                return TestStatus switch
                {
                    TestConnectionStatus.NotTested => "Not tested",
                    TestConnectionStatus.Testing => "Testing...",
                    TestConnectionStatus.Success => "Connection successful",
                    TestConnectionStatus.Failed => $"Connection failed: {_testErrorMessage}",
                    _ => "Not tested"
                };
            }
        }

        public Brush TestStatusColor
        {
            get
            {
                return TestStatus switch
                {
                    TestConnectionStatus.NotTested => Brushes.Gray,
                    TestConnectionStatus.Testing => Brushes.Orange,
                    TestConnectionStatus.Success => Brushes.Green,
                    TestConnectionStatus.Failed => Brushes.Red,
                    _ => Brushes.Gray
                };
            }
        }

        public ICommand TestConnectionCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ConnectionDialogViewModel(HashSet<string> existingConnectionNames = null, ConnectionNode existingConnection = null)
        {
            _existingConnectionNames = existingConnectionNames ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (existingConnection != null)
            {
                IsEditMode = true;
                OriginalConnectionName = existingConnection.Name;
                _connectionName = existingConnection.Name;
                _host = existingConnection.ConnectionString;
                _username = existingConnection.Username;
                _useSsl = existingConnection.UseSsl;

                _password = CredentialManagerService.GetPassword(existingConnection.Id);

                // Detect if it's Capella based on URL
                if (!string.IsNullOrEmpty(_host) && _host.Contains(".cloud.couchbase.com"))
                {
                    _isCouchbaseCapella = true;
                    _isCouchbaseServer = false;
                }
            }

            TestConnectionCommand = new RelayCommand(async _ => await TestConnectionAsync(), CanTestConnection);
            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void DetectCapellaUrl()
        {
            if (!string.IsNullOrEmpty(Host) && Host.Contains(".cloud.couchbase.com"))
            {
                IsCouchbaseCapella = true;
            }
        }

        private void ValidateConnectionName()
        {
            if (string.IsNullOrWhiteSpace(ConnectionName))
            {
                ConnectionNameError = "Connection name is required";
            }
            else if (_existingConnectionNames.Contains(ConnectionName) &&
                     (!IsEditMode || !string.Equals(ConnectionName, OriginalConnectionName, StringComparison.OrdinalIgnoreCase)))
            {
                ConnectionNameError = "A connection with this name already exists";
            }
            else
            {
                ConnectionNameError = null;
            }
        }

        private void ValidateHost()
        {
            if (string.IsNullOrWhiteSpace(Host))
            {
                HostError = "Host is required";
            }
            else
            {
                HostError = null;
            }
        }

        private void ValidateUsername()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                UsernameError = "Username is required";
            }
            else
            {
                UsernameError = null;
            }
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = "Password is required";
            }
            else
            {
                PasswordError = null;
            }
        }

        private void ValidateAll()
        {
            ValidateConnectionName();
            ValidateHost();
            ValidateUsername();
            ValidatePassword();
        }

        private bool HasValidationErrors()
        {
            return !string.IsNullOrEmpty(ConnectionNameError) ||
                   !string.IsNullOrEmpty(HostError) ||
                   !string.IsNullOrEmpty(UsernameError) ||
                   !string.IsNullOrEmpty(PasswordError);
        }

        private bool AreAllFieldsFilled()
        {
            return !string.IsNullOrWhiteSpace(ConnectionName) &&
                   !string.IsNullOrWhiteSpace(Host) &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private void ResetTestStatus()
        {
            if (TestStatus != TestConnectionStatus.NotTested)
            {
                TestStatus = TestConnectionStatus.NotTested;
                _testErrorMessage = null;
            }
        }

        private bool CanTestConnection(object parameter)
        {
            return AreAllFieldsFilled() && TestStatus != TestConnectionStatus.Testing;
        }

        private async Task TestConnectionAsync()
        {
            ValidateAll();
            if (HasValidationErrors())
            {
                return;
            }

            TestStatus = TestConnectionStatus.Testing;

            try
            {
                // TODO: Implement actual Couchbase connection test
                // For now, simulate a connection test
                await Task.Delay(1000);

                // Placeholder: In real implementation, use Couchbase SDK to test connection
                // var cluster = await Cluster.ConnectAsync(connectionString, options);
                // await cluster.WaitUntilReadyAsync(TimeSpan.FromSeconds(10));

                TestStatus = TestConnectionStatus.Success;
            }
            catch (Exception ex)
            {
                _testErrorMessage = ex.Message;
                TestStatus = TestConnectionStatus.Failed;
            }
        }

        private bool CanSave()
        {
            ValidateAll();
            return AreAllFieldsFilled() && !HasValidationErrors();
        }

        private void Save()
        {
            ValidateAll();
            if (HasValidationErrors())
            {
                return;
            }

            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(false);
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
