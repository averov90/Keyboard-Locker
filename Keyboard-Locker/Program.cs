using System;
using System.Windows.Forms;

namespace KeyboardLock
{
    static class Program
    {
        public static System.Collections.Generic.List<set_entry> settings = new System.Collections.Generic.List<set_entry>(); 
        public static set_entry current_settings = null;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Mutex nmp = new System.Threading.Mutex(true, "{29353cf2-3bee-43d9-9b9c-ae15ea16532e}", out bool runned);
            if (runned)
            {
                Microsoft.Win32.RegistryKey r_key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software",true);
                if (Get_Equals_by_key(r_key.GetSubKeyNames(), "KeyboardLocker"))
                {
                    r_key = r_key.OpenSubKey("KeyboardLocker", true);
                    byte []set = (byte[])r_key.GetValue("klset");
                    if (set != null)
                    {
                        ushort i1;
                        for (byte i = 0; i < (set.Length / 5); i++)
                        {
                            i1 = (ushort)(i * 5);
                            settings.Add(new set_entry(set[i1], set[i1 + 1], BitConverter.ToUInt16(set, i1 + 2), (set_opt_enum)set[i1 + 4]));
                        }
                    }
                }
                else
                {
                    r_key = r_key.CreateSubKey("KeyboardLocker");
                }
                r_key.Close();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainF());
            }
        }
        public static bool Get_Equals_by_key(string[] lines, string key)
        {
            for (ushort i = 0; i < lines.Length; i++)
            {
                if (lines[i] == key)
                {
                    return true;
                }
            }
            return false;
        }
        public enum set_opt_enum
        {
            Animation = 1, ShowFirstWindow = 2, ShowSecondWindow = 4, ShowWaitInFitst = 8, ShowWaitInSecond = 16, AnswerMouse = 32, AnswerKeyboard = 64, ChangeSettings = 128
        }
        public class set_entry //L1lvdSBmb3VuZCBzb21ldGhpbmcv
        {
            public byte hour, minute; public ushort len; public set_opt_enum option;
            public set_entry(byte hour, byte minute, ushort len, set_opt_enum option)
            {
                this.hour = hour;
                this.minute = minute;
                this.len = len;
                this.option = option;
            }
        }
    }
}
