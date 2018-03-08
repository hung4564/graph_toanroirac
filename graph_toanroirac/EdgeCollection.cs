﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace graph_toanroirac
{
    class EdgeCollection : IEnumerable<Edge>
    {
        List<Edge> _list;

        public int SelectedIndex
        {
            get;
            set;
        }
        public void Sort()
        {
            _list.Sort();
        }
        public Edge SelectedItem
        {
            get
            {
                return _list[this.SelectedIndex];
            }
        }
        public EdgeCollection()
        {
            _list = new List<Edge>();
        }
        public bool Add(Edge edge)
        {
            //Kiếm tra có cạnh đó chưa
            if (!_list.Contains(edge))
            {
                Edge newEdge = new Edge(edge.end, edge.start, edge.weight);
                //Kiểm tra cạnh vô hướng hay có hướng(tức có cạnh có đỉnh ngược)
                if (!_list.Contains(newEdge))
                {
                    edge = _list[_list.IndexOf(newEdge)];
                    edge.IsUndirected = true;
                }
                else
                {
                    _list.Add(edge);
                }
                return true;
            }
            return false;
        }
        public void Clear()
        {
            _list.Clear();
        }
        public bool Contains(Edge edge)
        {
            return _list.Contains(edge);
        }
        /// <summary>
        /// Trả về cạnh theo đỉnh
        /// </summary>
        /// <param name="start">Đỉnh đầu</param>
        /// <param name="end">Đỉnh cuối</param>
        /// <returns>Trả về null nếu không tồn tại</returns>
        public Edge this[Node start, Node end]
        {
            get
            {
                foreach (Edge item in _list)
                {
                    if (item.start == start && item.end == end)
                    {
                        return item;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Trả về các cạnh kề với đỉnh
        /// </summary>
        /// <param name="start">Đỉnh xét</param>
        /// <returns>Null nếu đỉnh không có cạnh kề</returns>
        public EdgeCollection this[Node start]
        {
            get
            {
                EdgeCollection edgeCollection = new EdgeCollection();
                foreach (Edge item in _list)
                {
                    if (item.start == start)
                    {
                        edgeCollection.Add(item);
                    }
                }
                if (edgeCollection.Count > 0)
                    return edgeCollection;
                else return null;
            }
        }
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
        public int Count
        {
            get { return _list.Count; }
        }
        public IEnumerator<Edge> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}