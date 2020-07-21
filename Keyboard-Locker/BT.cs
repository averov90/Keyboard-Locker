namespace KLT
{
    static class BT
    {
        readonly static string WWAY = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\KeyboardLockerBetaLog.log";
        
        public static void MakeCapture()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(WWAY, true, System.Text.Encoding.Default);
            sw.WriteLine("CT: " + System.DateTime.Now + "| HookA: " + HookKeyboard.HookK.Hook_active.ToString() + "| HookWm: " + HookKeyboard.HookK.Hook_workmode + "| CS: " + KeyboardLock.Program.current_settings.hour + ":" + KeyboardLock.Program.current_settings.minute + "-" + KeyboardLock.Program.current_settings.len + "-" + KeyboardLock.Program.current_settings.option);
            sw.Close();
        }

    }
}
