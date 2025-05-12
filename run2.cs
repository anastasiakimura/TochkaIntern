using System;
using System.Collections.Generic;
using System.Linq;


class Program
{
    // Константы для символов ключей и дверей
    static readonly char[] keys_char = Enumerable.Range('a', 26).Select(i => (char)i).ToArray();
    static readonly char[] doors_char = keys_char.Select(char.ToUpper).ToArray();
    
    const int INF = int.MaxValue / 2;
    
    // Метод для чтения входных данных
    static List<List<char>> GetInput()
    {
        var data = new List<List<char>>();
        string line;
        while ((line = Console.ReadLine()) != null && line != "")
        {
            data.Add(line.ToCharArray().ToList());
        }
        return data;
    }

    class Edge
    {
        public int ToIndex;
        public int Dist;
        public int RequiredMask;
    }
    
    static int Solve(List<List<char>> data)
    {
        int h = data.Count;
        if (h == 0) return 0;
        int w = data[0].Count;

        var starts = new List<(int r, int c)>();
        var keysList = new List<(int r, int c, char k)>();
        for (int i = 0; i < h; i++)
            for (int j = 0; j < w; j++)
            {
                char ch = data[i][j];
                if (ch == '@') starts.Add((i, j));
                else if (ch >= 'a' && ch <= 'z') keysList.Add((i, j, ch));
            }

        int R = starts.Count;
        int K = keysList.Count;

        keysList.Sort((a, b) => a.k.CompareTo(b.k));

        int N = R + K;
        var points = new (int r, int c)[N];
        for (int i = 0; i < R; i++) points[i] = starts[i];
        for (int i = 0; i < K; i++) points[R + i] = (keysList[i].r, keysList[i].c);

        var graph = new List<Edge>[N];
        for (int i = 0; i < N; i++) graph[i] = new List<Edge>();

        int[] dr = { -1, 1, 0, 0 };
        int[] dc = { 0, 0, -1, 1 };

        for (int i = 0; i < N; i++)
        {
            var dist = new int[h, w];
            var mask = new int[h, w];
            for (int r = 0; r < h; r++)
                for (int c = 0; c < w; c++)
                    dist[r, c] = INF;

            var q = new Queue<(int r, int c)>();
            var (sr, sc) = points[i];
            dist[sr, sc] = 0;
            q.Enqueue((sr, sc));

            while (q.Count > 0)
            {
                var (r, c) = q.Dequeue();
                int d0 = dist[r, c];
                int m0 = mask[r, c];
                for (int d = 0; d < 4; d++)
                {
                    int nr = r + dr[d], nc = c + dc[d];
                    if (nr < 0 || nr >= h || nc < 0 || nc >= w) continue;
                    char ch2 = data[nr][nc];
                    if (ch2 == '#') continue;

                    int m1 = m0;
                    if (ch2 >= 'A' && ch2 <= 'Z')
                        m1 |= 1 << (ch2 - 'A');

                    if (dist[nr, nc] > d0 + 1)
                    {
                        dist[nr, nc] = d0 + 1;
                        mask[nr, nc] = m1;
                        q.Enqueue((nr, nc));
                    }
                }
            }

            for (int j = 0; j < K; j++)
            {
                int ti = R + j;
                var (kr, kc, _) = keysList[j];
                if (dist[kr, kc] < INF)
                {
                    graph[i].Add(new Edge
                    {
                        ToIndex = ti,
                        Dist = dist[kr, kc],
                        RequiredMask = mask[kr, kc]
                    });
                }
            }
        }

        int fullMask = (1 << K) - 1;
        var distState = new Dictionary<string, int>();
        var pq = new PriorityQueue<(int[] pos, int mask), int>();

        var startPos = Enumerable.Range(0, R).ToArray();
        string startKey = string.Join(",", startPos) + "|0";
        distState[startKey] = 0;
        pq.Enqueue((startPos, 0), 0);

        while (pq.Count > 0)
        {
            pq.TryDequeue(out var state, out int cd);
            var pos = state.pos;
            int cm = state.mask;
            string key = string.Join(",", pos) + "|" + cm;
            if (distState[key] < cd) continue;

            if (cm == fullMask) return cd;

            for (int robotIndex = 0; robotIndex < R; robotIndex++)
            {
                int from = pos[robotIndex];
                foreach (var e in graph[from])
                {
                    int kid = e.ToIndex - R;
                    int bit = 1 << kid;
                    if ((cm & bit) != 0) continue;
                    if ((e.RequiredMask & ~cm) != 0) continue;

                    var newPos = (int[])pos.Clone();
                    newPos[robotIndex] = e.ToIndex;
                    int nm = cm | bit;
                    int nd = cd + e.Dist;
                    string nkey = string.Join(",", newPos) + "|" + nm;
                    if (!distState.TryGetValue(nkey, out int prev) || nd < prev)
                    {
                        distState[nkey] = nd;
                        pq.Enqueue((newPos, nm), nd);
                    }
                }
            }
        }

        return -1;
    }
    
    static void Main()
    {
        var data = GetInput();
        int result = Solve(data);
        
        if (result == -1)
        {
            Console.WriteLine("No solution found");
        }
        else
        {
            Console.WriteLine(result);
        }
    }
}