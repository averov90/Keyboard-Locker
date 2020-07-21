using System;
using System.Windows.Forms;

namespace KeyboardLock
{
    public partial class Set : Form
    {
        bool cs = true, current_change = false, set_cc = true;
        bool change_settings{ get {return cs;} set {cs = value; if (value) { label10.Text = "З: Р"; label10.BackColor = System.Drawing.Color.MediumSeaGreen; } else { label10.Text = "З: З"; label10.BackColor = System.Drawing.Color.SandyBrown; } } }
        public Set()
        {
            InitializeComponent();
            pos[0] = (ushort)Screen.GetWorkingArea(this).Width; pos[1] = (ushort)Screen.GetWorkingArea(this).Height;
            pos[2] = (ushort)Width; pos[3] = (ushort)Height;
        }

        private void Set_Shown(object sender, EventArgs e)
        {
            if (MessageBox.Show("Изменяйте настройки только если знаете, что делаете!", "Внимание!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Cancel) Close();
        }

        private void Set_Load(object sender, EventArgs e)
        {
            Renov_listbox();
            bool finded = false;
            for (byte i = 0; i < Program.settings.Count; i++)
            {
                if (Program.settings[i].hour == Program.current_settings.hour && Program.settings[i].minute == Program.current_settings.minute)
                {
                    if (Program.settings[i].len == Program.current_settings.len && Option_compare(Program.settings[i].option, Program.current_settings.option))
                    {
                        set_cc = false;
                        listBox1.SelectedIndex = Get_LB_by_List_index(i);
                        finded = true;
                    }
                    break;
                }
            }
            if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ChangeSettings) && finded) change_settings = false;
            if (!finded)
            {
                varH.Value = Program.current_settings.hour;
                varM.Value = Program.current_settings.minute;
                varL.Value = Program.current_settings.len;
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.Animation)) checkBox1.Checked = false;
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowFirstWindow)) checkBox2.Checked = false;
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowSecondWindow)) checkBox3.Checked = false;
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowWaitInFitst)) checkBox4.Checked = false;
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowWaitInSecond)) checkBox5.Checked = false;
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerMouse)) checkBox6.Checked = false;
                if (!Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard)) checkBox7.Checked = false;
            }
        }

        bool Option_compare(Program.set_opt_enum a, Program.set_opt_enum b)
        {
            a |= Program.set_opt_enum.ChangeSettings;
            b |= Program.set_opt_enum.ChangeSettings;
            return a == b;
        }

        void Renov_listbox()
        {
            listBox1.Items.Clear();
            ushort[] time_list = new ushort[Program.settings.Count], temp = new ushort[Program.settings.Count];
            for (byte i = 0; i < time_list.Length; i++)
            {
                time_list[i] = (ushort)(Program.settings[i].hour * 60 + Program.settings[i].minute);
                temp[i] = time_list[i];
            }
            Array.Sort(time_list);
            foreach (ushort i in time_list)
                for (byte i1 = 0; i1 < temp.Length; i1++)
                    if (i == temp[i1])
                        listBox1.Items.Add(Program.settings[i1].hour + ":" + Program.settings[i1].minute);
        }

        byte Get_LB_by_List_index(byte index)
        {
            string name = Program.settings[index].hour + ":" + Program.settings[index].minute;
            for (byte i = 0; i < listBox1.Items.Count; i++)
                if (listBox1.Items[i].ToString() == name)
                    return i;
            return 0;
        }

        byte Get_List_by_LB_index(byte index)
        {
            string[] temp = listBox1.Items[index].ToString().Split(':');
            for (byte i = 0; i < Program.settings.Count; i++)
                if (Program.settings[i].hour.ToString() == temp[0] && Program.settings[i].minute.ToString() == temp[1])
                    return i;
            return 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Program.current_settings.option.HasFlag(Program.set_opt_enum.ChangeSettings) || current_change)
            {
                if (MessageBox.Show("Вы уверены, что хотите выполнить это действие?", "Сообщение", MessageBoxButtons.YesNo) == DialogResult.No) return;
                Program.set_opt_enum opt = new Program.set_opt_enum();
                for (byte i = 0; i < Program.settings.Count; i++)
                {
                    if (Program.settings[i].hour == varH.Value && Program.settings[i].minute == varM.Value)
                    {
                        Program.settings[i].len = (ushort)varL.Value;
                        if (checkBox1.Checked) opt |= Program.set_opt_enum.Animation;
                        if (checkBox2.Checked) opt |= Program.set_opt_enum.ShowFirstWindow;
                        if (checkBox3.Checked) opt |= Program.set_opt_enum.ShowSecondWindow;
                        if (checkBox4.Checked) opt |= Program.set_opt_enum.ShowWaitInFitst;
                        if (checkBox5.Checked) opt |= Program.set_opt_enum.ShowWaitInSecond;
                        if (checkBox6.Checked) opt |= Program.set_opt_enum.AnswerMouse;
                        if (checkBox7.Checked) opt |= Program.set_opt_enum.AnswerKeyboard;
                        if (current_change)
                        {
                            if (change_settings) opt |= Program.set_opt_enum.ChangeSettings;
                        }
                        else if(Program.settings[i].option.HasFlag(Program.set_opt_enum.ChangeSettings) != change_settings)
                        {
                            if (Program.settings[i].option.HasFlag(Program.set_opt_enum.ChangeSettings)) opt |= Program.set_opt_enum.ChangeSettings;
                        }
                        Program.settings[i].option = opt;
                        listBox1.SelectedIndex = Get_LB_by_List_index(i);
                        rt_del(false);
                        Write_reg();
                        return;
                    }
                }
                if (Program.settings.Count == 255) { MessageBox.Show("Невозможно добавить ещё одну запись!", "Ошибка", MessageBoxButtons.OK); return; };
                if (checkBox1.Checked) opt |= Program.set_opt_enum.Animation;
                if (checkBox2.Checked) opt |= Program.set_opt_enum.ShowFirstWindow;
                if (checkBox3.Checked) opt |= Program.set_opt_enum.ShowSecondWindow;
                if (checkBox4.Checked) opt |= Program.set_opt_enum.ShowWaitInFitst;
                if (checkBox5.Checked) opt |= Program.set_opt_enum.ShowWaitInSecond;
                if (checkBox6.Checked) opt |= Program.set_opt_enum.AnswerMouse;
                if (checkBox7.Checked) opt |= Program.set_opt_enum.AnswerKeyboard;
                if (current_change)
                {
                    if (change_settings) opt |= Program.set_opt_enum.ChangeSettings;
                }
                else
                    opt |= Program.set_opt_enum.ChangeSettings;
                Program.settings.Add(new Program.set_entry((byte)varH.Value, (byte)varM.Value, (ushort)varL.Value, opt));
                rt_del(false);
                Write_reg();
                Renov_listbox();
                listBox1.SelectedIndex = Get_LB_by_List_index((byte)(Program.settings.Count - 1));
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                byte i = Get_List_by_LB_index((byte)listBox1.SelectedIndex);
                varH.Value = Program.settings[i].hour;
                varM.Value = Program.settings[i].minute;
                varL.Value = Program.settings[i].len;
                if (Program.settings[i].option.HasFlag(Program.set_opt_enum.Animation)) checkBox1.Checked = true; else checkBox1.Checked = false;
                if (Program.settings[i].option.HasFlag(Program.set_opt_enum.ShowFirstWindow)) checkBox2.Checked = true; else checkBox2.Checked = false;
                if (Program.settings[i].option.HasFlag(Program.set_opt_enum.ShowSecondWindow)) checkBox3.Checked = true; else checkBox3.Checked = false;
                if (Program.settings[i].option.HasFlag(Program.set_opt_enum.ShowWaitInFitst)) checkBox4.Checked = true; else checkBox4.Checked = false;
                if (Program.settings[i].option.HasFlag(Program.set_opt_enum.ShowWaitInSecond)) checkBox5.Checked = true; else checkBox5.Checked = false;
                if (Program.settings[i].option.HasFlag(Program.set_opt_enum.AnswerMouse)) checkBox6.Checked = true; else checkBox6.Checked = false;
                if (Program.settings[i].option.HasFlag(Program.set_opt_enum.AnswerKeyboard)) checkBox7.Checked = true; else checkBox7.Checked = false;
                if (set_cc)
                    if (Program.settings[i].option.HasFlag(Program.set_opt_enum.ChangeSettings)) change_settings = true; else change_settings = false;
                else
                    set_cc = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && (Program.current_settings.option.HasFlag(Program.set_opt_enum.ChangeSettings) || current_change))
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить элемент?", "Сообщение", MessageBoxButtons.YesNo) == DialogResult.No) return;
                Program.settings.RemoveAt(Get_List_by_LB_index((byte)listBox1.SelectedIndex));
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                varH.Value = 0;
                varM.Value = 0;
                varL.Value = 180;
                checkBox1.Checked = true;
                checkBox2.Checked = true;
                checkBox3.Checked = true;
                checkBox4.Checked = true;
                checkBox5.Checked = true;
                checkBox6.Checked = true;
                checkBox7.Checked = true;
                change_settings = true;
                rt_del(false);
                Write_reg();
            }
        }
        void Write_reg()
        {
            Microsoft.Win32.RegistryKey r_key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\KeyboardLocker", true);
            byte[] set_data = new byte[Program.settings.Count * 5], t = new byte[2];
            ushort i1;
            for (byte i = 0; i < Program.settings.Count; i++)
            {
                i1 = (ushort)(i * 5);
                set_data[i1] = Program.settings[i].hour;
                set_data[i1 + 1] = Program.settings[i].minute;
                t = BitConverter.GetBytes(Program.settings[i].len);
                set_data[i1 + 2] = t[0];
                set_data[i1 + 3] = t[1];
                set_data[i1 + 4] = (byte)Program.settings[i].option;
            }
            r_key.SetValue("klset", set_data, Microsoft.Win32.RegistryValueKind.Binary);
            r_key.Close();
        }

        public delegate void reset_time(bool mode); public reset_time rt_del;
        private void button4_Click(object sender, EventArgs e)
        {
            if (Program.current_settings.option.HasFlag(Program.set_opt_enum.ChangeSettings) || current_change)
            {
                Program.set_opt_enum opt = new Program.set_opt_enum();
                if (checkBox1.Checked) opt |= Program.set_opt_enum.Animation;
                if (checkBox2.Checked) opt |= Program.set_opt_enum.ShowFirstWindow;
                if (checkBox3.Checked) opt |= Program.set_opt_enum.ShowSecondWindow;
                if (checkBox4.Checked) opt |= Program.set_opt_enum.ShowWaitInFitst;
                if (checkBox5.Checked) opt |= Program.set_opt_enum.ShowWaitInSecond;
                if (checkBox6.Checked) opt |= Program.set_opt_enum.AnswerMouse;
                if (checkBox7.Checked) opt |= Program.set_opt_enum.AnswerKeyboard;
                if (current_change)
                {
                    if (change_settings) opt |= Program.set_opt_enum.ChangeSettings;
                }
                else
                    opt |= Program.set_opt_enum.ChangeSettings;
                Program.current_settings = new Program.set_entry((byte)varH.Value, (byte)varM.Value, (ushort)varL.Value, opt);
                rt_del(true);
            }
        }
        ushort[] pos = new ushort[4];
        private void Set_Move(object sender, EventArgs e)
        {
            if (Left < 0)
            {
                Left = 0;
            }
            else if (pos[2] + Left > pos[0])
            {
                Left = pos[0] - pos[2];
            }
            if (Top < 0)
            {
                Top = 0;
            }
            else if (pos[3] + Top > pos[1])
            {
                Top = pos[1] - pos[3];
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Renov_listbox();
            bool finded = false;
            listBox1.SelectedIndex = -1;
            if (current_change)
                for (byte i = 0; i < Program.settings.Count; i++)
                {
                    if (Program.settings[i].hour == Program.current_settings.hour && Program.settings[i].minute == Program.current_settings.minute)
                    {
                        if (Program.settings[i].len == Program.current_settings.len && Program.settings[i].option == Program.current_settings.option)
                        {
                            listBox1.SelectedIndex = Get_LB_by_List_index(i);
                            finded = true;
                        }
                        break;
                    }
                }
            else
            {
                for (byte i = 0; i < Program.settings.Count; i++)
                {
                    if (Program.settings[i].hour == Program.current_settings.hour && Program.settings[i].minute == Program.current_settings.minute)
                    {
                        if (Program.settings[i].len == Program.current_settings.len && Option_compare(Program.settings[i].option, Program.current_settings.option))
                        {
                            set_cc = false;
                            listBox1.SelectedIndex = Get_LB_by_List_index(i);
                            finded = true;
                        }
                        break;
                    }
                }
                if (finded) if (Program.current_settings.option.HasFlag(Program.set_opt_enum.ChangeSettings)) change_settings = true; else change_settings = false;
            }
            if (!finded)
            {
                varH.Value = Program.current_settings.hour;
                varM.Value = Program.current_settings.minute;
                varL.Value = Program.current_settings.len;
                if (Program.current_settings.option.HasFlag(Program.set_opt_enum.Animation)) checkBox1.Checked = true; else checkBox1.Checked = false;
                if (Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowFirstWindow)) checkBox2.Checked = true; else checkBox2.Checked = false;
                if (Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowSecondWindow)) checkBox3.Checked = true; else checkBox3.Checked = false;
                if (Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowWaitInFitst)) checkBox4.Checked = true; else checkBox4.Checked = false;
                if (Program.current_settings.option.HasFlag(Program.set_opt_enum.ShowWaitInSecond)) checkBox5.Checked = true; else checkBox5.Checked = false;
                if (Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerMouse)) checkBox6.Checked = true; else checkBox6.Checked = false;
                if (Program.current_settings.option.HasFlag(Program.set_opt_enum.AnswerKeyboard)) checkBox7.Checked = true; else checkBox7.Checked = false;
                if (Program.current_settings.option.HasFlag(Program.set_opt_enum.ChangeSettings)) change_settings = true; else change_settings = false;
            }
        }

        private void Set_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 79 && e.Alt == true)
            {
                WSet f = new WSet() { write = change_settings };
                f.ShowDialog();
                change_settings = f.write;
                current_change = true;
                label10.Visible = true;
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {
            WSet f = new WSet() { write = change_settings };
            f.ShowDialog();
            change_settings = f.write;
        }
    }
}
