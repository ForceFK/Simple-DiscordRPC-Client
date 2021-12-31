/*
 * Arquivo: DiscordPresenceTask.cs
 * Criado em: 24-12-2021
 * https://github.com/ForceFK
 * Última modificação: 30-12-2021
 */
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DiscordRPC.Tasks
{
    
    //Para mais informações, acesse: https://github.com/Lachee/discord-rpc-csharp
    public class DiscordPresenceTask
    {
        internal DiscordRpcClient ClientRPC;
        private RichPresence CurrentPresence;
        internal ulong ApplicationID;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="applicationID">ID do aplicativo obtido em https://discord.com/developers/applications </param>
        public DiscordPresenceTask(ulong applicationID)
        {
            ApplicationID = applicationID;
            ClientRPC = new DiscordRpcClient(ApplicationID.ToString());
        }

        /// <summary>
        /// Iniciar conexão com o discord
        /// </summary>
        /// <returns></returns>
        internal Task Start()
        {
            //thread safe
            lock (ClientRPC)
            {
                if (ApplicationID <= 0)
                {
                    throw new InvalidOperationException("ApplicationID e invalido.");
                }


                if(ClientRPC.IsDisposed)
                    ClientRPC = new DiscordRpcClient(ApplicationID.ToString());

                
                // Initialize
                if (!ClientRPC.Initialize())
                {
                    throw new Exception("ClientRPC.Initialize error.");
                }

            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Desconecta o cliente RPC e libera os recursos usados
        /// </summary>
        /// <returns></returns>
        internal Task Stop()
        {
            try
            {
                if (ClientRPC != null)
                {
                    lock (ClientRPC)
                    {
                        ClientRPC.ClearPresence();
                        ClientRPC.Dispose();
                    }
                }
            }
            catch
            {
                //Ignorado
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Defina um novo RichPresence, se nenhum for especificado atualize com o presence atual
        /// </summary>
        /// <param name="richPresence">Novo RichPresence</param>
        /// <returns></returns>
        internal Task SetPresence([Optional] RichPresence richPresence)
        {
            CurrentPresence = richPresence ?? CurrentPresence;

            if (CurrentPresence != null && IsInitialized)
            {
                ClientRPC?.SetPresence(CurrentPresence);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Limpa o RichPresence atual
        /// </summary>
        /// <returns></returns>
        internal Task CleanPresence()
        {
            if (CurrentPresence != null)
            {
                if (CurrentPresence != null)
                {
                    lock (CurrentPresence)
                    {
                        CurrentPresence = null;
                        ClientRPC.ClearPresence();
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Atualiza os botões do RichPresence
        /// </summary>
        /// <param name="btns">Novos botões, não pode conter valores null</param>
        /// <returns></returns>
        internal Task UpdateDefaultButtons(Button[] btns)
        {
            lock (CurrentPresence)
            {
                CurrentPresence.Buttons = btns;
            }

            return SetPresence();
        }

        internal Task UpdateLargeAsset(string key, string text)
        {
            if (ClientRPC != null && !ClientRPC.IsDisposed && ClientRPC.IsInitialized)
            {
                lock (ClientRPC)
                {
                    CurrentPresence = ClientRPC?.UpdateLargeAsset(key, text);
                }
            }

            return Task.CompletedTask;
        }

        internal Task UpdateSmallAsset(string key, string text)
        {
            if (ClientRPC != null && !ClientRPC.IsDisposed && ClientRPC.IsInitialized)
            {
                lock (ClientRPC)
                {
                    CurrentPresence = ClientRPC?.UpdateSmallAsset(key, text);
                }
            }

            return Task.CompletedTask;
        }

        internal Task UpdateDetails(string value)
        {
            if (ClientRPC != null && !ClientRPC.IsDisposed && ClientRPC.IsInitialized)
            {
                lock (ClientRPC)
                {
                    CurrentPresence = ClientRPC?.UpdateDetails(value);
                }
            }

            return Task.CompletedTask;
        }

        internal Task UpdateStates(string value)
        {
            if (ClientRPC != null && !ClientRPC.IsDisposed && ClientRPC.IsInitialized)
            {
                lock (ClientRPC)
                {
                    CurrentPresence = ClientRPC?.UpdateState(value);
                }
            }

            return Task.CompletedTask;
        }

        internal bool IsInitialized => !ClientRPC.IsDisposed && ClientRPC.IsInitialized;
    }
}
