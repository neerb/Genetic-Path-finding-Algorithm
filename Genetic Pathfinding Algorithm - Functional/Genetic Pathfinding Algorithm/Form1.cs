using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genetic_Pathfinding_Algorithm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<DNANode> nodes;
        Random rand = new Random();

        int objX = 0;
        int objY = 0;

        int resetX;
        int resetY;

        int generation = 1;
        int mutations = 0;

        int availableMoves = 8;

        Obstacle[] obstacles;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            spacing = width / boardSize;
            objX = boardSize - 3;
            objY = rand.Next(boardSize - 2) + 1;

            resetX = 2;
            resetY = rand.Next(boardSize - 2) + 1;

            obstacles = new Obstacle[boardSize * 2];

            //for (int y = 0; y < obstacles.Length; y++)
            //{
            //    //if(y != 3)
            //    obstacles[y] = null;//new Obstacle(rand.Next(boardSize - 1), rand.Next(boardSize - 1));//new Obstacle(boardSize / 2, y + 5);
            //}
        }

        void SelectParentsActivateBreed()
        {
            SortBasedDistance();

            List<DNANode> newNodes = new List<DNANode>();
            for (int i = 0; i < nodeCount; i++)
            {
                newNodes.Add(Breed(nodes[0], nodes[rand.Next((nodes.Count / 3) - 1)]));

                nodes[i].x = resetX;
                nodes[i].y = resetY;
            }

            //nodes.Clear();
            //nodes = newNodes;

            //this.Invalidate();
            //this.Update();
            //this.Refresh();

            //MessageBox.Show(nodes.Count.ToString());

            for (int i = 0; i < nodes.Count; i++)
            {
                DNANode n = nodes[i];
                int pathCount = newNodes[i].path.Count;
                nodes[i].path.Clear();
                nodes[i].posPath.Clear();

                //MessageBox.Show("Node " + i);
                //MessageBox.Show(pathCount.ToString());
                //MessageBox.Show(nodes[i].x + ", " + nodes[i].y);

                for (int p = 0; p < pathCount; p++)
                {
                    //MessageBox.Show(nodes[i].x + "," + nodes[i].y);

                    //this.Invalidate();
                    //this.Update();
                    //this.Refresh();

                    if (!(n.x <= 0 || n.y <= 0 || n.x >= boardSize - 1 || n.y >= boardSize - 1)
                        && !nodes[i].isStuck)
                    {
                        //MessageBox.Show(newNodes[i].path[p].ToString());
                        nodes[i].AddNewPath(newNodes[i].path[p]);

                        //Obstacle Logic
                        foreach (Obstacle ob in obstacles)
                        {
                            for (int k = 0; k < nodes.Count; k++)
                            {
                                if (!nodes[k].isStuck)
                                    if (NeighboringObstacle(nodes[k], ob))
                                    {
                                        nodes[k].isStuck = true;
                                    }
                            }
                        }
                    }
                    else
                        break;
                }

                this.Invalidate();
                this.Update();
                this.Refresh();
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].distance = Distance(nodes[i].x, objX, nodes[i].y, objY);
                nodes[i].functionalCost = nodes[i].distance / nodes[i].posPath.Count;
                nodes[i].isStuck = false;
            }
        }

        bool NeighboringObstacle(DNANode node, Obstacle obst)
        {
            if (obst != null)
                for (int x = -1; x <= 1; x += 1)
                {
                    for (int y = -1; y <= 1; y += 1)
                    {
                        if (obst.x == x + node.x && obst.y == y + node.y)
                        {
                            return true;
                        }
                    }
                }

            return false;
        }

        DNANode Breed(DNANode p1, DNANode p2)
        {
            List<int> newPath = new List<int>();

            for (int i = 0; i < p1.path.Count / 2; i++)
            {
                newPath.Add(p1.path[i]);
            }

            for (int i = p2.path.Count / 2; i < p2.path.Count; i++)
            {
                newPath.Add(p2.path[i]);
            }

            /*
             * Potential Breeding methodology *
             */
            //List<int> newPath = new List<int>();
            //int pathSize = 0;

            //DNANode shortest;
            //DNANode longest;

            //if (p1.path.Count > p2.path.Count)
            //{
            //    shortest = p2;
            //    longest = p1;
            //}
            //else
            //{
            //    shortest = p1;
            //    longest = p2;
            //}

            //pathSize = shortest.path.Count;

            //for(int i = 0; i < pathSize; i++)
            //{
            //    if (i % 2 == 0)
            //    {
            //        newPath.Add(shortest.path[i]);
            //    }
            //    else
            //        newPath.Add(longest.path[i]);
            //}

            //for(int i = pathSize; i < longest.path.Count; i++)
            //{
            //    newPath.Add(longest.path[i]);
            //}


            //List<int> newPath2 = new List<int>();

            //for (int i = 0; i < p2.path.Count / 2; i++)
            //{
            //    newPath2.Add(p1.path[i]);
            //}

            //for (int i = p1.path.Count / 2; i < p1.path.Count; i++)
            //{
            //    newPath2.Add(p2.path[i]);
            //}

            //List<int> newPath = new List<int>();

            //for(int i = 0; i < newPath1.Count; i++)
            //{
            //    newPath.Add((newPath1[i] + newPath2[i]) / 2);
            //}


            //Mutation
            if (generation % 2 == 0 && rand.Next(100) >= 25)
            {
                if (newPath.Count > 0)
                {
                    if (rand.Next(2) == 1)
                        newPath[rand.Next(newPath.Count - 1)] = rand.Next(availableMoves);
                    else
                    {
                        for (int i = 0; i < 5; i++)
                            newPath.Add(rand.Next(availableMoves));
                    }
                }
                mutations++;
            }

            DNANode child = new DNANode(resetX, resetY, p1.brushColor);
            child.path = newPath;

            return child;
        }

        float Distance(int x1, int x2, int y1, int y2)
        {
            return (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        void GenerateInitialPaths(DNANode n)
        {
            int totalMoves = 0;

            while (!(n.x <= 0 || n.y <= 0 || n.x >= boardSize - 1 || n.y >= boardSize - 1))
            {
                if (n.isStuck)
                    break;

                n.AddNewPath(rand.Next(availableMoves));
                totalMoves++;

                //Obstacle Logic
                foreach (Obstacle ob in obstacles)
                {
                    for (int k = 0; k < nodes.Count; k++)
                    {
                        if (!nodes[k].isStuck)
                            if (NeighboringObstacle(nodes[k], ob))
                            {
                                nodes[k].isStuck = true;
                            }
                    }
                }

                //this.Invalidate();
                //this.Update();
                //this.Refresh();

                //System.Threading.Thread.Sleep(20);

                //if (totalMoves >= boardSize * 5)
                //    break;
            }

            nodes[0].fittest = false;

            for (int i = 0; i < nodeCount; i++)
            {
                nodes[i].distance = Distance(nodes[i].x, objX, nodes[i].y, objY);
                nodes[i].functionalCost = nodes[i].distance / nodes[i].posPath.Count;
                nodes[i].isStuck = false;
            }
        }

        void SortBasedDistance()
        {
            int tracker = 0;

            while (tracker <= nodes.Count)
            {
                for (int i = 0; i < nodeCount - 1; i++)
                {
                    DNANode temp = nodes[i];

                    float comparative1 = nodes[i].functionalCost;
                    float comparative2 = nodes[i + 1].functionalCost;

                    if (comparative1 > comparative2)
                    {
                        nodes[i] = nodes[i + 1];
                        nodes[i + 1] = temp;
                        tracker = 0;
                    }
                    else
                        tracker++;
                }
            }

            //nodes.Reverse();
            nodes[0].fittest = true;
        }

        int boardSize = 20;
        int nodeCount = 7;
        int width = 800;
        int spacing;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            spacing = width / boardSize;


            if (nodes != null)
            {
                for (int i = 0; i < nodeCount; i++)
                {
                    for (int y = 0; y < obstacles.Length; y++)
                    {
                        if (obstacles[y] != null)
                            g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(obstacles[y].x * spacing, obstacles[y].y * spacing, spacing, spacing));
                    }

                    if (nodes[i].fittest)
                    {
                        for (int j = 0; j < nodes[i].posPath.Count; j++)
                        {
                            Point p = nodes[i].posPath[j];
                            g.FillRectangle(new SolidBrush(nodes[i].brushColor), new Rectangle(p.X * spacing, p.Y * spacing, spacing, spacing));
                        }

                        g.FillRectangle(Brushes.Gold, new Rectangle(nodes[i].x * spacing, nodes[i].y * spacing, spacing, spacing));
                    }
                    //else
                    //g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(nodes[i].x * spacing, nodes[i].y * spacing, spacing, spacing));
                }

                Font fnt = new Font("Arial", 10);

                g.DrawString("Generation: " + generation.ToString(), new Font("Arial", 12), Brushes.White, new Point(width + 20, 5));

                int y1 = 40;
                for (int i = 0; i < nodes.Count; i++)
                {
                    g.FillRectangle(new SolidBrush(nodes[i].brushColor), new Rectangle(width + 2, y1, 15, (int)fnt.Size));
                    g.DrawString("Node: " + (i + 1) + " - Distance: " + nodes[i].distance.ToString() + " & FCost = " + nodes[i].functionalCost, fnt, Brushes.White, new Point(width + 20, y1));
                    y1 += (int)fnt.Size + 2;
                }


                g.DrawString("Total Mutations: " + mutations.ToString(), new Font("Arial", 12), Brushes.White, new Point(width + 20, y1 + 3));
            }

            g.FillRectangle(new SolidBrush(Color.Green), new Rectangle(resetX * spacing, resetY * spacing, spacing, spacing));
            g.DrawString("A", new Font("Arial", spacing / 2), Brushes.Black, new Point(resetX * spacing, resetY * spacing));


            g.FillRectangle(Brushes.Blue, new Rectangle(objX * spacing, objY * spacing, spacing, spacing));
            g.DrawString("B", new Font("Arial", spacing / 2), Brushes.Black, new Point(objX * spacing, objY * spacing));


            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    g.DrawRectangle(new Pen(Color.White, 1f), new Rectangle(x * spacing, y * spacing, spacing, spacing));
                }
            }
        }

        bool initial = true;

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                Start();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            initial = true;
            nodes = null;

            objX = boardSize - 3;
            objY = rand.Next(boardSize - 2) + 1;

            resetX = 2;
            resetY = rand.Next(boardSize - 2) + 1;

            nodeCount = (int)numericUpDown1.Value;

            mutations = 0;
            generation = 0;

            this.Invalidate();
            this.Update();
            this.Refresh();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            Start();
        }

        void Start()
        {
            nodes = new List<DNANode>();
            nodeCount = (int)numericUpDown1.Value;

            for (int i = 0; i < nodeCount; i++)
            {
                nodes.Add(new DNANode(resetX, resetY, Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256))));
                //GenerateInitialPaths(nodes[i]);
            }

            if (initial)
            {
                for (int i = 0; i < nodeCount; i++)
                    GenerateInitialPaths(nodes[i]);
                initial = false;
                generation++;
            }

            bool pointFound = false;
            while (!pointFound)
            {
                SelectParentsActivateBreed();
                generation++;
                nodes[0].fittest = false;

                for (int i = 0; i < nodes.Count; i++)
                    if (nodes[i].distance <= 1.5)
                    {
                        nodes[i].fittest = true;
                        pointFound = true;
                        break;
                    }
            }

            this.Invalidate();
            this.Update();
            this.Refresh();
            MessageBox.Show("PATH FOUND", "Notification");
        }

        private void boardSizeVal_ValueChanged(object sender, EventArgs e)
        {
            boardSize = (int)boardSizeVal.Value;

            objX = boardSize - 3;
            objY = rand.Next(boardSize - 2) + 1;

            resetX = 2;
            resetY = rand.Next(boardSize - 2) + 1;

            this.Invalidate();
            this.Update();
            this.Refresh();
        }
    }
}
