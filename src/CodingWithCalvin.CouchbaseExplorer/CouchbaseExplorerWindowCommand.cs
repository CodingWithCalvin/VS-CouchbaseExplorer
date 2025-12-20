using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodingWithCalvin.CouchbaseExplorer
{
    internal sealed class CouchbaseExplorerWindowCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("715bc434-afd2-4104-a6a0-287120f718f5");

        private readonly Package _package;

        private CouchbaseExplorerWindowCommand(Package package)
        {
            this._package = package ?? throw new ArgumentNullException(nameof(package));

            if (
                !(
                    this.ServiceProvider.GetService(typeof(IMenuCommandService))
                    is OleMenuCommandService commandService
                )
            )
            {
                return;
            }

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.ShowToolWindow, menuCommandId);

            commandService.AddCommand(menuItem);
        }

        public static CouchbaseExplorerWindowCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider => this._package;

        public static void Initialize(Package package)
        {
            Instance = new CouchbaseExplorerWindowCommand(package);
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            var window = this._package.FindToolWindow(typeof(CouchbaseExplorerWindow), 0, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
