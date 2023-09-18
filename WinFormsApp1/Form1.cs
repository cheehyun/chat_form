using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;



namespace WinFormsApp1
{
    //서버의 txtChatMsg 텍스트박스에 글을 쓰기위한 델리게이트

    //실제 글을 쓰는것은 Form1클래스의 UI쓰레드가 아닌 다른 스레드인 ClientHandler의 스레드 이기에        

    //ClientHandler의 스레드에서 이 델리게이트를 호출하여 텍스트 박스에 글을 쓴다.

    //(만약 컨트롤을 만든 윈폼의 UI쓰레드가 아닌 다른 스레드에서 텍스트박스에 글을 쓴다면 에러발생)

    delegate void SetTextDelegate(string s);



    public partial class Form1 : Form
    {
        TcpListener chatServer = new TcpListener(IPAddress.Parse("127.0.0.1"), 1103);

        public static ArrayList clientSocketArray = new ArrayList();



        public Form1()
        {
            InitializeComponent();
        }



        //서버 시작/종료 클릭

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                // 현재 서버가 종료 상태인 경우
                if (lblMsg.Tag.ToString() == "Stop")
                {
                    //채팅서버 시작

                    chatServer.Start();

                    //계속 떠 있으면서 클라이언트의 연결을 기다리는 쓰레드 생성

                    //이 스레드가 실행하는 메소드에서 클라이언트 연결을 받고

                    //생성된 클라이언트 소켓을 clientSocketArray에 담고 새로운 쓰레드를 만들어

                    //접속된 클라이언트 전용으로 채팅을 한다.

                    Thread waitThread = new Thread(new ThreadStart(AcceptClient));

                    waitThread.Start();



                    lblMsg.Text = "Server 시작됨";

                    lblMsg.Tag = "Start";

                    btnStart.Text = "서버 종료";
                }
                else
                {
                    chatServer.Stop();

                    foreach (Socket soket in Form1.clientSocketArray)
                    {
                        soket.Close();
                    }

                    clientSocketArray.Clear();

                    lblMsg.Text = "Server 중지됨";

                    lblMsg.Tag = "Stop";

                    btnStart.Text = "서버 시작";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("서버 시작 오류 :" + ex.Message);
            }
        }



        //윈도우 창을 닫을 때

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            Application.Exit();

            chatServer.Stop();

        }



        //무한루프로 떠 있으면서 클라이언트 접속을 기다린다.

        private void AcceptClient()
        {
            Socket socketClient = null;

            while (true)
            {
                //Socket 생성 및 연결 대기
                try
                {
                    //연결을 기다리다가 클라이언트가 접속하면 AcceptSocket 메서드가 실행되어

                    //클라이언트와 상대할 소켓을 리턴 받는다.

                    socketClient = chatServer.AcceptSocket();



                    //Chatting을 실행하는 ClientHandler 인스턴스화시키고

                    //접속한 클라이언트 접속 소켓을 할당

                    ClientHandler clientHandler = new ClientHandler();

                    clientHandler.ClientHandler_Setup(this, socketClient, this.txtChatMsg);

                    //클라이언트를 상대하면서 채팅을 수행하는 스레드 생성 후 시작

                    Thread thd_ChatProcess = new Thread(new ThreadStart(clientHandler.Chat_Process));

                    thd_ChatProcess.Start();
                }
                catch (System.Exception)
                {
                    Form1.clientSocketArray.Remove(socketClient); break;
                }
            }
        }



        //텍스트박스에 대화내용을 쓰는 메소드 

        public void SetText(string text)
        {
            //t.InvokeRequired가 True를 반환하면 

            // Invoke 메소드 호출을 필요로 하는 상태고 즉 현재 스레드가 UI스레드가 아님

            //  이때 Invoke를 시키면 UI스레드가 델리게이트에 설정된 메소드를 실행해준다.

            //  False를 반환하면 UI스레드가 좁근하는 경우로 컨트롤에 직접 접근해도 문제가 없는 상태다.

            if (this.txtChatMsg.InvokeRequired)
            {
                SetTextDelegate d = new SetTextDelegate(SetText);  //델리게이트 선언

                this.Invoke(d, new object[] { text });//델리게이트를 통해 글을 쓴다.

                //이경우 UI스레드를 통해 SetText를 호출함
            }
            else
            {
                this.txtChatMsg.AppendText(text);  //텍스트박스에 글을 씀
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
            this.txtChatMsg = txtChatMsg;   //채팅 메시지 출력을 위한 TextBox

            this.socketClient = socketClient; //클라이언트 접속소켓, 이를 통해 스트림을 만들어 채팅한다.

            this.netStream = new NetworkStream(socketClient);

            Form1.clientSocketArray.Add(socketClient); //클라이언트 접속소켓을 List에 담음

            this.strReader = new StreamReader(netStream);

            this.form1 = form1;

        }



        public void Chat_Process()
        {
            while (true)
            {
                try
                {
                    //문자열을 받음

                    string lstMessage = strReader.ReadLine();

                    if (lstMessage != null && lstMessage != "")
                    {
                        //Form1클래스의 SetText메소드를 호출

                        //SetText에서는 델리게이트를 통해 TextBox에 글을 쓴다.

                        //직접 다른 쓰레드의 TextBox에 값을 쓰면 오류 발생 : Cross-thread operation not valid

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
                    MessageBox.Show("채팅 오류 : " + ex.ToString());

                    Form1.clientSocketArray.Remove(socketClient);

                    break;
                }
            }
        }
    }
}