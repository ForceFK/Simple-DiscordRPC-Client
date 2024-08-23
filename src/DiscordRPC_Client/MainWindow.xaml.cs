/*
 * Arquivo: MainWindow.xaml.cs
 * Criado em: 24-12-2021
 * https://github.com/ForceFK
 * Última modificação: 23-08-2024
 */
using DiscordRPC.Config;
using DiscordRPC.Tasks;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DiscordRPC
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DiscordPresenceTask discordPresence;
        internal readonly Button[] buttons = new Button[] { null, null };
        private SettingsModel settigns;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Botão Iniciar/Parar
        /// </summary>
        private async void Btn_StartStop_Click(object sender, RoutedEventArgs e)
        {
            Btn_StartStop.IsEnabled = false;

            //Se o cliente já está inicializado, vamos parar ou poderá ocorrer erros caso iniciar mais de um
            if (discordPresence != null && discordPresence.IsInitialized)
            {
                await discordPresence.Stop();

                Btn_StartStop.Content = "Start";
                Grid_Head.IsEnabled = true;
            }
            else
            {
                //Vamos inciar uma instancia do DiscordPresenceTask passando o applicationID
                discordPresence = new DiscordPresenceTask(ulong.Parse(settigns.ApplicationID));

                //Inicializar o cliente DiscordRPC para se conectar ao discord.
                await discordPresence.Start();

                Btn_StartStop.Content = "Stop";
                Grid_Head.IsEnabled = false;

                //Criamos o presence com os labels e definimos no cliente
                //Para mais duvida consulte o painel de desenvolvedor do discord
                await discordPresence.SetPresence(new RichPresence
                {
                    Details = TXT_Details.Text,
                    State = TXT_State.Text,
                    Assets = new Assets
                    {
                        LargeImageKey = TXT_LargeImgKey.Text,
                        LargeImageText = TXT_LargeImgText.Text,
                        SmallImageKey = TXT_SmallImgKey.Text,
                        SmallImageText = TXT_SmallImgText.Text
                    },

                    //Máximo de botões no presence é 2
                    //Pegamos a lista de botões, ignorando os valores nulos e convertemos em array
                    Buttons = buttons.Where(x => x != null).ToArray(),

                    //Tempo crescente no presence
                    Timestamps = CheckBox_Timestamps.IsChecked.GetValueOrDefault() ? Timestamps.Now : null
                });
            }
            Btn_StartStop.Focus();
            await Task.Delay(20);
            Btn_StartStop.IsEnabled = true;
        }


        /// <summary>
        /// Desconectar o clienteRPC no fechamento do form
        /// </summary>
        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            if (discordPresence != null)
                await discordPresence?.Stop();
        }

        #region Evento de componentes do form sem muita importância

        private void Btn_SaveAppID_Click(object sender, RoutedEventArgs e)
        {
            if (TXT_ApplicationID.Text.Length == 0)
            {
                Btn_StartStop.IsEnabled = false;
            }
            else
            {
                if (TXT_ApplicationID.Text.Length <= 5)
                {
                    MessageBox.Show("Invalid ApplicationID!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Btn_StartStop.IsEnabled = true;
            }
            settigns.ApplicationID = TXT_ApplicationID.Text;
            Btn_SaveAppID.IsEnabled = false;
            settigns.Save();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void TXT_ApplicationID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            Btn_SaveAppID.IsEnabled = !text.Equals(settigns.ApplicationID);
        }

        private async void TXT_Details_LostFocus(object sender, RoutedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            if (settigns.Details != text)
            {
                settigns.Details = text;
                settigns.Save();
            }

            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateDetails(text);
        }

        private async void TXT_State_LostFocus(object sender, RoutedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            if (settigns.State != text)
            {
                settigns.State = text;
                settigns.Save();
            }
            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateStates(text);
        }

        private async void TXT_LargeImgKey_LostFocus(object sender, RoutedEventArgs e)
        {
            if (settigns.LargeImgKey != TXT_LargeImgKey.Text || settigns.LargeImgText != TXT_LargeImgText.Text)
            {
                settigns.LargeImgKey = TXT_LargeImgKey.Text;
                settigns.LargeImgText = TXT_LargeImgText.Text;
                settigns.Save();
            }

            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateLargeAsset(TXT_LargeImgKey.Text, TXT_LargeImgText.Text);
        }

        private async void TXT_SmallImgKey_LostFocus(object sender, RoutedEventArgs e)
        {
            if (settigns.SmallImgKey != TXT_SmallImgKey.Text || settigns.SmallImgText != TXT_SmallImgText.Text)
            {
                settigns.SmallImgKey = TXT_SmallImgKey.Text;
                settigns.SmallImgText = TXT_SmallImgText.Text;
                settigns.Save();
            }

            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateSmallAsset(TXT_SmallImgKey.Text, TXT_SmallImgText.Text);
        }

        private async void TXT_Button1Label_LostFocus(object sender, RoutedEventArgs e)
        {
            if (buttons is null)
                return;
            try
            {
                if (TXT_Button1Label.Text.Length == 0 && TXT_Button1Url.Text.Length == 0)
                {
                    settigns.Button_1_Enable = CheckBox_Button1.IsChecked = false;
                    settigns.Button_1 = null;
                }
                else if (TXT_Button1Url.Text.Length > 4)
                    settigns.Button_1 = new Button { Label = TXT_Button1Label.Text, Url = TXT_Button1Url.Text };

                if ((settigns.Button_1_Enable = CheckBox_Button1.IsChecked).GetValueOrDefault() && TXT_Button1Url.Text.Length > 4)
                    buttons[0] = settigns.Button_1;
                else
                {
                    settigns.Button_1_Enable = CheckBox_Button1.IsChecked = false;
                    buttons[0] = null;
                }

                settigns.Save();

                if ((discordPresence?.IsInitialized).GetValueOrDefault())
                    await discordPresence.UpdateDefaultButtons(buttons.Where(x => x != null).ToArray());

            }
            catch (Exception ex)
            {
                settigns.Button_1_Enable = CheckBox_Button1.IsChecked = false;
                settigns.Save();
                MessageBox.Show(ex.Message, "Button 01", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void TXT_Button2Label_LostFocus(object sender, RoutedEventArgs e)
        {
            if (buttons is null)
                return;
            try
            {
                if (TXT_Button2Label.Text.Length == 0 && TXT_Button2Url.Text.Length == 0)
                {
                    settigns.Button_2_Enable = CheckBox_Button2.IsChecked = false;
                    settigns.Button_2 = null;
                }
                else if (TXT_Button2Url.Text.Length > 4)
                    settigns.Button_2 = new Button { Label = TXT_Button2Label.Text, Url = TXT_Button2Url.Text };

                if ((settigns.Button_2_Enable = CheckBox_Button2.IsChecked).GetValueOrDefault() && TXT_Button2Url.Text.Length > 4)
                    buttons[1] = settigns.Button_2;
                else
                {
                    settigns.Button_2_Enable = CheckBox_Button2.IsChecked = false;
                    buttons[1] = null;
                }

                settigns.Save();

                if ((discordPresence?.IsInitialized).GetValueOrDefault())
                    await discordPresence.UpdateDefaultButtons(buttons.Where(x => x != null).ToArray());
            }
            catch (Exception ex)
            {
                settigns.Button_2_Enable = CheckBox_Button2.IsChecked = false;
                settigns.Save();
                MessageBox.Show(ex.Message, "Button 02", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckBox_Button1_Checked(object sender, RoutedEventArgs e)
        {
            TXT_Button1Label.Focus();
            Btn_StartStop.Focus();
        }

        private void CheckBox_Button2_Unchecked(object sender, RoutedEventArgs e)
        {
            TXT_Button2Label.Focus();
            Btn_StartStop.Focus();
        }


        private async void CheckBox_Timestamps_Checked(object sender, RoutedEventArgs e)
        {
            settigns.ShowTimestamps = true;
            settigns.Save();
            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateTime(true);

        }

        private async void CheckBox_Timestamps_Unchecked(object sender, RoutedEventArgs e)
        {
            settigns.ShowTimestamps = false;
            settigns.Save();
            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateTime(false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsManager.Load(this, out settigns);
            TXT_ApplicationID.Focus();
            TXT_Button1Label.Focus();
            TXT_Button2Label.Focus();
            TXT_Details.Focus();
        }

        private void TXT_ApplicationID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TXT_ApplicationID.Text.Length > 5 && settigns.ApplicationID == TXT_ApplicationID.Text)
            {
                Btn_SaveAppID.IsEnabled = false;
                Btn_StartStop.IsEnabled = true;
            }
        }
        #endregion
    }
}
