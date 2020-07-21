using System;
using System.Windows.Forms;

namespace KeyboardLock
{
    public partial class Check : Form
    {
        System.Drawing.Point mouse_pos = new System.Drawing.Point(0, 0);
        public Check()
        {
            InitializeComponent();
            Left = Screen.PrimaryScreen.Bounds.Width - Width;
            Top = Screen.PrimaryScreen.Bounds.Height - Width;
            if (HookKeyboard.HookK.Hook_workmode == 2)
            {
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowWaitInSecond)) button2.Enabled = false;
            }
            else
            {
               if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowWaitInFitst)) button2.Enabled = false; 
            }
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        byte i = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (HookKeyboard.HookK.Hook_workmode == 2)
            {
                if (HookKeyboard.HookK.current_pressed)
                {
                    i = 0;
                    HookKeyboard.HookK.current_pressed = false;
                }
                else
                {
                    i++;
                    if (i == 10)
                    {
                        timer1.Stop();
                        DialogResult = DialogResult.Cancel;
                        Close();
                    }
                }
            }
            else
            {
                if (Math.Abs(MousePosition.X - mouse_pos.X) < 10 && Math.Abs(MousePosition.Y - mouse_pos.Y) < 10)
                {
                    i++;
                    if (i == 10)
                    {
                        timer1.Stop();
                        DialogResult = DialogResult.Cancel;
                        Close();
                    }
                }
                else
                {
                    i = 0;
                    mouse_pos.X = MousePosition.X; mouse_pos.Y = MousePosition.Y;
                }
            }
        }
    }
}
