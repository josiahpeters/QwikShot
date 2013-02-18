using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QwikShot.WinApp
{
    public class CustomToolStripRenderer : ToolStripSystemRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // disable this so we have no border
            //base.OnRenderToolStripBorder(e);
        }
    }
}
