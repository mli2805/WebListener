using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs eventArgs)
        {
            var eventsAsObservable = (from move in Observable.FromEvent<MouseEventArgs>(eventArgs, "MouseDown")
                    select new { move.EventArgs.X, move.EventArgs.Y }).TimeInterval()
                .Where(e => rectangle.Contains(e.Value.X, e.Value.Y) &&
                            e.Interval.TotalMilliseconds < 500);
            eventsAsObservable.Subscribe(e => 
                Console.WriteLine("Double click: X={0}, Y={1}, Interval={2}",
                    e.Value.X, e.Value.Y, e.Interval));
        }
    }
}
