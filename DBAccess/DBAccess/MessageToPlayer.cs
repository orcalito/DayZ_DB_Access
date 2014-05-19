using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBAccess
{
    public partial class MessageToPlayer : Form
    {
        private MainWindow masterForm = null;

        public MessageToPlayer(MainWindow masterForm)
        {
            this.masterForm = masterForm;

            InitializeComponent();
        }
    }
}
