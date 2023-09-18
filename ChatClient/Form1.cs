using System.Net;

using System.Net.Sockets;

using System.Text;



namespace ChatClient

{

    //Ŭ���̾�Ʈ�� �ؽ�Ʈ�ڽ��� ���� �������� ��������Ʈ

    //���� ���� ���°��� Form1Ŭ������ �����尡 �ƴ� �ٸ� �������� ChatHandler�� ������ �̱⿡

    //(���� ��Ʈ���� ���� �����尡 �ƴ� �ٸ� �����忡�� �ؽ�Ʈ�ڽ��� ���� ���ٸ� �����߻�)

    //ChatHandler�� �����忡�� �� ��������Ʈ�� ȣ���Ͽ� �������� �Ѿ���� �޽����� ����.

    delegate void SetTextDelegate(string s);



    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }



        TcpClient tcpClient = null;

        NetworkStream ntwStream = null;

        //������ ä���� ����

        ChatHandler chatHandler = new ChatHandler();



        //���� ��ư Ŭ��

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "����")
            {
                try
                {
                    tcpClient = new TcpClient();

                    tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 1103);

                    ntwStream = tcpClient.GetStream();



                    chatHandler.Setup(this, ntwStream, this.txtChatMsg);

                    Thread chatThread = new Thread(new ThreadStart(chatHandler.ChatProcess));

                    chatThread.Start(); 



                    Message_Snd("<" + txtName.Text + "> �Բ��� ����  .", true);

                    btnConnect.Text = "������";
                }
                catch (System.Exception Ex)
                {
                    MessageBox.Show("Server �����߻� �Ǵ� Start ���� �ʾҰų�\n\n" + Ex.Message, "Client");
                }
            }
            else
            {
                Message_Snd("<" + txtName.Text + "> �Բ��� �������� �ϼ̽��ϴ�.", false);

                btnConnect.Text = "����";

                chatHandler.ChatClose();

                ntwStream.Close();

                tcpClient.Close();
            }
        }



        private void Message_Snd(string lstMessage, Boolean Msg)
        {
            try
            {
                //���� �����͸� �о� Default ������ ����Ʈ �迭�� ��ȯ �ؼ� ���� 

                string dataToSend = lstMessage + "\r\n";

                byte[] data = Encoding.Default.GetBytes(dataToSend);

                ntwStream.Write(data, 0, data.Length);
            }
            catch (Exception Ex)
            {
                if (Msg == true)
                {
                    MessageBox.Show("������ Start ���� �ʾҰų�\n\n" + Ex.Message, "Client");

                    btnConnect.Text = "����";

                    chatHandler.ChatClose();

                    ntwStream.Close();

                    tcpClient.Close();
                }
            }
        }



        //�ٸ� �������� ChatHandler�� �����忡�� ȣ���ϴ� �Լ���

        //��������Ʈ�� ���� ä�� ���ڿ��� �ؽ�Ʈ�ڽ��� ��

        public void SetText(string text)
        {
            if (this.txtChatMsg.InvokeRequired)
            {
                SetTextDelegate d = new SetTextDelegate(SetText);

                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.txtChatMsg.AppendText(text);
            }
        }



        private void txtMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                //������ ������ �� ��쿡�� �޽����� ������  ����

                if (btnConnect.Text == "������")
                {
                    Message_Snd("<" + txtName.Text + "> " + txtMsg.Text, true);
                }
                txtMsg.Text = "";

                e.Handled = true;  //�̺�Ʈó������, KeyUp or Click��
            }
        }
    }



    public class ChatHandler
    {
        private TextBox txtChatMsg;

        private NetworkStream netStream;

        private StreamReader strReader;

        private Form1 form1;



        public void Setup(Form1 form1, NetworkStream netStream, TextBox txtChatMsg)
        {

            this.txtChatMsg = txtChatMsg;

            this.netStream = netStream;

            this.form1 = form1;

            this.netStream = netStream;

            this.strReader = new StreamReader(netStream);

        }



        public void ChatClose()
        {
            netStream.Close();

            strReader.Close();
        }



        public void ChatProcess()
        {
            while (true)
            {
                try
                {
                    //���ڿ��� ����

                    string lstMessage = strReader.ReadLine();

                    if (lstMessage != null && lstMessage != "")
                    {
                        //SetText �޼��忡�� ��������Ʈ�� �̿��Ͽ� �������� �Ѿ���� �޽����� ����.

                        form1.SetText(lstMessage + "\r\n");
                    }
                }
                catch (System.Exception)
                {
                    break;
                }
            }
        }
    }
}