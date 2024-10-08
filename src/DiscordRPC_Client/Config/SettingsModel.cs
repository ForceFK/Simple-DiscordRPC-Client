﻿/*
 * Arquivo: SettingsModel.cs
 * Criado em: 31-12-2021
 * https://github.com/ForceFK
 * Última modificação: 23-08-2024
 */
namespace DiscordRPC.Config
{
    public class SettingsModel
    {
        public string ApplicationID { get; set; }
        public string Details { get; set; }
        public string State { get; set; }
        public string LargeImgKey { get; set; }
        public string LargeImgText { get; set; }
        public string SmallImgKey { get; set; }
        public string SmallImgText { get; set; }
        public bool? Button_1_Enable { get; set; }
        public Button Button_1 { get; set; }
        public bool? Button_2_Enable { get; set; }
        public Button Button_2 { get; set; }
        public bool ShowTimestamps { get; set; }
    }
}
