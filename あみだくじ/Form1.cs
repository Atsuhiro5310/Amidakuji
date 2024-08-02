using System.Data;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace あみだくじ
{
    public partial class Form1 : Form
    {

        private System.Windows.Forms.Timer timer;

        private int cntYoko = 6;

        private int cntTate = 2;

        /// <summary>ボール座標</summary>
        private List<DtoBallPoint> lstBalls = new List<DtoBallPoint>();
        private List<Color> colors = new List<Color>();

        private List<TextBox> lstTxts = new List<TextBox>();
        private List<Label> lstLbls = new List<Label>();

        private List<DtoPoint> lstYoko = new List<DtoPoint>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            Pen blackPen = new Pen(Color.Black, 2);

            for (int i = 0; i < cntTate; i++)
            {
                g.DrawLine(blackPen, 600 / (cntTate + 1) * (i + 1), 50, 600 / (cntTate + 1) * (i + 1), 450);
            }

            foreach (var yoko in lstYoko)
            {
                for (int i = 0; i < cntTate - 1; i++)
                {
                    if (yoko.FirstPoint.X == 600 / (cntTate + 1) * (i + 1))
                    {
                        g.DrawLine(blackPen, 600 / (cntTate + 1) * (i + 1), yoko.FirstPoint.Y, 600 / (cntTate + 1) * (i + 2), yoko.SecondPoint.Y);
                        break;
                    }
                }
            }

            //ボール
            for (int i = 0; i < cntTate; i++)
            {
                DrawBall(g, i);
            }


        }

        private void DrawBall(Graphics g, int cntNum)
        {
            SolidBrush brush = new SolidBrush(colors[cntNum]);
            Rectangle rect = new Rectangle(new Point(lstBalls[cntNum].X - 5, lstBalls[cntNum].Y - 5), new Size(10, 10));
            g.FillEllipse(brush, rect);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var ball in lstBalls)
            {
                if (ball.Y != 50)
                {
                    if (ball.BallDirection == 0)
                    {
                        foreach (DtoPoint dtoPoint in lstYoko)
                        {
                            if (dtoPoint.FirstPoint == new Point(ball.X, ball.Y))
                            {
                                ball.BallDirection = 1;
                                break;
                            }
                            else if (dtoPoint.SecondPoint == new Point(ball.X, ball.Y))
                            {
                                ball.BallDirection = 2;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (DtoPoint dtoPoint in lstYoko)
                        {
                            if (dtoPoint.FirstPoint == new Point(ball.X, ball.Y) || dtoPoint.SecondPoint == new Point(ball.X, ball.Y))
                            {
                                ball.BallDirection = 0;
                                break;
                            }
                        }
                    }

                    switch (ball.BallDirection)
                    {
                        case 0:
                            ball.Y -= 1;
                            break;
                        case 1:
                            ball.X += 1;
                            break;
                        case 2:
                            ball.X -= 1;
                            break;
                    }
                }
            }

            Invalidate();

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            colors.Add(Color.Red);
            colors.Add(Color.Green);
            colors.Add(Color.Blue);
            colors.Add(Color.Yellow);
            colors.Add(Color.Violet);
            colors.Add(Color.Orange);

            Label lblWhatText = new Label();
            lblWhatText.Text = "何にする？";
            lblWhatText.Location = new Point(274, 12);
            panel2.Controls.Add(lblWhatText);

            Label labelNinzu = new Label();
            labelNinzu.Location = new Point(87, 41);
            labelNinzu.Text = "何人でする？";
            panel2.Controls.Add(labelNinzu);

            NumericUpDown nbNinzu = new NumericUpDown();
            nbNinzu.Minimum = 2;
            nbNinzu.Maximum = 6;
            nbNinzu.Value = 2;
            nbNinzu.Location = new Point(71, 71);
            nbNinzu.Tag = 1;
            nbNinzu.ValueChanged += new EventHandler(nbNinzu_ValueChanged);
            panel2.Controls.Add(nbNinzu);

            Label lblYokosen = new Label();
            lblYokosen.Location = new Point(449, 41);
            lblYokosen.Text = "何本横線引く？";
            panel2.Controls.Add(lblYokosen);

            NBRedisplay();

            Button btnStart = new Button();
            btnStart.Text = "スタート";
            btnStart.Location = new Point(266, 240);
            btnStart.Click += new EventHandler(ButtonStart);
            panel2.Controls.Add(btnStart);

            panel1_Show();
            BallList();
            YokoList();

        }

        private void ButtonStart(object sender, EventArgs e)
        {
            List<string> lstItems = new List<string>();
            foreach (TextBox tb in lstTxts)
            {
                lstItems.Add(tb.Text);
                panel1.Controls.Remove(tb);
            }

            panel2.Dispose();

            for (int i = 1; i <= cntTate; i++)
            {
                Label label = new Label();
                label.Text = lstItems[i - 1];
                label.AutoSize = false;
                label.Location = new Point(600 / (cntTate + 1) * i - (600 / (cntTate + 1) - (30 / cntTate)) / 2, 20);
                label.Size = new Size(600 / (cntTate + 1) - (30 / cntTate), 25);
                label.TextAlign = ContentAlignment.MiddleCenter;
                panel1.Controls.Add(label);
                lstLbls.Add(label);
            }

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        private void nbNinzu_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = sender as NumericUpDown;
            cntTate = decimal.ToInt32(numericUpDown.Value);
            
            panel1_Show();
            BallList();
            YokoList();
            NBRedisplay();
        }

        private void nbYokosen_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = sender as NumericUpDown;
            cntYoko = decimal.ToInt32(numericUpDown.Value);

            YokoList();
        }

        private void NBRedisplay()
        {
            NumericUpDown NUD = new NumericUpDown();
            foreach (Control control in panel2.Controls)
            {
                if (control is NumericUpDown NBYokosen)
                {
                    if ((int)NBYokosen.Tag == 99999)
                    {
                        NUD = NBYokosen;
                    }
                }
            }
            panel2.Controls.Remove(NUD);

            NumericUpDown nbYokosen = new NumericUpDown();
            nbYokosen.Minimum = 0;
            nbYokosen.Maximum = 320;
            nbYokosen.Value = cntYoko;
            nbYokosen.Location = new Point(428, 71);
            nbYokosen.Tag = 99999;
            nbYokosen.ValueChanged += new EventHandler(nbYokosen_ValueChanged);
            panel2.Controls.Add(nbYokosen);



        }

        private void panel1_Show()
        {
            panel1.Controls.Clear();
            lstTxts.Clear();

            for (int i = 1; i <= cntTate; i++)
            {
                TextBox textBox = new TextBox();
                textBox.Location = new Point(600 / (cntTate + 1) * i - (600 / (cntTate + 1) - (30 / cntTate)) / 2, 20);
                textBox.Size = new Size(600 / (cntTate + 1) - (30 / cntTate), 25);
                textBox.TextAlign = HorizontalAlignment.Center;
                panel1.Controls.Add(textBox);
                lstTxts.Add(textBox);
            }
        }

        private void BallList()
        {

            lstBalls.Clear();

            for (int i = 0; i < cntTate; i++)
            {
                DtoBallPoint dtoBallPoint = new DtoBallPoint();
                dtoBallPoint.X = 600 / (cntTate + 1) * (i + 1);
                dtoBallPoint.Y = 450;
                lstBalls.Add(dtoBallPoint);
            }
        }

        private void YokoList()
        {
            lstYoko.Clear();

            Random random = new Random();
            List<int> cntNumber = new List<int>();

            for (int i = 0; i < cntTate - 1; i++)
            {
                cntNumber.Add(cntYoko / (cntTate - 1));
            }
            for (int i = 0; i < cntYoko % (cntTate - 1); i++)
            {
                cntNumber[i]++;
            }
            for (int i = 0; i < cntTate - 1; i++)
            {
                HashSet<int> lstPoint = new HashSet<int>();

                while (lstPoint.Count() < cntNumber[i])
                {
                    int addNum = random.Next(80 / (cntTate - 1), 400 / (cntTate - 1)) * (cntTate - 1) + i;
                    lstPoint.Add(addNum);
                }
                foreach (var point in lstPoint)
                {
                    DtoPoint dtoPoint = new DtoPoint();
                    dtoPoint.FirstPoint = new Point(600 / (cntTate + 1) * (i + 1), point);
                    dtoPoint.SecondPoint = new Point(600 / (cntTate + 1) * (i + 2), point);
                    lstYoko.Add(dtoPoint);
                }
            }
        }
    }

    public class DtoPoint()
    {
        /// <summary>ファーストポイント</summary>
        private Point firstPoint = new Point();

        /// <summary>セカンドポイント</summary>
        private Point secondPoint = new Point();

        /// <summary>ファーストポイント</summary>
        public Point FirstPoint
        {
            get
            {
                return firstPoint;
            }
            set
            {
                firstPoint = value;
            }
        }

        /// <summary>セカンドポイント</summary>
        public Point SecondPoint
        {
            get
            {
                return secondPoint;
            }
            set
            {
                secondPoint = value;
            }
        }
    }

    public class DtoBallPoint()
    {
        private int x = 0;

        private int y = 0;

        /// <summary>
        /// 0: 上
        /// 1: 右
        /// 2: 左
        /// </summary>
        private int ballDirection = 0;

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        /// <summary>
        /// 0: 上
        /// 1: 右
        /// 2: 左
        /// </summary>
        public int BallDirection
        {
            get
            {
                return ballDirection;
            }
            set
            {
                ballDirection = value;
            }
        }
    }
}
