using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;

namespace CodingWithCalvin.CouchbaseExplorer
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(CouchbaseExplorerWindow))]
    public sealed class CouchbaseExplorerPackage : AsyncPackage
    {
        public const string PackageGuidString = "ef261503-b2ae-4b90-8c86-0becd83348cc";

        protected override async System.Threading.Tasks.Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress
        )
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            CouchbaseExplorerWindowCommand.Initialize(this);
        }
    }
}
