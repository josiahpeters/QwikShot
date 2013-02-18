using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QwikShot.WinApp
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();

            comboActiveModifier.SelectedIndex = 1;
            comboActiveKey.SelectedIndex = 1;

            comboRegionModifier.SelectedIndex = 1;
            comboRegionKey.SelectedIndex = 2;
        }
    }
}
