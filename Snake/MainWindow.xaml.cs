using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    public partial class MainWindow : Window
    {
        private const int Rows = 20;
        private const int Cols = 20;
        private const int CellSize = 20;
        

        private enum Direction { Up, Down, Left, Right }
        private Direction currentDirection = Direction.Right;
        private Direction nextDirection = Direction.Right;

        private List<Point> snake = new List<Point>();
        private Point food;
        private DispatcherTimer timer;
        private Random rand = new Random();
        private int score = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitGame();
        }

        private void InitGame()
        {
            GameCanvas.Width = Cols * CellSize;
            GameCanvas.Height = Rows * CellSize;
            snake.Clear();
            snake.Add(new Point(5, 5));
            snake.Add(new Point(6, 5));
            currentDirection = Direction.Right;
            GenerateFood();
            score = 0;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += (s, e) => GameLoop();
            timer.Start();
        }

        private void GameLoop()
        {
            currentDirection = nextDirection;
            MoveSnake();
            CheckCollision();
            Draw();
        }

        private void MoveSnake()
        {
            Point head = snake.Last();
            Point newHead = head;
            switch (currentDirection)
            {
                case Direction.Up: newHead.Y--; break;
                case Direction.Down: newHead.Y++; break;
                case Direction.Left: newHead.X--; break;
                case Direction.Right: newHead.X++; break;
            }
            snake.Add(newHead);

            if (newHead.Equals(food))
            {
                score++;
                GenerateFood();
            }
            else
            {
                snake.RemoveAt(0);
            }
        }

        private void CheckCollision()
        {
            Point head = snake.Last();

            if (head.X < 0 || head.Y < 0 || head.X >= Cols || head.Y >= Rows || snake.Take(snake.Count - 1).Contains(head))
            {
                timer.Stop();
                MessageBox.Show($"Game Over! Punkte: {score}");
                InitGame();
            }
        }

        private void GenerateFood()
        {
            do
            {
                food = new Point(rand.Next(Cols), rand.Next(Rows));
            } while (snake.Contains(food));
        }

        private void Draw()
        {
            GameCanvas.Children.Clear();
            foreach (Point part in snake)
            {
                Rectangle rect = new Rectangle
                {
                    Width = CellSize,
                    Height = CellSize,
                    Fill = Brushes.Green
                };
                Canvas.SetLeft(rect, part.X * CellSize);
                Canvas.SetTop(rect, part.Y * CellSize);
                GameCanvas.Children.Add(rect);
            }

            Rectangle foodRect = new Rectangle
            {
                Width = CellSize,
                Height = CellSize,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(foodRect, food.X * CellSize);
            Canvas.SetTop(foodRect, food.Y * CellSize);
            GameCanvas.Children.Add(foodRect);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (currentDirection != Direction.Down) nextDirection = Direction.Up;
                    break;
                case Key.Down:
                    if (currentDirection != Direction.Up) nextDirection = Direction.Down;
                    break;
                case Key.Left:
                    if (currentDirection != Direction.Right) nextDirection = Direction.Left;
                    break;
                case Key.Right:
                    if (currentDirection != Direction.Left) nextDirection = Direction.Right;
                    break;
            }
        }
    }
}