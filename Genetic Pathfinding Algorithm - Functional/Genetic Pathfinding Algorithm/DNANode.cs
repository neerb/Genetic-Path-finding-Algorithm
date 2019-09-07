using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Genetic_Pathfinding_Algorithm
{
    class DNANode
    {
        public List<int> path = new List<int>();
        public List<Point> posPath = new List<Point>();
        public float distance;
        public float functionalCost;
        public int x, y;
        public bool activeNode = true;
        public Color brushColor;
        Random r = new Random();
        public int totalFail = 0;
        public bool isStuck = false;
        public bool fittest = false;

        public DNANode(int x1, int y1, Color c)
        {
            x = x1;
            y = y1;

            brushColor = c;
        }

        public void AddNewPath(int r)
        {
            int dir = r;
            path.Add(dir);

            switch (dir)
            {
                case 0:
                    if (!posPath.Contains(new Point(x - 1, y)))
                        x -= 1;
                    break;
                case 1:
                    if (!posPath.Contains(new Point(x, y - 1)))
                        y -= 1;
                    break;
                case 2:
                    if (!posPath.Contains(new Point(x + 1, y)))
                        x += 1;
                    break;
                case 3:
                    if (!posPath.Contains(new Point(x, y + 1)))
                        y += 1;
                    break;
                case 4:
                    if (!posPath.Contains(new Point(x - 1, y - 1)))
                    {
                        x -= 1;
                        y -= 1;
                    }
                    break;
                case 5:
                    if (!posPath.Contains(new Point(x - 1, y + 1)))
                    {
                        x -= 1;
                        y += 1;
                    }
                    break;
                case 6:
                    if (!posPath.Contains(new Point(x + 1, y + 1)))
                    {
                        x += 1;
                        y += 1;
                    }
                    break;
                case 7:
                    if (!posPath.Contains(new Point(x + 1, y - 1)))
                    {
                        x += 1;
                        y -= 1;
                    }
                    break;
            }

            int tot = 0;
            for (int x1 = -1; x1 <= 1; x1 += 2)
            {
                for (int y1 = -1; y1 <= 1; y1 += 2)
                {
                    if (posPath.Contains(new Point(x + x1, y + y1)))
                        tot++;
                }
            }

            if (tot >= 3)
                isStuck = true;

            if (!posPath.Contains(new Point(x, y)))
                posPath.Add(new Point(x, y));
        }
    }
}
