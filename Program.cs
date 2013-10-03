using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ryfloyd_HW6
{
    class Program
    {
        static int[] votes;
        static cell[,] matrix;
        static int partition;
        static void Main(string[] args)
        {
            //Electoral College Votes for each state (the actual state to index is ilrelevant for this problem). Obtained from http://www.iweblists.com/us/government/2012ElectoralVotesPerState.html
            votes = new int[] { 9, 3, 11, 6, 55, 9, 7, 3, 3, 29, 16, 4, 4, 20, 11, 6, 6, 8, 8, 4, 10, 11, 16, 10, 6, 10, 3, 5, 6, 4, 14, 5, 29, 15, 3, 18, 7, 7, 20, 4, 9, 3, 11, 38, 6, 3, 13, 12, 5, 10, 3 };
            //votes = new int[] { 3, 1, 1, 2, 2, 1 };
            //votes = new int[] { 3, 1, 1, 2, 2, 1, 3, 1, 1, 2, 2, 1 };
            partition = votes.Sum() / 2;
            matrix = new cell[partition + 1, votes.Length + 1];

            // For the zero-sum, we can always include no votes, so this is our base case of 1.
            for (int i = 0; i <= votes.Length; i++)
            {
                matrix[0, i] = new cell(1,votes.Length);
            }

            // Zero out the rest of the matrix
            for (int i = 1; i <= partition; i++)
            {
                for (int j = 0; j <= votes.Length; j++)
                {
                    matrix[i, j] = new cell(0, votes.Length);
                }
            }
            Dictionary<int,int> startingVoteOccurances = DistinctCounts(votes);
            //O(Votes.Length * Sum/2) 
            for (int targetSum = 1; targetSum <= partition; targetSum++)
            {
                Dictionary<int, int> voteOccurrances = new Dictionary<int, int>();
                for (int j = 1; j <= votes.Length; j++)
                {
                    int vote = votes[j - 1];
                    //Keep track of the vote occurances we've seen so far.
                    if (voteOccurrances.ContainsKey(vote))
                        voteOccurrances[vote]++;
                    else
                        voteOccurrances[vote] = 1;

                    if (targetSum < vote) //We can't fit any more votes into this target sum. Use the last Optimal solution
                    {
                        matrix[targetSum, j].count = matrix[targetSum, j - 1].count;
                        matrix[targetSum, j - 1].CopyTo(matrix[targetSum, j]);
                    }
                    else //Else, include this vote to get to the target sum.
                    {
                        //If the backtrack cells contain a vote occurrance too many times, then we've exhausted all our votes.
                        cell Left = matrix[(targetSum - vote), j];
                        cell Up = matrix[targetSum, j - 1];
                        if (Left.SelectCount(vote) + Up.SelectCount(vote) >= voteOccurrances[vote])
                        {
                            matrix[targetSum, j].count = Left.count;
                        }
                        else
                        {
                            matrix[targetSum, j].count = Left.count + Up.count;
                            Left.CopyTo(matrix[targetSum, j]);
                            Up.CopyTo(matrix[targetSum, j]);
                            matrix[targetSum, j].Add(vote);
                        }
                    }
                    //Or call recursively
                    //matrix[i, c] = count(i, c);
                }
            }
            printMatrix();
            Console.ReadLine();
        }

        static int count(int n, int m)
        {
            if (n == 0)
                return 1;
            if (n < 0)
                return 0;
            if (m <= 0 && n >= 1)
                return 0;

            return count(n, m - 1) + count(n - votes[m-1], m);
        }
        static void printMatrix()
        {

            Console.Write("\t{0:0.##}", 0);
            for (int i = 0; i < votes.Length; i++)
            {
                Console.Write("\t{0:0.##}", votes[i]);
            }
            Console.WriteLine();
            for (int i = 0; i <= partition; i++)
            {
                Console.Write("\t______");
            }
            Console.WriteLine();

            for (int i = 0; i <= partition; i++)
            {
                if (i == 0) Console.Write("{0:0.##}\t", 0); else Console.Write("{0:0.##}\t", i);
                for (int j = 0; j <= votes.Length; j++)
                    Console.Write
                        ("{0:0.##}\t", matrix[i, j]);
                Console.WriteLine();
            }
        }
        static Dictionary<int,int> DistinctCounts (int[] list)
        {
            Dictionary<int, int> distinctCounts = new Dictionary<int,int>();
            foreach (int i in list)
            {
                if (distinctCounts.ContainsKey(i))
                    distinctCounts[i]++;
                else
                    distinctCounts.Add(i, 1);
            }
            foreach (var item in distinctCounts)
            {
                Console.WriteLine(item);
            }

            return distinctCounts;
        }
    }
    public class cell
    {
        public ulong count { get; set; }
        List<int> states;
        public cell(ulong initial, int length)
        {
            count = initial;
            states = new List<int>(length);
        }
        public override string ToString()
        {
            return count.ToString(); // String.Format("{0}:{1}", count.ToString(), String.Join(",", states));
        }

        internal void CopyTo(cell cell)
        {
            cell.states.AddRange(this.states);
        }

        internal void Add(int vote)
        {
            this.states.Add(vote);
        }

        internal int SelectCount(int vote)
        {
            var items = from p in states
                        where states.Contains(vote)
                        select p;
            return items.Count();
        }
    }
}
