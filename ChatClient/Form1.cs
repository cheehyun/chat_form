using System.Net;

using System.Net.Sockets;

using System.Text;



namespace ChatClient

{

    //클라이언트의 텍스트박스에 글을 쓰기위한 델리게이트

    //실제 글을 쓰는것은 Form1클래스의 쓰레드가 아닌 다른 스레드인 ChatHandler의 스레드 이기에

    //(만약 컨트롤을 만든 쓰레드가 아닌 다른 스레드에서 텍스트박스에 글을 쓴다면 에러발생)

    //ChatHandler의 스레드에서 이 델리게이트를 호출하여 서버에서 넘어오는 메시지를 쓴다.

    delegate void SetTextDelegate(string s);



    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }



        TcpClient tcpClient = null;

        NetworkStream ntwStream = null;

        //서버와 채팅을 실행

        ChatHandler chatHandler = new ChatHandler();



        //입장 버튼 클릭

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "입장")
            {
                try
                {
                    tcpClient = new TcpClient();

                    tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 1103);

                    ntwStream = tcpClient.GetStream();



                    chatHandler.Setup(this, ntwStream, this.txtChatMsg);

                    Thread chatThread = new Thread(new ThreadStart(chatHandler.ChatProcess));

                    chatThread.Start(); 



                    Message_Snd("<" + txtName.Text + "> 님께서 접속  .", true);

                    btnConnect.Text = "나가기";
                }
                catch (System.Exception Ex)
                {
                    MessageBox.Show("Server 오류발생 또는 Start 되지 않았거나\n\n" + Ex.Message, "Client");
                }
            }
            else
            {
                Message_Snd("<" + txtName.Text + "> 님께서 접속해제 하셨습니다.", false);

                btnConnect.Text = "입장";

                chatHandler.ChatClose();

                ntwStream.Close();

                tcpClient.Close();
            }
        }



        private void Message_Snd(string lstMessage, Boolean Msg)
        {
            try
            {
                //보낼 데이터를 읽어 Default 형식의 바이트 배열로 변환 해서 전송 

                string dataToSend = lstMessage + "\r\n";

                byte[] data = Encoding.Default.GetBytes(dataToSend);

                ntwStream.Write(data, 0, data.Length);
            }
            catch (Exception Ex)
            {
                if (Msg == true)
                {
                    MessageBox.Show("서버가 Start 되지 않았거나\n\n" + Ex.Message, "Client");

                    btnConnect.Text = "입장";

                    chatHandler.ChatClose();

                    ntwStream.Close();

                    tcpClient.Close();
                }
            }
        }



        //다른 스레드인 ChatHandler의 쓰레드에서 호출하는 함수로

        //델리게이트를 통해 채팅 문자열을 텍스트박스에 씀

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
                //서버에 접속이 된 경우에만 메시지를 서버로  보냄

                if (btnConnect.Text == "나가기")
                {
                    Message_Snd("<" + txtName.Text + "> " + txtMsg.Text, true);
                }
                txtMsg.Text = "";

                e.Handled = true;  //이벤트처리중지, KeyUp or Click등
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
                    //문자열을 받음

                    string lstMessage = strReader.ReadLine();

                    if (lstMessage != null && lstMessage != "")
                    {
                        //SetText 메서드에서 델리게이트를 이용하여 서버에서 넘어오는 메시지를 쓴다.

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