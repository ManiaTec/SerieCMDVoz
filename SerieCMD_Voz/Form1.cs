using Microsoft.Speech.Recognition;
using System;
using System.IO.Ports;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace SerieCMD_Voz
{
    public partial class Form1 : Form
    {
        //Configura o sitema de comando de voz
        private SpeechRecognitionEngine sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pt-BR"));
        //Reconhecendo ?
        private bool ligado = false;
        //COM Port selecionada
        private String compt;
        public Form1()
        {
            InitializeComponent();
            //Pega as portas serial disponiveis e as coloca na Combobox
            string[] ports = SerialPort.GetPortNames();
            comboBox1.DataSource = ports;
            //Informa as palavreas a serem reconhecidas e configura a gramática
            Choices Palavras_Rec = new Choices();
            Palavras_Rec.Add(new string[] {"Oi", "Ligar luz", "Desligar luz" });
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(Palavras_Rec);
            Grammar g = new Grammar(gb);
            sre.LoadGrammar(g);
            //Informa a função que recebe a palavra reconhecida
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(quando_reconhecer);
            //Faz com que use o microfone padrão
            sre.SetInputToDefaultAudioDevice();
        }
        //Quando reconhecer ↓
        void quando_reconhecer(object sender, SpeechRecognizedEventArgs e)
        {
            //Só concidera quando tiver confiança de 70% quado reconhecer
            if (e.Result.Confidence > (Convert.ToDouble(70)/100)) {
                //Inicia uma tentativa de executar oque foi solicitado por voz
                try
                {
                    //Passa o texto reconhecido para variável
                    var resp = e.Result.Text;
                    //Se o texto for "Oi" ↓
                    if (resp == "Oi") {
                        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
                        //Volume de resposta
                        synthesizer.Volume = 100;  // 0...100
                        //Velocidade de resposta
                        synthesizer.Rate = 0;     // -10...10
                        //Oque será dito
                        synthesizer.Speak("Olá");
                    }
                    else /*Se o texto for "Ligar luz" →*/ if (resp == "Ligar luz") {
                        //Seleciona a porta COM
                        SerialPort port = new SerialPort(compt, 9600);
                        //Tenta enviar o comando por serial
                        try
                        {
                            port.Open();
                            port.Write("ligar");
                            //Foi
                        }
                        catch (Exception ex)
                        {
                            //Erro
                            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
                            synthesizer.Volume = 100;  // 0...100
                            synthesizer.Rate = 0;     // -10...10
                            synthesizer.Speak("Erro COM");
                            MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        port.Close();
                    }
                }
                catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Verifica se está reconhecendo ou não
            if (!ligado)
            {
                //Estava desligado e vai ser ligado 
                compt = comboBox1.Text;
                button1.Text = "On";
                ligado = true;
                sre.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                //Estava ligado vai ser desligado
                button1.Text = "Off";
                ligado = false;
                sre.RecognizeAsyncStop();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://maniatecja.tk/");
        }
    }
}
