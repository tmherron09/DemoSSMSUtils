using System;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;


namespace DemoSevenTeen.Utilities
{
    /// <summary>
    /// Helper class to work with <see cref="IWpfTextView"/>.
    /// </summary>
    internal static class VsWpfExtensionUtility
    {

        public async static Task<IWpfTextView> GetWpfTextViewAsync(IAsyncServiceProvider serviceProvider)
        {
            try
            {
                var textManager = await serviceProvider.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
                Assumes.Present(textManager);
                var componentModel = await serviceProvider.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
                Assumes.Present(componentModel);
                var editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();
                IVsTextView textViewCurrent;
                textManager.GetActiveView(1, null, out textViewCurrent);
                return editor.GetWpfTextView(textViewCurrent);
            }
            catch
            {
                return null;
            }
        }

        public static int GetCursorPosition(IWpfTextView wpfTextView)
        {
            return wpfTextView.Caret.Position.BufferPosition.Position;
        }

    }
}
