using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NthDeveloper.AppFramework.Dialogs
{
    public interface ICommonDialogs
    {
        System.Drawing.Color SelectColor(System.Drawing.Color initialCol, out bool result);
        System.Drawing.Font SelectFont(System.Drawing.Font initialFont);

        string NewFile(string filter, string defDir, string defaultExt);

        string SelectFile(string filter, string defDir);
        string SelectFile(string filter, int filterIndex, string defDir, string defFile);
        string[] SelectFileMulti(string filter, string defDir);        

        string SelectFolder(string defDir);
        string SelectFolderWithInput(string defDir, string customTitle, object ownerWindow);
    }
}
