using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Holy_Pandas_Key_Caps_Sounds
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true; 
                var sound = new SoundPlayer(@"Holy Pandas down.wav");
                sound.Play(); 
            }).Start();
            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true; 
                var sound = new SoundPlayer(@"Holy Pandas up.wav");
                sound.Play(); 
            }).Start();
        }

    }
}