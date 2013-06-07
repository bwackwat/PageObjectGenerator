using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    class Start
    {
        [STAThread]
        private static void Main(string[] args)
        {
//            new Receiver(8055);
            var receiver = new Thread(new ThreadStart(Receiver.StartRecevicer));
//            var gui = new Thread(new ThreadStart(GUI.startGUI));

            receiver.Start();
//            gui.Start();

        }
    }
}
