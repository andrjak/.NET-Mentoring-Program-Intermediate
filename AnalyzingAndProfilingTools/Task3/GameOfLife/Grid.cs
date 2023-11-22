using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Grid
    {
        private readonly int SizeX;
        private readonly int SizeY;
        private readonly Cell[,] cells;
        private readonly Cell[,] nextGenerationCells;
        private readonly Random random;
        private readonly Canvas drawCanvas;
        private readonly Ellipse[,] cellsVisuals;


        public Grid(Canvas canvas)
        {
            drawCanvas = canvas;
            random = new Random();
            SizeX = (int)(canvas.Width / Settings.CellSize);
            SizeY = (int)(canvas.Height / Settings.CellSize);
            cells = new Cell[SizeX, SizeY];
            nextGenerationCells = new Cell[SizeX, SizeY];
            cellsVisuals = new Ellipse[SizeX, SizeY];

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, false);
                    nextGenerationCells[i, j] = new Cell(i, j, 0, false);
                }
            }

            SetRandomPattern();
            InitCellsVisuals();
            SetInitGraphics();
        }

        public void Clear()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    if (cells[i, j].IsAlive)
                    {
                        cellsVisuals[i, j].Fill = Brushes.Gray;
                    }

                    cells[i, j].IsAlive = false;
                    cells[i, j].Age = 0;
                }
            }
        }

        public void UpdateGraphics()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    var cell = cells[i, j];
                    var visualCell = cellsVisuals[i, j];

                    if (cell.IsAlive && cell.Age < 2)
                    {
                        visualCell.Fill = Brushes.White;
                    }
                    else if (cell.IsAlive)
                    {
                        visualCell.Fill = Brushes.DarkGray;
                    }
                    else
                    {
                        visualCell.Fill = Brushes.Gray;
                    }
                }
            }
        }

        public void InitCellsVisuals()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    var positionX = cells[i, j].PositionX;
                    var positionY = cells[i, j].PositionY;

                    var cellsVisual = new Ellipse()
                    {
                        Width = Settings.CellSize,
                        Height = Settings.CellSize,
                        Margin = new Thickness(positionX, positionY, 0, 0),
                        Fill = Brushes.Gray
                    };

                    drawCanvas.Children.Add(cellsVisual);

                    cellsVisual.MouseMove += MouseMove;
                    cellsVisual.MouseLeftButtonDown += MouseMove;

                    cellsVisuals[i, j] = cellsVisual;
                }
            }
        }

        public bool GetRandomBoolean()
        {
            return random.NextDouble() > 0.8;
        }

        public void SetRandomPattern()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j].IsAlive = GetRandomBoolean();
                }
            }
        }

        public void UpdateToNextGeneration()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j].IsAlive = nextGenerationCells[i, j].IsAlive;
                    cells[i, j].Age = nextGenerationCells[i, j].Age;
                }
            }

            UpdateGraphics();
        }


        public void Update()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    CalculateNextGeneration(i, j);
                }
            }
            UpdateToNextGeneration();
        }

        public void CalculateNextGeneration(int i, int j)
        {
            var currentGenerationCell = cells[i, j];
            var nextGenerationCell = nextGenerationCells[i, j];
            var isAlive = currentGenerationCell.IsAlive;

            var neighborsCount = CountNeighbors(i, j);

            if (isAlive && neighborsCount < 2)
            {
                nextGenerationCell.IsAlive = false;
                nextGenerationCell.Age = 0;
            }
            else if (isAlive && (neighborsCount == 2 || neighborsCount == 3))
            {
                nextGenerationCell.IsAlive = true;
                nextGenerationCell.Age = currentGenerationCell.Age + 1;
            }
            else if (isAlive && neighborsCount > 3)
            {
                nextGenerationCell.IsAlive = false;
                nextGenerationCell.Age = 0;
            }
            else if (!isAlive && neighborsCount == 3)
            {
                nextGenerationCell.IsAlive = true;
                nextGenerationCell.Age = 0;
            }
            else
            {
                nextGenerationCell.IsAlive = false;
                nextGenerationCell.Age = 0;
            }
        }

        public int CountNeighbors(int i, int j)
        {
            int count = 0;

            if (i != SizeX - 1 && cells[i + 1, j].IsAlive) count++;
            if (i != SizeX - 1 && j != SizeY - 1 && cells[i + 1, j + 1].IsAlive) count++;
            if (j != SizeY - 1 && cells[i, j + 1].IsAlive) count++;
            if (i != 0 && j != SizeY - 1 && cells[i - 1, j + 1].IsAlive) count++;
            if (i != 0 && cells[i - 1, j].IsAlive) count++;
            if (i != 0 && j != 0 && cells[i - 1, j - 1].IsAlive) count++;
            if (j != 0 && cells[i, j - 1].IsAlive) count++;
            if (i != SizeX - 1 && j != 0 && cells[i + 1, j - 1].IsAlive) count++;

            return count;
        }

        // At the first generation, there are no generations that live long enough to paint them dark
        private void SetInitGraphics()
        {
            for (var i = 0; i < SizeX; i++)
            {
                for (var j = 0; j < SizeY; j++)
                {
                    if (cells[i, j].IsAlive)
                    {
                        cellsVisuals[i, j].Fill = Brushes.White;
                    }
                }
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            var cellVisual = sender as Ellipse;

            int i = (int)cellVisual.Margin.Left / Settings.CellSize;
            int j = (int)cellVisual.Margin.Top / Settings.CellSize;


            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!cells[i, j].IsAlive)
                {
                    cells[i, j].IsAlive = true;
                    cells[i, j].Age = 0;
                    cellVisual.Fill = Brushes.White;
                }
            }
        }
    }
}