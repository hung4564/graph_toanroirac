﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace graph_toanroirac
{
    class Program
    {
        /// <summary>
        /// Nhập ma trận vuông đối xứng
        /// </summary>
        /// <param name="a">Mảng chứa dữ liệu</param>
        /// <param name="n">số lượng phần tử</param>
        static void NhapMatrix(int[,] a, out int n)
        {
            Console.Write("Nhap so luong diem:");
            n = int.Parse(Console.ReadLine());
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    Console.Write("a[{0}:{1}]=", i, j);
                    a[i, j] = int.Parse(Console.ReadLine());
                    a[j, i] = a[i, j];
                }
            }
        }
        /// <summary>
        /// Xuất ma trận vuông
        /// </summary>
        /// <param name="a">Mảng chứa dữ liệu</param>
        /// <param name="n">số lượng phần tử</param>
        static void XuatMatrix(int[,] a, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(a[i, j] + " ");
                }
                Console.Write("\n");
            }
        }
        /// <summary>
        /// Đọc ma trận từ file
        /// </summary>
        /// <param name="a">Mảng chứa dữ liệu</param>
        /// <param name="n">số lượng phần tử</param>
        static void ReadMatrix(int[,] a, out int n)
        {
            string path = "matrix.txt";
            if (File.Exists(path))
            {
                n = 0;
                FileStream inFile = new FileStream(path, FileMode.Open, FileAccess.Read);
                using (var sr = new StreamReader(inFile))
                {
                    // Doc file, lay tung hang roi tach tung cot cho vao ma tran
                    string matrix = sr.ReadToEnd();
                    while (matrix.IndexOf("\r") > 0)
                        matrix = matrix.Remove(matrix.IndexOf("\r"), 1);
                    string[] rows = matrix.Split('\n');
                    n = rows.Count();
                    for (int i = 0; i < rows.Count(); i++)
                    {
                        string[] columns = rows[i].Split(' ');
                        for (int j = 0; j < columns.Count(); j++)
                        {
                            a[i, j] = int.Parse(columns[j]);
                        }
                    }
                }
            }
            else
            {
                File.Create(path);
                NhapMatrix(a, out n);
            }
        }
        /// <summary>
        /// Ghi ma trận vào file
        /// </summary>
        /// <param name="a">Mảng chứa dữ liệu</param>
        /// <param name="n">số lượng phần tử</param>
        static void WriteMarix(int[,] a, out int n)
        {
            string path = "matrix.txt";
            NhapMatrix(a, out n);
            if (File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
            FileStream inFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            using (var sw = new StreamWriter(inFile))
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        sw.Write(a[i, j].ToString() + " ");
                    }
                    if (i != n - 1) sw.WriteLine();
                }
            }
        }
        /// <summary>
        /// Kiểm tra tính liên thông của ma trận
        /// </summary>
        /// <param name="A">Ma trân vuông cần kiểm tra</param>
        /// <param name="n">Độ dài cạnh</param>
        /// <returns>Trả về đúng nếu liên thông</returns>
        static bool Is_lienthong(int[,] A, int n)
        {
            bool[] DanhDau = new bool[n];
            int ThanhCong;
            int Dem = 0;
            DanhDau[0] = true;         //Đánh dấu đỉnh đầu
            Dem++;              //Đểm số đỉnh đã đánh dấu là 1
            do
            {
                ThanhCong = 1;      //Giả sử không còn khả năng loang
                for (int i = 0; i < n; i++)
                    if (DanhDau[i])
                    {
                        for (int j = 0; j < n; j++)
                            if (!DanhDau[j] && A[i, j] > 0)
                            {
                                DanhDau[j] = true;
                                ThanhCong = 0;  //Thực tế còn khả năng loang
                                Dem++;
                                if (Dem == n) return true;
                            }
                    }
            } while (ThanhCong == 0);	//Lặp lại cho đến khi không còn khả năng loang
            return false;
        }
        /// <summary>
        /// Kiểm tra tính đầy đủ của ma trận
        /// </summary>
        /// <param name="a">ma trận cần kiểm tra</param>
        /// <param name="n">số cạnh của ma trận</param>
        /// <returns>Trả về đúng nếu là ma trận đầy đủ</returns>
        static bool Is_daydu(int[,] a, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (a[i, j] == 0) return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Duyệt theo chiều sâu
        /// </summary>
        /// <param name="a">Mảng chứa đồ thị cần duyệt</param>
        /// <param name="n">số đỉnh</param>
        /// <param name="start">đỉnh bắt đầu(mặc định là đỉnh 0)</param>
        static void Travel_Deep(int[,] a, int n, int start = 0)
        {
            bool[] visit = new bool[n];
            Stack<int> stack = new Stack<int>();
            stack.Push(start);
            while (stack.Count > 0)
            {
                int current = stack.Pop();
                if (!visit[current])
                {
                    visit[current] = true;
                    Console.Write(current + 1 + "->");
                    for (int i = 0; i < n; i++)
                    {
                        // Nếu là đỉnh kề và chưa được ghé thăm
                        if (a[current, i] > 0 && !visit[i])
                        {
                            stack.Push(i);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Duyệt theo chiều rộng
        /// </summary>
        /// <param name="a">Mảng chứa đồ thị cần duyệt</param>
        /// <param name="n">số đỉnh</param>
        /// <param name="start">đỉnh bắt đầu(mặc định là đỉnh 0)</param>
        static void Travel_Breadth(int[,] a, int n, int start = 0)
        {
            bool[] visit = new bool[n];
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                if (!visit[current])
                {
                    visit[current] = true;
                    Console.Write(current + 1 + "->");
                    for (int i = 0; i < n; i++)
                    {
                        // Nếu là đỉnh kề và chưa được ghé thăm
                        if (a[current, i] > 0 && !visit[i])
                        {
                            queue.Enqueue(i);
                        }
                    }
                }
            }
        }
        static void Kruskal(int[,] a, int n)
        {
            List<Edge> list = matrix_edge_convert(a, n);
            list.Sort();
            List<Edge> listResult = new List<Edge>();
            //danh dau nhan i cho dinh i
            int[] label = new int[n];
            for (int i = 0; i < n; i++)
            {
                label[i] = i;
            }
            int lab1 = 0;
            int lab2 = 0;
            foreach (Edge item in list)
            {
                if(label[item.start]!=label[item.end])
                {
                    listResult.Add(item);
                    if(label[item.start] > label[item.end])
                    {
                        lab1 = label[item.end];
                        lab2 = label[item.start];                           
                    }
                    else
                    {
                        lab2 = label[item.end];
                        lab1 = label[item.start];
                    }
                    for (int i = 0; i < n; i++)
                    {
                        if (label[i] == lab2) label[i] = lab1;
                    }
                }
            }
            foreach (Edge item in listResult)
            {
                Console.WriteLine(item.ToString());
            }
        }
        static List<Edge> matrix_edge_convert(int[,] a, int n)
        {
            List<Edge> listEdge = new List<Edge>();
            Edge edge;
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    if (a[i, j] > 0)
                    {
                        edge = new Edge(i, j, a[i, j]);
                        listEdge.Add(edge);
                    }
                }
            }
           // listEdge.Sort();
            return listEdge;
        }
        static void Prime(int[,] matrix, int n, int start = 0)
        {
            List<int> list_dinh = new List<int>();
            List<int> list_result = new List<int>();
            for (int i = 0; i < n; i++)
            {
                list_dinh.Add(i);
            }
            while(list_dinh.Count>0)
            {
            }
        }
        class Edge : IEquatable<Edge>, IComparable<Edge>
        {
            public int start;
            public int end;
            int weight;

            public Edge(int start, int end, int weight)
            {
                this.start = start;
                this.end = end;
                this.weight = weight;
            }

            public override string ToString()
            {
                return string.Format("{0}->{1}:{2}", start+1, end+1, weight);
            }
            public int CompareTo(Edge other)
            {
                if (other == null)
                    return 1;

                else
                    return this.weight.CompareTo(other.weight);
            }
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                Edge objAsEdge = obj as Edge;
                if (objAsEdge == null) return false;
                else return Equals(objAsEdge);
            }
            public override int GetHashCode()
            {
                return weight;
            }
            public bool Equals(Edge other)
            {
                if (other == null) return false;
                return (this.weight.Equals(other.weight));
            }
        }
        static void Main(string[] args)
        {
            int n = 0;
            int[,] matrix = new int[50, 50];
            ReadMatrix(matrix, out n);
            Kruskal(matrix, n);
            Console.ReadKey();
        }
    }
}
