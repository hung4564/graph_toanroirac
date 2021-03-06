﻿using System;
using System.Collections.Generic;
using System.Collections;
namespace graph_toanroirac
{
    class EdgeCollection : IEnumerable<Edge>
    {
        public int SelectedIndex
        {
            get;
            set;
        }
        // Lưu thông tin về các cạnh, chỉ lưu cạnh có hướng, nếu 2 chiều giống nhau thì là coi là 1 cạnh vô hướng
        List<Edge> _list;
        public EdgeCollection()
        {
            _list = new List<Edge>();
        }
        public int Count
        {
            get { return _list.Count; }
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
                    if ((item.start == start && item.end == end)||(item.IsUndirected&&item.end == start && item.start == end))
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
                    else if(item.IsUndirected && item.end == start)
                    {
                        edgeCollection.Add(item);
                    }
                }
                if (edgeCollection.Count > 0)
                    return edgeCollection;
                else return null;
            }
        }
        public Edge this[int index]
        {
            get
            {
                return _list[index];
            }
        }
        public void Sort()
        {
            _list.Sort();
        }
        public void Add(Edge edge)
        {
            //Kiếm tra có cạnh đó chưa
            if (!_list.Contains(edge))
            {
                Edge newEdge = new Edge(edge.end, edge.start, edge.weight);
                //Kiểm tra cạnh vô hướng hay có hướng
                if (_list.Contains(newEdge))
                {
                    edge = _list[_list.IndexOf(newEdge)];
                    edge.IsUndirected = true;
                }
                else
                {
                    _list.Add(edge);
                }
            }
        }
        public void Clear()
        {
            _list.Clear();
        }
        public bool Contains(Edge edge)
        {
            if (edge.IsUndirected)
                return _list.Contains(edge) || _list.Contains(new Edge(edge.end, edge.start, edge.weight));
            else
                return _list.Contains(edge);
        }
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
        public bool Remove(Edge edge)
        {
            return _list.Remove(edge);
        }
        /// <summary>
        /// Xóa các cạnh liên quan đến đỉnh
        /// </summary>
        /// <param name="node">Đỉnh liên quan</param>
        public void RemoveBy(Node node)
        {
            for(int i = 0;i<_list.Count;i++)
            {
                if (_list[i].IsUndirected)
                {
                    if (_list[i].start == node || _list[i].end == node)
                    {
                        _list.Remove(_list[i]);
                        i--;
                    }
                }
                else
                {
                    if (_list[i].start == node)
                    {
                        _list.Remove(_list[i]);
                        i--;
                    }
                }
            }
        }
        public IEnumerator<Edge> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        public void Reset()
        {
            foreach (var item in _list)
            {
                item.Reset();
            }
        }

        
    }
}
