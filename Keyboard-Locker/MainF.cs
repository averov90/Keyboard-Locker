using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KeyboardLock
{
    public partial class MainF : Form
    {
        Point mouse_pos = new Point(0,0); ushort time = 0; bool is_add = true, hide1 = true, no_window = false;
        public MainF()
        {
            InitializeComponent();
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;
            Top = 0;
            Left = 0;
            if (Program.settings.Count > 0)
            {
                Program.set_entry temp = Get_nearlest_back();
                Program.current_settings = new Program.set_entry(temp.hour, temp.minute, temp.len, temp.option);
            }
            else
            {
                Program.current_settings = new Program.set_entry(0, 0, 180, (Program.set_opt_enum)255);
            }
            if (Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard)) { HookKeyboard.HookK.Hook_workmode = 1; HookKeyboard.HookK.Hook_active = true; }
            if (Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerMouse) || Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard)) timer1.Start();
            timer3.Start();
        }

        Program.set_entry Get_nearlest_back()
        {
            DateTime c = DateTime.Now;
            short diff, min = 3;
            Program.set_entry min_o = null;

            for (byte i = 0; i < Program.settings.Count; i++)
            {
                diff = (short)((Program.settings[i].hour * 60 + Program.settings[i].minute) - (c.Hour * 60 + c.Minute));
                if (diff >= 0 && diff < min)
                {
                    min = diff;
                    min_o = Program.settings[i];
                }
            }
            if (min <= 2)
            {
                return min_o;
            }
            
            min = 3;
            for (byte i = 0; i < Program.settings.Count; i++)
            {
                diff = (short)(1439 - (c.Hour * 60 + c.Minute));
                if (diff >= 0 && diff < min)
                {
                    min = diff;
                }
            }
            if (min <= 2)
            {
                min = 3;
                for (byte i = 0; i < Program.settings.Count; i++)
                {
                    diff = (short)(Program.settings[i].hour * 60 + Program.settings[i].minute);
                    if (diff < min)
                    {
                        
                        min = diff;
                        min_o = Program.settings[i];
                    }
                }
                if (min <= 2)
                {
                    return min_o;
                }
            }
            min = 1440;
            for (byte i = 0; i < Program.settings.Count; i++)
            {
                diff = (short)((c.Hour * 60 + c.Minute) - (Program.settings[i].hour * 60 + Program.settings[i].minute));
                if (diff >= 0 && diff < min)
                {
                    min = diff;
                    min_o = Program.settings[i];
                }
            }
            if (min != 1440)
            {
                return min_o;
            }
            for (byte i = 0; i < Program.settings.Count; i++)
            {
                diff = (short)(1439 - (Program.settings[i].hour * 60 + Program.settings[i].minute));
                if (diff >= 0 && diff < min)
                {
                    min = diff;
                    min_o = Program.settings[i];
                }
            }
            return min_o;
        }
        public void Reset_time(bool mode)
        {
            if (mode)
            {
                time = 0;
                if (Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard) && !HookKeyboard.HookK.Hook_active) { HookKeyboard.HookK.Hook_workmode = 1; HookKeyboard.HookK.Hook_active = true; }
                else if (HookKeyboard.HookK.Hook_workmode == 1 && !Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard) && HookKeyboard.HookK.Hook_active) { HookKeyboard.HookK.Hook_active = false; HookKeyboard.HookK.current_pressed = false; }
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerMouse) && !Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard))
                {
                    no_window = false;
                    HookKeyboard.HookK.Hook_active = false;
                    HookKeyboard.HookK.Hook_workmode = 0;
                    HookKeyboard.HookK.current_pressed = false;
                    настройкиToolStripMenuItem.Enabled = true;
                    блокироватьКлавиатуруToolStripMenuItem.Enabled = true;
                    блокироватьКлавиатуруToolStripMenuItem.Text = "Заблокировать клавиатуру";
                    is_add = true;
                }
                n_set = true;
            }
            else
              n_set = true;
        }

        private void MainF_Shown(object sender, EventArgs e)
        {
            if (hide1) { Hide(); hide1 = false; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (is_add)
            {
                
                if ((Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerMouse) ? (Math.Abs(MousePosition.X - mouse_pos.X) < 10 && Math.Abs(MousePosition.Y - mouse_pos.Y) < 10) : true) && (Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard) ? !HookKeyboard.HookK.current_pressed : true))
                {
                    if (HookKeyboard.HookK.current_pressed) HookKeyboard.HookK.current_pressed = false;
                    if (time == Program.current_settings.len)
                    {
                        time = 0;
                        is_add = false;
                        if (Program.current_settings.option.HasFlag(Program.set_opt_enum.Animation)) Animate();
                        if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard))
                            HookKeyboard.HookK.Hook_active = true;
                        HookKeyboard.HookK.Hook_workmode = 0;
                        блокироватьКлавиатуруToolStripMenuItem.Enabled = false;
                        настройкиToolStripMenuItem.Enabled = false;
                        блокироватьКлавиатуруToolStripMenuItem.Text = "Разблокировать клавиатуру";
                        if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowFirstWindow) && !Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowSecondWindow))
                        {
                            блокироватьКлавиатуруToolStripMenuItem.Enabled = true;
                            no_window = true;
                        }
                        else if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowFirstWindow)) HookKeyboard.HookK.Hook_workmode = 2;
                    }
                    else
                    {
                        time++;
                    }
                }
                else
                {
                    if (HookKeyboard.HookK.current_pressed) HookKeyboard.HookK.current_pressed = false;
                    time = 0;
                    mouse_pos.X = MousePosition.X; mouse_pos.Y = MousePosition.Y;
                }
            }
            else
            {
                if (Math.Abs(MousePosition.X - mouse_pos.X) > 10 && Math.Abs(MousePosition.Y - mouse_pos.Y) > 10 && HookKeyboard.HookK.Hook_workmode == 0 && !no_window)
                {
                    timer1.Stop();
                    Check f = new Check();
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        is_add = true;
                        if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard))
                            HookKeyboard.HookK.Hook_active = false;
                        HookKeyboard.HookK.Hook_workmode = 1;
                        блокироватьКлавиатуруToolStripMenuItem.Enabled = true;
                        настройкиToolStripMenuItem.Enabled = true;
                        блокироватьКлавиатуруToolStripMenuItem.Text = "Заблокировать клавиатуру";
                    }
                    else if (dr == DialogResult.Abort)
                    {
                        if (Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowSecondWindow))
                            HookKeyboard.HookK.Hook_workmode = 2;
                        else
                        {
                            блокироватьКлавиатуруToolStripMenuItem.Enabled = true;
                            no_window = true;
                        }
                            
                    }
                    else
                    {
                        mouse_pos.X = MousePosition.X; mouse_pos.Y = MousePosition.Y;
                    }
                    if (Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerMouse) || Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard)) timer1.Start();
                }
                else if (HookKeyboard.HookK.Hook_workmode == 2 && HookKeyboard.HookK.current_pressed)
                {
                    timer1.Stop();
                    Check f = new Check();
                    HookKeyboard.HookK.current_pressed = false;
                    DialogResult dr = f.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        is_add = true;
                        if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard))
                            HookKeyboard.HookK.Hook_active = false;
                        HookKeyboard.HookK.Hook_workmode = 1;
                        блокироватьКлавиатуруToolStripMenuItem.Enabled = true;
                        настройкиToolStripMenuItem.Enabled = true;
                        блокироватьКлавиатуруToolStripMenuItem.Text = "Заблокировать клавиатуру";
                    }
                    else if (dr == DialogResult.Abort)
                    {
                        HookKeyboard.HookK.Hook_workmode = 0;
                        no_window = true;
                        блокироватьКлавиатуруToolStripMenuItem.Enabled = true;
                    }
                    if (Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerMouse) || Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard)) timer1.Start();
                }
            }
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer3.Stop();
            timer1.Stop();
            настройкиToolStripMenuItem.Enabled = false;
            блокироватьКлавиатуруToolStripMenuItem.Enabled = false;
            Set form = new Set() { rt_del = new Set.reset_time(Reset_time) };
            form.ShowDialog();
            c_time = DateTime.Now;
            sunc_pos = 0;
            timer3.Start();
            настройкиToolStripMenuItem.Enabled = true;
            блокироватьКлавиатуруToolStripMenuItem.Enabled = true;
            if (Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerMouse) || Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard)) timer1.Start();
        }

        Bitmap sshot, sshot1; Graphics graph;
        void Animate()
        {
            Show();
            timer1.Stop();
            sshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            graph = Graphics.FromImage(sshot);
            graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            graph.CopyFromScreen(0, 0, 0, 0, sshot.Size);
            graph.DrawString("KeyboardLocker v1.3", new Font("Lucida Console", 14), Brushes.Black, 10,10);
            sshot1 = new Bitmap(sshot);
            graph = Graphics.FromImage(sshot1);
            graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            pos[0] = (ushort)((sshot.Width - Properties.Resources.keyboard_5643.Width) / 2);
            pos[1] = 4;
            pos[2] = (ushort)(Properties.Resources.keyboard_5643.Width + pos[0]);
            pos[3] = (ushort)Properties.Resources.keyboard_5643.Height;
            graph.DrawImage(Properties.Resources.keyboard_5643, pos[0], pos[1]);
            pictureBox1.Image = sshot1;
            timer2.Start(); 
        }

        ushort[] pos = new ushort[4]; byte sunc_pos = 15; byte[] n_time = new byte[2]; bool n_set = true; DateTime c_time;

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (sunc_pos == 15)
            {
                c_time = DateTime.Now;
                sunc_pos = 0;
            }
            else
            {
                c_time = c_time.AddMinutes(1);
                sunc_pos++;
            }
            if (!n_set && c_time.Hour == n_time[0] && Math.Abs(c_time.Minute - n_time[1]) <= 2)
            {
                foreach (Program.set_entry i in Program.settings)
                {
                    if (i.hour == n_time[0] && i.minute == n_time[1])
                    {
                        Program.current_settings = new Program.set_entry(i.hour,i.minute,i.len,i.option);
                        time = 0;
                        if (i.option.HasFlag(Program.set_opt_enum.AnswerKeyboard) && !HookKeyboard.HookK.Hook_active) { HookKeyboard.HookK.Hook_workmode = 1; HookKeyboard.HookK.Hook_active = true; }
                        else if (HookKeyboard.HookK.Hook_workmode == 1 && !i.option.HasFlag(Program.set_opt_enum.AnswerKeyboard) && HookKeyboard.HookK.Hook_active) { HookKeyboard.HookK.Hook_active = false; HookKeyboard.HookK.current_pressed = false; }
                        if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerMouse) && !Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard))
                        {
                            timer1.Stop();
                            no_window = false;
                            HookKeyboard.HookK.Hook_active = false;
                            HookKeyboard.HookK.Hook_workmode = 0;
                            HookKeyboard.HookK.current_pressed = false;
                            настройкиToolStripMenuItem.Enabled = true;
                            блокироватьКлавиатуруToolStripMenuItem.Enabled = true;
                            блокироватьКлавиатуруToolStripMenuItem.Text = "Заблокировать клавиатуру";
                            is_add = true;
                        }
                        else
                            timer1.Start();
                        break;
                    }
                }
                n_set = true;
            }
            if (n_set)
            {
                short diff, diff_min = 1440;
                for (byte i = 0; i < Program.settings.Count; i++)
                {
                    if (Program.settings[i].hour != Program.current_settings.hour || Program.settings[i].minute != Program.current_settings.minute || Program.settings[i].len != Program.current_settings.len || Program.settings[i].option != Program.current_settings.option)
                    {
                        diff = (short)((Program.settings[i].hour * 60 + Program.settings[i].minute) - (c_time.Hour * 60 + c_time.Minute));
                        if (diff >= 0 && diff < diff_min)
                        {
                            diff_min = diff;
                            n_time[0] = Program.settings[i].hour;
                            n_time[1] = Program.settings[i].minute;
                            
                        }
                    }
                }
                if (diff_min == 1440)
                {
                    for (byte i = 0; i < Program.settings.Count; i++)
                    {
                        if (Program.settings[i].hour != Program.current_settings.hour || Program.settings[i].minute != Program.current_settings.minute || Program.settings[i].len != Program.current_settings.len || Program.settings[i].option != Program.current_settings.option)
                        {
                            diff = (short)(Program.settings[i].hour * 60 + Program.settings[i].minute);
                            if (diff >= 0 && diff < diff_min)
                            {
                                diff_min = diff;
                                n_time[0] = Program.settings[i].hour;
                                n_time[1] = Program.settings[i].minute;
                            }
                        }
                    }
                }
                if (diff_min == 1440)
                {
                    n_time[0] = 0;
                    n_time[1] = 0;
                }
                n_set = false;
            }
        }

        Pen pen1 = new Pen(Color.Black,8);
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (pos[1] < Screen.PrimaryScreen.Bounds.Height)
            {
                graph.DrawImage(sshot,0,0);
                pos[1] += 10;
                graph.DrawLine(pen1, 0, 0, pos[0], pos[1]);
                graph.DrawLine(pen1, sshot.Width, 0, pos[2], pos[1]);
                graph.DrawLine(pen1, 0, sshot.Height, pos[0], pos[3] + pos[1]);
                graph.DrawLine(pen1, sshot.Width, sshot.Height, pos[2], pos[3] + pos[1]);
                graph.DrawImage(Properties.Resources.keyboard_5643, pos[0], pos[1]);
                pictureBox1.Image = sshot1;
            }
            else
            {
                Hide();
                timer2.Stop();
                timer1.Start();
            }
        }

        private void блокироватьКлавиатуруToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (no_window)
            {
                no_window = false;
                is_add = true;
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard))
                    HookKeyboard.HookK.Hook_active = false;
                HookKeyboard.HookK.Hook_workmode = 1;
                настройкиToolStripMenuItem.Enabled = true;
                блокироватьКлавиатуруToolStripMenuItem.Text = "Заблокировать клавиатуру";
            }
            else
            {
                if (is_add)
                {
                    блокироватьКлавиатуруToolStripMenuItem.Enabled = false;
                    настройкиToolStripMenuItem.Enabled = false;
                    блокироватьКлавиатуруToolStripMenuItem.Text = "Разблокировать клавиатуру";
                    time = 0;
                    is_add = false;
                    if (Program.current_settings.option.HasFlag(Program.set_opt_enum.Animation)) Animate();
                    if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard))
                        HookKeyboard.HookK.Hook_active = true;
                    HookKeyboard.HookK.Hook_workmode = 0;
                    if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowFirstWindow) && !Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowSecondWindow))
                    {
                        блокироватьКлавиатуруToolStripMenuItem.Enabled = true;
                        no_window = true;
                    }
                    else if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowFirstWindow)) HookKeyboard.HookK.Hook_workmode = 2;
                }
            }
            
        }
    }
}
