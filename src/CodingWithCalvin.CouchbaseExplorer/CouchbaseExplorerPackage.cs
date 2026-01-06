using System;
using System.Runtime.InteropServices;
using System.Threading;
using CodingWithCalvin.CouchbaseExplorer.Editors;
using CodingWithCalvin.Otel4Vsix;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodingWithCalvin.CouchbaseExplorer
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(CouchbaseExplorerWindow),
        Style = VsDockStyle.Linked,
        Orientation = ToolWindowOrientation.Left,
        Window = ToolWindowGuids.ServerExplorer)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideEditorFactory(typeof(DocumentEditorFactory), 110, TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
    [ProvideEditorExtension(typeof(DocumentEditorFactory), ".cbjson", 50)]
    public sealed class CouchbaseExplorerPackage : AsyncPackage
    {
        public const string PackageGuidString = "ef261503-b2ae-4b90-8c86-0becd83348cc";

        private DocumentEditorFactory _editorFactory;

        protected override async System.Threading.Tasks.Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress
        )
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            var builder = VsixTelemetry.Configure()
                .WithServiceName(VsixInfo.DisplayName)
                .WithServiceVersion(VsixInfo.Version)
                .WithVisualStudioAttributes(this)
                .WithEnvironmentAttributes();

#if !DEBUG
            builder
                .WithOtlpHttp("https://api.honeycomb.io")
                .WithHeader("x-honeycomb-team", HoneycombConfig.ApiKey);
#endif

            builder.Initialize();

            // Register the editor factory
            _editorFactory = new DocumentEditorFactory();
            RegisterEditorFactory(_editorFactory);

            CouchbaseExplorerWindowCommand.Initialize(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                VsixTelemetry.Shutdown();
                _editorFactory?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
