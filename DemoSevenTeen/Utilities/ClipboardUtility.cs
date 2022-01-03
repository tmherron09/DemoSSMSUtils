using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DemoSevenTeen.Utilities
{
    internal static class ClipboardUtility
    {
        public static string GetText()
        {
            string clipboardContents = null;
            var staThread = new Thread(() =>
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        clipboardContents = Clipboard.GetText(TextDataFormat.UnicodeText);
                    }
                }
                catch
                {
                    // To do
                }
            });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();

            return clipboardContents;
        }

        public static bool SetClipboardText(string textToCopy)
        {
            Clipboard.SetText(textToCopy);

            return CompareClipboardText(textToCopy);
        }

        public static bool CompareClipboardText(string textToCompare)
        {
            if (Clipboard.ContainsText())
            {
                return Clipboard.GetText() == textToCompare;
            } else
            {
                return false;
            }
        }

    }
}
