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
