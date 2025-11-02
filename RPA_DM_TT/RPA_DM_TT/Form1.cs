using CefSharp;
using CefSharp.WinForms;
using System;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using Timer = System.Windows.Forms.Timer;

namespace RPA_DM_TT
{
    public partial class Form1 : Form
    {
        private Timer tmrDMTwitter; //Temporizador (timer) que vai disparar eventos
        private ChromiumWebBrowser browser; //Navegador baseado no Chromium

        public Form1()
        {
            InitializeComponent(); //Inicializa os componentes visuais
            InicializaChromiun();

            tmrDMTwitter = new Timer(); //Cria o temporizador
            tmrDMTwitter.Interval = 1000; //Timer vai ser disparado a cada 1 segundo
            tmrDMTwitter.Tick += new EventHandler(tmrDMTwitter_Tick); //Associa o evento Tick ao método tmrDMTwitter_Tick
        }

        public void InicializaChromiun()
        {
            CefSettings configs = new CefSettings(); //Configurações do CefSharp
            Cef.Initialize(configs);

            browser = new ChromiumWebBrowser("https://twitter.com");
            browser.Dock = DockStyle.Fill; //Preenche todo o espaço da janela
            this.Controls.Add(browser); //Adiciona o navegador a interface do formulário
            browser.FrameLoadEnd += VerificaBrowserCarregou;
        }

        private void VerificaBrowserCarregou(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                tmrDMTwitter.Start(); //Só inicia o timer após o carregamento completo da página
                tmrDMTwitter.Interval = 5000; //Delay de 5 segundos para garantir que todos os elementos estejam carregados
            }
        }

        private void tmrDMTwitter_Tick(object sender, EventArgs e)
        {
            browser.ExecuteScriptAsync(@"document.querySelector('[data-testid=""AppTabBar_DirectMessage_Link""]').click();"); //Clica no botão de DMs
            browser.ExecuteScriptAsync(@"document.querySelector('[data-testid=""conversation""]').click()"); //Clica na primeira DM
            browser.ExecuteScriptAsync(@"document.querySelector('[data-testid=""dmComposerTextInput""]').click()"); //Clica na caixa de texto
            browser.ExecuteScriptAsync("document.execCommand('insertText', false, 'Ola Mundo!');"); //Escreve na caixa de texto
            browser.ExecuteScriptAsync(@"document.querySelector('[data-testid=""dmComposerSendButton""]').click()"); //Envia a mensagem
        }
    }
}
