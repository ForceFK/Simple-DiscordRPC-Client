/*
 * Arquivo: SettingsManager.cs
 * Criado em: 31-12-2021
 * https://github.com/ForceFK
 * Última modificação: 23-08-2024
 */
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordRPC.Config
{
    public static class SettingsManager
    {
        internal static void Save(this SettingsModel set)
            => File.WriteAllText(Environment.CurrentDirectory + @"\Settings.json", JsonConvert.SerializeObject(set, Formatting.Indented));

        internal static Task Load(MainWindow main, out SettingsModel st)
        {
            string file = Environment.CurrentDirectory + @"\Settings.json";
            if (File.Exists(file))
            {
                st = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(file));

                main.TXT_ApplicationID.Text = st.ApplicationID;
                main.TXT_Details.Text = st.Details;
                main.TXT_State.Text = st.State;
                main.TXT_LargeImgKey.Text = st.LargeImgKey;
                main.TXT_LargeImgText.Text = st.LargeImgText;
                main.TXT_SmallImgKey.Text = st.SmallImgKey;
                main.TXT_SmallImgText.Text = st.SmallImgText;

                main.TXT_Button1Label.Text = st.Button_1?.Label;
                main.TXT_Button1Url.Text = st.Button_1?.Url;

                main.TXT_Button2Label.Text = st.Button_2?.Label;
                main.TXT_Button2Url.Text = st.Button_2?.Url;
                main.buttons[0] = st.Button_1;
                main.buttons[1] = st.Button_2;
                main.CheckBox_Button1.IsChecked = st.Button_1_Enable;
                main.CheckBox_Button2.IsChecked = st.Button_2_Enable;

                main.CheckBox_Timestamps.IsChecked = st.ShowTimestamps;

            }
            else
                st = new SettingsModel();

            return Task.CompletedTask;
        }
    }
}
