// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using System.IO.Ports;
using System.Text;


namespace Mercury.PandaSerial
{
    public class SerialComm
    {
        private string respostaMaquina = string.Empty;
        static bool _continue;
        static SerialPort _serialPort;
        SerialPort serialPort = new SerialPort();

        List<string> lstPortasCom;
        List<string> lstBaudRate;
        List<string> lstDataBit;
        List<string> lstCheckBit;
        List<string> lstStopBit;
        bool blnSendDataASCII = true;
        bool blnReceiveDataASCII = true;
        String saveDataFile = null;
        FileStream saveDataFS = null;

        public SerialComm()
        {
            //Instancia o objeto porta serial
            _serialPort = new System.IO.Ports.SerialPort();
            lstPortasCom = new List<string>();
            lstBaudRate = new List<string>();
            lstDataBit = new List<string>();
            lstCheckBit = new List<string>();
            lstStopBit = new List<string>();

            serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
        }

        //Inicializa as configurações dos parâmetros da interface da porta serial
        private void Init_Port_Confs()
        {
            /*------Configuração do parâmetro da interface da porta serial------*/

            //Verifica se existe porta serial
            string[] str = SerialPort.GetPortNames();
            if (str == null)
            {
                Console.WriteLine("Esta máquina não possui porta serial!", "Erro");
                return;
            }
            //adiciona porta serial
            foreach (string s in str)
            {
                lstPortasCom.Add(s);
            }
            

            /*------Configuração da taxa de transmissão-------*/
            string[] baudRate = { "9600", "19200", "38400", "57600", "115200" };
            foreach (string s in baudRate)
            {
                lstBaudRate.Add(s);
            }

            /*------configuração do bit de dados-------*/
            string[] dataBit = { "5", "6", "7", "8" };
            foreach (string s in dataBit)
            {
                lstDataBit.Add(s);
            }


            /*------verificar configuração de dígito-------*/
            string[] checkBit = { "None", "Even", "Odd", "Mask", "Space" };
            foreach (string s in checkBit)
            {
                lstCheckBit.Add(s);
            }

            /*------configuração do bit de parada-------*/
            string[] stopBit = { "1", "1.5", "2" };
            foreach (string s in stopBit)
            {
                lstStopBit.Add(s);
            }

            /*------configuração do formato de dados-------*/
            blnSendDataASCII = true;
            blnReceiveDataASCII = true;
        }

        //Abre a porta serial e fecha a porta serial
        public void OpenCloseCom(string portCOM)
        {
            Console.WriteLine($"configurando porta : {portCOM}");
            Init_Port_Confs();

            if (!serialPort.IsOpen)//A porta serial está fechada
            {
                Console.WriteLine($"Porta não esta aberta, definindo Configurações : {portCOM}");

                try
                {

                    if (lstPortasCom == null || lstPortasCom.Count <= 0)
                    {
                        Console.WriteLine("Erro: porta inválida, escolha novamente");
                        return;
                    }
                    string strSerialName = lstPortasCom[lstPortasCom.FindIndex(x => x.Contains(portCOM))];

                    string strBaudRate = lstBaudRate[0];
                    string strDataBit = lstDataBit[0];
                    string strCheckBit = lstCheckBit[0];
                    string strStopBit = lstStopBit[0];

                    Int32 iBaudRate = Convert.ToInt32(strBaudRate);
                    Int32 iDataBit = Convert.ToInt32(strDataBit);

                    serialPort.PortName = strSerialName;//número da porta serial
                    serialPort.BaudRate = iBaudRate;//taxa de transmissão
                    serialPort.DataBits = iDataBit;//bits de dados



                    switch (strStopBit)            //bit de parada
                    {
                        case "1":
                            serialPort.StopBits = StopBits.One;
                            break;
                        case "1.5":
                            serialPort.StopBits = StopBits.OnePointFive;
                            break;
                        case "2":
                            serialPort.StopBits = StopBits.Two;
                            break;
                        default:
                           Console.WriteLine("Error：O parâmetro stop bit está incorreto!", "Error");
                            break;
                    }
                    switch (strCheckBit) //verifica o bit
                    {
                        case "None":
                            serialPort.Parity = Parity.None;
                            break;
                        case "Odd":
                            serialPort.Parity = Parity.Odd;
                            break;
                        case "Even":
                            serialPort.Parity = Parity.Even;
                            break;
                        default:
                           Console.WriteLine("Error：Parâmetro de dígito de verificação incorreto!", "Error");
                            break;
                    }



                    //if (saveDataFile != null)
                    //{
                    //    saveDataFS = File.Create(saveDataFile);
                    //}

                    //abre a porta serial

                    Console.WriteLine($"comando abre a porta serial : {portCOM}");

                    serialPort.Open();
                    Console.WriteLine($"Porta aberta com SUCESSO : {portCOM}");
                    var res = SendData("#n010.");
                    Console.WriteLine("RES" + res);
                    //As configurações não serão mais válidas após a abertura da porta serial
                    //comboBoxCom.Enabled = false;
                    //comboBoxBaudRate.Enabled = false;
                    //comboBoxDataBit.Enabled = false;
                    //comboBoxCheckBit.Enabled = false;
                    //comboBoxStopBit.Enabled = false;
                    //radioButtonSendDataASCII.Enabled = false;
                    //radioButtonSendDataHex.Enabled = false;
                    //radioButtonReceiveDataASCII.Enabled = false;
                    //radioButtonReceiveDataHEX.Enabled = false;
                    //buttonSendData.Enabled = true;
                    //Button_Refresh.Enabled = false;

                    //buttonOpenCloseCom.Text = "Fechar a porta serial";
                    //buttonOpenCloseCom.BackColor = Color.Red;
                }
                catch (System.Exception ex)
                {
                   Console.WriteLine("Error:" + ex.Message, "Error");
                    return;
                }
            }
            else //A porta serial está aberta
            {
                Console.WriteLine($"Porta serial já esta aberta : {portCOM}");

                serialPort.Close();//  Fecha a porta serial 
                //A configuração é válida quando a porta serial está fechada
                //comboBoxCom.Enabled = true;
                //comboBoxBaudRate.Enabled = true;
                //comboBoxDataBit.Enabled = true;
                //comboBoxCheckBit.Enabled = true;
                //comboBoxStopBit.Enabled = true;
                //radioButtonSendDataASCII.Enabled = true;
                //radioButtonSendDataHex.Enabled = true;
                //radioButtonReceiveDataASCII.Enabled = true;
                //radioButtonReceiveDataHEX.Enabled = true;
                //buttonSendData.Enabled = false;
                //Button_Refresh.Enabled = true;

                //buttonOpenCloseCom.Text = "porta serial aberta";
                //buttonOpenCloseCom.BackColor = Color.Lime;


                //if (saveDataFS != null)
                //{
                //    saveDataFS.Close(); // fecha o arquivo
                //    saveDataFS = null;//Libera o identificador de arquivo
                //}

            }
        }

        //receber dados
        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine($"Evento Data Received");

            if (serialPort.IsOpen)
            {
                //MessageBox.Show("sss","OK");
                //tempo atual de saída
                DateTime dateTimeNow = DateTime.Now;
                //dateTimeNow.GetDateTimeFormats(); 
                //dateTimeNow.GetDateTimeFormats('f')[0].ToString() + "\r\n"; 

                if (blnReceiveDataASCII == true) //O formato de recebimento é ASCII
                {
                    try
                    {
                        Console.WriteLine($"Leitura do Retorno");

                        String input = serialPort.ReadExisting();

                        Console.WriteLine($"Retorno da Leitura => {input}");

                        respostaMaquina = input.TrimEnd('\0') + "sucesso";
                        //String input = serialPort.ReadLine(); 

                        //richTextBox1.ForeColor = Color.Red;
                        //richTextBox1.AppendText(string.Format("{0} - [OUT] -> {1}\r\n", DateTime.Now, input));

                        // save data to file
                        if (saveDataFS != null)
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(input + "\r\n");
                            saveDataFS.Write(info, 0, info.Length);
                        }
                    }
                    catch (System.Exception ex)
                    {
                       Console.WriteLine(ex.Message, "Há algum problema com sua taxa de transmissão?？？？");
                        return;
                    }

                    //textBoxReceive.SelectionStart = textBoxReceive.Text.Length;
                    //textBoxReceive.ScrollToCaret();//role para o cursor

                    //richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    //richTextBox1.ScrollToCaret();

                    serialPort.DiscardInBuffer(); //Limpe o Buffer do controle SerialPort 
                }
                else //O formato de recebimento é HEX
                {
                    try
                    {

                        string input = serialPort.ReadLine();
                        char[] values = input.ToCharArray();
                        foreach (char letter in values)
                        {
                            // Get the integral value of the character.
                            int value = Convert.ToInt32(letter);
                            // Convert the decimal value to a hexadecimal value in string form.
                            string hexOutput = String.Format("{0:X}", value);
                            //richTextBox1.AppendText(hexOutput + " ");
                            //richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            //richTextBox1.ScrollToCaret();//role para o cursor
                            //textBoxReceive.Text += hexOutput + " ";

                        }

                        // save data to file
                        if (saveDataFS != null)
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(input + "\r\n");
                            saveDataFS.Write(info, 0, info.Length);
                        }


                    }
                    catch (System.Exception ex)
                    {
                       Console.WriteLine(ex.Message, "Error");
                        //richTextBox1.Text = "";//vazio
                    }
                }
            }
            else
            {
               Console.WriteLine("Abra uma porta serial", "Prompt de erro");
            }
        }

        //enviar dados
        public string SendData(string comando)
        {
            return SendDataBySerial(comando);

        }

        public string SendDataBySerial(string strComando)
        {
            if (!serialPort.IsOpen)
            {
               Console.WriteLine("Por favor, abra a porta serial primeiro", "Error");
                return "";
            }

            //String strSend = ("#" + textBoxSend.Text + ".").Trim(); //enviar dados da caixa
            String strSend = string.Format("#{0}.", strComando);

            if (blnSendDataASCII == true)//enviar como string ASCII
            {
                serialPort.WriteLine(strSend);//enviar uma linha de dados 
                                              //richTextBox1.ForeColor = Color.Blue;
                                              //richTextBox1.AppendText(string.Format("{0} - [IN ] -> {1} \r\n", DateTime.Now, strSend));


                Thread.Sleep(5000);

                return respostaMaquina;

                //return "SUCESSO";

            }
            else
            {
                //Envio de formato de dados hexadecimais HXE

                char[] values = strSend.ToCharArray();
                foreach (char letter in values)
                {
                    // Get the integral value of the character.
                    int value = Convert.ToInt32(letter);
                    // Convert the decimal value to a hexadecimal value in string form.
                    string hexIutput = String.Format("{0:X}", value);
                    serialPort.WriteLine(hexIutput);

                }

                return respostaMaquina;

            }
        }

    }
}