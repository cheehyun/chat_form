using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;



namespace WinFormsApp1
{
    //������ txtChatMsg �ؽ�Ʈ�ڽ��� ���� �������� ��������Ʈ

    //���� ���� ���°��� Form1Ŭ������ UI�����尡 �ƴ� �ٸ� �������� ClientHandler�� ������ �̱⿡        

    //ClientHandler�� �����忡�� �� ��������Ʈ�� ȣ���Ͽ� �ؽ�Ʈ �ڽ��� ���� ����.

    //(���� ��Ʈ���� ���� ������ UI�����尡 �ƴ� �ٸ� �����忡�� �ؽ�Ʈ�ڽ��� ���� ���ٸ� �����߻�)

    delegate void SetTextDelegate(string s);



    public partial class Form1 : Form
    {
        TcpListener chatServer = new TcpListener(IPAddress.Parse("127.0.0.1"), 1103);

        public static ArrayList clientSocketArray = new ArrayList();



        public Form1()
        {
            InitializeComponent();
        }



        //���� ����/���� Ŭ��

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                // ���� ������ ���� ������ ���
                if (lblMsg.Tag.ToString() == "Stop")
                {
                    //ä�ü��� ����

                    chatServer.Start();

                    //��� �� �����鼭 Ŭ���̾�Ʈ�� ������ ��ٸ��� ������ ����

                    //�� �����尡 �����ϴ� �޼ҵ忡�� Ŭ���̾�Ʈ ������ �ް�

                    //������ Ŭ���̾�Ʈ ������ clientSocketArray�� ��� ���ο� �����带 �����

                    //���ӵ� Ŭ���̾�Ʈ �������� ä���� �Ѵ�.

                    Thread waitThread = new Thread(new ThreadStart(AcceptClient));

                    waitThread.Start();



                    lblMsg.Text = "Server ���۵�";

                    lblMsg.Tag = "Start";

                    btnStart.Text = "���� ����";
                }
                else
                {
                    chatServer.Stop();

                    foreach (Socket soket in Form1.clientSocketArray)
                    {
                        soket.Close();
                    }

                    clientSocketArray.Clear();

                    lblMsg.Text = "Server ������";

                    lblMsg.Tag = "Stop";

                    btnStart.Text = "���� ����";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("���� ���� ���� :" + ex.Message);
            }
        }



        //������ â�� ���� ��

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            Application.Exit();

            chatServer.Stop();

        }



        //���ѷ����� �� �����鼭 Ŭ���̾�Ʈ ������ ��ٸ���.

        private void AcceptClient()
        {
            Socket socketClient = null;

            while (true)
            {
                //Socket ���� �� ���� ���
                try
                {
                    //������ ��ٸ��ٰ� Ŭ���̾�Ʈ�� �����ϸ� AcceptSocket �޼��尡 ����Ǿ�

                    //Ŭ���̾�Ʈ�� ����� ������ ���� �޴´�.

                    socketClient = chatServer.AcceptSocket();



                    //Chatting�� �����ϴ� ClientHandler �ν��Ͻ�ȭ��Ű��

                    //������ Ŭ���̾�Ʈ ���� ������ �Ҵ�

                    ClientHandler clientHandler = new ClientHandler();

                    clientHandler.ClientHandler_Setup(this, socketClient, this.txtChatMsg);

                    //Ŭ���̾�Ʈ�� ����ϸ鼭 ä���� �����ϴ� ������ ���� �� ����

                    Thread thd_ChatProcess = new Thread(new ThreadStart(clientHandler.Chat_Process));

                    thd_ChatProcess.Start();
                }
                catch (System.Exception)
                {
                    Form1.clientSocketArray.Remove(socketClient); break;
                }
            }
        }



        //�ؽ�Ʈ�ڽ��� ��ȭ������ ���� �޼ҵ� 

        public void SetText(string text)
        {
            //t.InvokeRequired�� True�� ��ȯ�ϸ� 

            // Invoke �޼ҵ� ȣ���� �ʿ�� �ϴ� ���°� �� ���� �����尡 UI�����尡 �ƴ�

            //  �̶� Invoke�� ��Ű�� UI�����尡 ��������Ʈ�� ������ �޼ҵ带 �������ش�.

            //  False�� ��ȯ�ϸ� UI�����尡 �����ϴ� ���� ��Ʈ�ѿ� ���� �����ص� ������ ���� ���´�.

            if (this.txtChatMsg.InvokeRequired)
            {
                SetTextDelegate d = new SetTextDelegate(SetText);  //��������Ʈ ����

                this.Invoke(d, new object[] { text });//��������Ʈ�� ���� ���� ����.

                //�̰�� UI�����带 ���� SetText�� ȣ����
            }
            else
            {
                this.txtChatMsg.AppendText(text);  //�ؽ�Ʈ�ڽ��� ���� ��
            }
        }
    }



    public class ClientHandler
    {
        private TextBox txtChatMsg;

        private Socket socketClient;

        private NetworkStream netStream;

        private StreamReader strReader;

        private Form1 form1;



        public void ClientHandler_Setup(Form1 form1, Socket socketClient, TextBox txtChatMsg)
        {
            this.txtChatMsg = txtChatMsg;   //ä�� �޽��� ����� ���� TextBox

            this.socketClient = socketClient; //Ŭ���̾�Ʈ ���Ӽ���, �̸� ���� ��Ʈ���� ����� ä���Ѵ�.

            this.netStream = new NetworkStream(socketClient);

            Form1.clientSocketArray.Add(socketClient); //Ŭ���̾�Ʈ ���Ӽ����� List�� ����

            this.strReader = new StreamReader(netStream);

            this.form1 = form1;

        }



        public void Chat_Process()
        {
            while (true)
            {
                try
                {
                    //���ڿ��� ����

                    string lstMessage = strReader.ReadLine();

                    if (lstMessage != null && lstMessage != "")
                    {
                        //Form1Ŭ������ SetText�޼ҵ带 ȣ��

                        //SetText������ ��������Ʈ�� ���� TextBox�� ���� ����.

                        //���� �ٸ� �������� TextBox�� ���� ���� ���� �߻� : Cross-thread operation not valid

                        form1.SetText(lstMessage + "\r\n");



                        byte[] bytSand_Data = Encoding.Default.GetBytes(lstMessage + "\r\n");

                        lock (Form1.clientSocketArray)
                        {
                            foreach (Socket soket in Form1.clientSocketArray)
                            {
                                NetworkStream stream = new NetworkStream(soket);

                                stream.Write(bytSand_Data, 0, bytSand_Data.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ä�� ���� : " + ex.ToString());

                    Form1.clientSocketArray.Remove(socketClient);

                    break;
                }
            }
        }
    }
}