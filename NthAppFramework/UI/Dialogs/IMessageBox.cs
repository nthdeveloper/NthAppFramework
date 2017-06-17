using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NthDeveloper.AppFramework.Dialogs
{
    public interface IMessageBox
    {
        bool CheckValue { get; set; }
        System.Drawing.Image Warning { get; }

        //void Initialize();

        DialogResult Show(string message);
        DialogResult Show(string message, MessageBoxButtons buttons);
        DialogResult Show(string message, MessageBoxIcon icon, MessageBoxButtons buttons);
        DialogResult Show(string message, MessageBoxButtons buttons, MessageBoxIcon icon);
        DialogResult Show(string message, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
        DialogResult Show(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
        DialogResult Show(string message, MessageBoxIcon icon);
        DialogResult Show(string message, string caption);
        DialogResult Show(string message, string caption, MessageBoxIcon icon);
        DialogResult Show(string message, string caption, MessageBoxButtons buttons);
        DialogResult Show(string message, string caption, MessageBoxIcon icon, MessageBoxButtons buttons);

        DialogResult Show(string message, string caption, MessageBoxIcon icon, MessageBoxButtons buttons, string checkText);
        DialogResult Show(string message, string caption, MessageBoxIcon icon, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, string chkText);
    }
}
