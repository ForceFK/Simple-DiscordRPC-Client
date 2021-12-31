/*
 * Arquivo: MainWindow.xaml.cs
 * Criado em: 24-12-2021
 * https://github.com/ForceFK
 * Última modificação: 30-12-2021
 */
using DiscordRPC.Tasks;
using System;
using System.Collections.Generic;
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
        private string _applicationID;
        private List<Button> buttons;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Botão Iniciar/Parar
        /// </summary>
        private async void Btn_StartClose_Click(object sender, RoutedEventArgs e)
        {
            Btn_StartClose.IsEnabled = false;

            //Se o cliente já está inicializado, vamos parar ou poderá ocorrer erros caso iniciar mais de um
            if (discordPresence != null && discordPresence.IsInitialized)
            {
                await discordPresence.Stop();

                Btn_StartClose.Content = "Iniciar";
                Grid_Head.IsEnabled = true;
            }
            else
            {
                //Vamos inicar uma instancia do DiscordPresenceTask passando o applicationID
                discordPresence = new DiscordPresenceTask(ulong.Parse(_applicationID));

                //Incializar o cliente DiscordRPC para se conectar ao discord.
                await discordPresence.Start();

                Btn_StartClose.Content = "Parar";
                Grid_Head.IsEnabled = false;

                //Maximo de botões no presence é 2,
                //então alocamos uma lista com espaços vazios para povoar/editar posteriormente
                buttons = new List<Button> { null, null };


                //Ver se os botões estão ativos, se sim, criamos o modelo Button com os valores dos textbox

                //botão 1
                if (CheckBox_Button1.IsChecked.GetValueOrDefault())
                    buttons[0] = new Button { Label = TXT_Button1Label.Text, Url = TXT_Button1Url.Text };

                //Botão 2
                if (CheckBox_Button2.IsChecked.GetValueOrDefault())
                    buttons[1] = new Button { Label = TXT_Button2Label.Text, Url = TXT_Button2Url.Text };


                //Criamos o presence com os label e definimos no cliente
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
                    //Pegamos a lista de botões, ignorando os valores nulos e convertemos em array
                    Buttons = buttons.TakeWhile(x => x != null).ToArray(),

                    //Tempo crescente no presence
                    Timestamps = Timestamps.Now
                });


            }
            Btn_StartClose.Focus();
            await Task.Delay(20);
            Btn_StartClose.IsEnabled = true;
        }


        /// <summary>
        /// Desconectar o clienteRPC no fechamento do form
        /// </summary>
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await discordPresence?.Stop();
        }

        #region Evento de componentes do form sem muita importancia

        private void Btn_SaveAppID_Click(object sender, RoutedEventArgs e)
        {
            if (TXT_ApplicationID.Text.Length <= 5)
            {
                MessageBox.Show("Application ID inválido!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Btn_SaveAppID.IsEnabled = false;
            _applicationID = TXT_ApplicationID.Text;

            Btn_StartClose.IsEnabled = true;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void TXT_ApplicationID_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            Btn_SaveAppID.IsEnabled = !text.Equals(_applicationID);
        }

        private async void TXT_Details_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateDetails((sender as TextBox).Text);
        }

        private async void TXT_State_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateStates((sender as TextBox).Text);
        }

        private async void TXT_LargeImgKey_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateLargeAsset(TXT_LargeImgKey.Text, TXT_LargeImgText.Text);
        }

        private async void TXT_SmallImgKey_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((discordPresence?.IsInitialized).GetValueOrDefault())
                await discordPresence.UpdateSmallAsset(TXT_SmallImgKey.Text, TXT_SmallImgText.Text);
        }

        private async void TXT_Button1Label_LostFocus(object sender, RoutedEventArgs e)
        {
            if (buttons is null)
                return;
            try
            {
                if (CheckBox_Button1.IsChecked.GetValueOrDefault() && TXT_Button1Url.Text.Length > 4)
                    buttons[0] = new Button { Label = TXT_Button1Label.Text, Url = TXT_Button1Url.Text };
                else
                    buttons[0] = null;

                if ((discordPresence?.IsInitialized).GetValueOrDefault())
                    await discordPresence.UpdateDefaultButtons(buttons.TakeWhile(x => x != null).ToArray());
            }
            catch (Exception ex)
            {
                CheckBox_Button1.IsChecked = false;
                MessageBox.Show(ex.Message, "Botão 01", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void TXT_Button2Label_LostFocus(object sender, RoutedEventArgs e)
        {
            if (buttons is null)
                return;
            try
            {
                if (CheckBox_Button2.IsChecked.GetValueOrDefault() && TXT_Button2Url.Text.Length > 4)
                    buttons[1] = new Button { Label = TXT_Button2Label.Text, Url = TXT_Button2Url.Text };
                else
                    buttons[1] = null;

                if ((discordPresence?.IsInitialized).GetValueOrDefault())
                    await discordPresence.UpdateDefaultButtons(buttons.TakeWhile(x => x != null).ToArray());
            }
            catch (Exception ex)
            {
                CheckBox_Button2.IsChecked = false;
                MessageBox.Show(ex.Message, "Botão 02", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckBox_Button1_Checked(object sender, RoutedEventArgs e)
        {
            TXT_Button1Label.Focus();
            Btn_StartClose.Focus();
        }

        private void CheckBox_Button2_Unchecked(object sender, RoutedEventArgs e)
        {
            TXT_Button2Label.Focus();
            Btn_StartClose.Focus();
        }
        #endregion
    }
}
