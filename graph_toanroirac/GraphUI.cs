﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace graph_toanroirac
{
    class GraphUI : Control
    {
        public event EventHandler SelectedNodeChanged;
        public event EventHandler DrawEvent;
        public event EventHandler GraphChange;
        public EdgeCollection _list = new EdgeCollection();
        Graph _graph;
        public Graph Data
        {
            get { return _graph; }
            set
            {
                _graph = value;
                _graph.GraphChange += GraphChange;
            }
        }

        private void _graph_GraphChange(object sender, EventArgs e)
        {
            if (GraphChange != null)
                GraphChange(sender, null);
        }

        public const int NODE_RADIUS = 12;
        public const int NODE_DIAMETER = NODE_RADIUS * 2;
        Pen _penEdge;
        public DrawingTools Tool;
        public IEnumerable<char> NodeNames
        {
            get
            {
                List<char> list = new List<char>();
                for (int i = 0; i < this.Controls.Count - 1; i++)
                {
                    list.Add((char)('A' + i));
                }
                return list;
            }
        }
        public bool IsUndirectedGraph
        {
            get { return _graph.IsUndirected; }
            set
            {
                _graph.IsUndirected = value;
                if (value)
                    _penEdge.EndCap = LineCap.NoAnchor;
                else
                    _penEdge.EndCap = LineCap.ArrowAnchor;
                //RefreshMatrix();
                Invalidate();
            }
        }

        Point _startPoint;
        Node _startNode;
        Point _p;
        int _selectedIndex;
        public NodeUI SelectedNode
        {
            get
            {
                if (_selectedIndex < 0)
                    return null;
                return this.Controls[_selectedIndex] as NodeUI;
            }
        }
        public GraphUI()
        {
            this.DoubleBuffered = true;
            Control.CheckForIllegalCrossThreadCalls = false;

            _penEdge = new Pen(Color.MediumPurple, 4);
            _penEdge.EndCap = LineCap.ArrowAnchor;

            _graph = new Graph();
            _graph.GraphChange += GraphChange;
            //Reset();
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            //foreach(Control ctl in this.Controls)

            if (e.Button == MouseButtons.Left)
            {
                if (this.Tool == DrawingTools.Node)
                {
                    int m = 'Z' - 'A' + 2;
                    if (this.Controls.Count == m)
                    {
                        MessageBox.Show("You can only add " + m + " nodes to graph");
                        return;
                    }
                    NodeUI n=CreateNewNode(e.Location);
                    _graph.AddNode(n.node);

                }
                else
                if (this.Tool == DrawingTools.Edge || this.Tool == DrawingTools.Eraser)
                {
                    int count = 0;
                    foreach (Edge edge in _graph.edgeCollection)
                    {
                        //Vi tri bat dau cua canh
                        var start = this.Controls[edge.start.Index].Location;
                        start.X += NODE_RADIUS;
                        start.Y += NODE_RADIUS;

                        var end = this.Controls[edge.end.Index].Location;
                        end.X += NODE_RADIUS;
                        end.Y += NODE_RADIUS;
                        if (Edge.Contains(start, end, e.Location))
                        {
                            if (this.Tool == DrawingTools.Edge)
                            {
                                _graph.edgeCollection.SelectedIndex = count;
                                Edge edgetemp = _graph.edgeCollection[count];
                                OnDrawEvent(edgetemp, null);
                            }
                            else if (this.Tool == DrawingTools.Eraser)
                            {
                                _graph.edgeCollection.RemoveAt(count);
                                _graph.edgeCollection.SelectedIndex = -1;
                            }
                            this.Invalidate();

                            break;
                        }
                        count++;
                    }
                }
                else if (this.Tool == DrawingTools.Eraser) // delete edge
                {

                }
            }
            //OnContentChanged(null, null);
            base.OnMouseDown(e);
        }
        protected virtual void OnSeletedNodeChanged(object sender, EventArgs e)
        {
            if (SelectedNodeChanged != null)
                SelectedNodeChanged(sender, null);
        }
        protected virtual void OnDrawEvent(object sender, EventArgs e)
        {
            if (DrawEvent != null)
                DrawEvent(sender, null);
        }
        public void ClearEdges()
        {
            _graph.ClearEdge();
            Invalidate();
        }
        public void Clear()
        {
            _graph.Clear();
            this.Controls.Clear();
            Invalidate();
        }
        public void Reset()
        {
            _graph.Reset();
            Invalidate();
        }

        #region mouse_event
        void Node_MouseDown(object sender, MouseEventArgs e)
        {
            NodeUI ctl = (NodeUI)sender;
            if (e.Button == MouseButtons.Left)
            {
                if (_selectedIndex >= 0)
                {
                    NodeUI node = this.Controls[_selectedIndex] as NodeUI;
                    node.Selected = false;
                    node.Invalidate();
                }

                this._selectedIndex = ctl.Index;
                OnSeletedNodeChanged(sender, null);

                ctl.Selected = true;
                ctl.Invalidate();

                if (this.Tool == DrawingTools.Select || this.Tool == DrawingTools.Node)
                    _p = e.Location;
                else if (this.Tool == DrawingTools.Edge)
                {
                    _p = this.PointToClient((ctl.PointToScreen(e.Location)));
                    _startNode = ctl.node;
                    _startPoint = ctl.Location;
                }
                else if (this.Tool == DrawingTools.Eraser)
                {
                    DeleteSelectedNode();
                }
            }
        }
        void Node_MouseMove(object sender, MouseEventArgs e)
        {
            Control ctl = (Control)sender;

            if (e.Button == MouseButtons.Left)
            {

                Point p = this.PointToClient(ctl.PointToScreen(e.Location));
                if (this.Tool == DrawingTools.Select || this.Tool == DrawingTools.Node)
                {
                    if (p.X > 0 && p.Y > 0 && p.X < this.Width && p.Y < this.Height)
                    {
                        p.X -= _p.X;
                        p.Y -= _p.Y;

                        ctl.Location = p;

                        Invalidate();
                    }
                }

                else if (this.Tool == DrawingTools.Edge)
                {
                    Point p2 = this.PointToClient(ctl.PointToScreen(e.Location));
                    using (Graphics g = this.CreateGraphics())
                    {
                        g.DrawLine(Pens.Red, _p, p2);
                        Invalidate();
                    }
                }
            }
        }
        void Node_MouseUp(object sender, MouseEventArgs e)
        {
            NodeUI ctl = (NodeUI)sender;
            if (e.Button == MouseButtons.Left)
            {
                if (this.Tool == DrawingTools.Edge)
                {
                    Point p2 = this.PointToClient(ctl.PointToScreen(e.Location));
                    NodeUI node = this.GetChildAtPoint(p2) as NodeUI;
                    if (node != null)
                    {
                        Edge edge = new Edge(_startNode, node.node);
                        OnDrawEvent(edge, null);
                        if (edge.weight != 0)
                            _graph.AddEdge(edge);
                    }
                }
                Invalidate();
            }
        }
        #endregion
        public void DeleteLastestEdge()
        {
            if (_graph.edgeCollection.Count > 0)
            {
                DeleteEdgeAt(_graph.edgeCollection.Count - 1);
            }
        }
        public void DeleteSelectedNode()
        {
            if (_selectedIndex < 0)
                return;

            NodeUI n = this.Controls[_selectedIndex] as NodeUI;
            n.DoRemovingAnimation();
            this.Controls.RemoveAt(_selectedIndex);
            _graph.DeleteNode(n.node);
            RefreshSubControls();
            Invalidate();
        }
        private void DeleteEdgeAt(int index)
        {
            _graph.edgeCollection[index].IsRemoving = true;
            Refresh();
            _graph.DeleteEdeg(_graph.edgeCollection[index]);
            Invalidate();
        }
        public void DeleteEdge(Edge edge)
        {
            edge.IsRemoving = true;
            _graph.DeleteEdeg(edge);
            Invalidate();
        }
        void RefreshSubControls()
        {
            this._selectedIndex = -1;
            for (int i = 0; i < this.Controls.Count; i++)
            {
                NodeUI node = this.Controls[i] as NodeUI;
                node.Index = i;
                node.DisplayName = (char)('A' + i);
                node.Invalidate();
            }
            OnSeletedNodeChanged(null, null);
        }
        private NodeUI CreateNewNode(Point location)
        {
            NodeUI n = new NodeUI();
            n.Index = this.Controls.Count;
            n.DisplayName = (char)(n.Index + 'A');
            n.Location = location;
            this.Controls.Add(n);
            n.DoCreatingAnimation();
            n.Width = NODE_DIAMETER;
            n.Height = NODE_DIAMETER;
            n.MouseDown += new MouseEventHandler(Node_MouseDown);
            n.MouseMove += new MouseEventHandler(Node_MouseMove);
            n.MouseUp += new MouseEventHandler(Node_MouseUp);
            return n;
        }
        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int count = 0;
            foreach (var item in _graph.edgeCollection)
            {
                Control ctl1 = this.Controls[item.start.Index];
                Control ctl2 = this.Controls[item.end.Index];
                PointF p1 = ctl1.Location;
                PointF p2 = ctl2.Location;

                DrawEdge(g, item, p1, p2);
                count++;
            }

            g.DrawRectangle(Pens.Black, 0, 0, this.Width - 1, this.Height - 1);

            base.OnPaint(e);
        }
        void DrawEdge(Graphics g, Edge item, PointF p1, PointF p2)
        {
            string distance = item.weight.ToString();

            p1.X += NODE_RADIUS;
            p1.Y += NODE_RADIUS;
            p2.X += NODE_RADIUS;
            p2.Y += NODE_RADIUS;

            Vector2D v1 = new Vector2D(p1.X - p2.X, p1.Y - p2.Y);
            if (v1.Length - NODE_RADIUS > 0)
            {
                v1.Contract(NODE_RADIUS);
                p1.X = p2.X + v1.X;
                p1.Y = p2.Y + v1.Y;
            }
            Vector2D v = new Vector2D(p2.X - p1.X, p2.Y - p1.Y);
            if (v.Length - NODE_RADIUS > 0)
            {
                v.Contract(NODE_RADIUS);
                p2.X = p1.X + v.X;
                p2.Y = p1.Y + v.Y;
            }
            if (!IsUndirectedGraph && item.IsUndirected)
            {
                _penEdge.StartCap = LineCap.ArrowAnchor;
            }
            else
                _penEdge.StartCap = LineCap.NoAnchor;

            if (item.IsRemoving)
            {
                Pen p = new Pen(Color.Red, 4);
                g.DrawLine(p, p1, p2);

            }
            else
            {
                if (_list != null && (_list.Contains(item)))
                {
                    if (item.IsSelected)
                    {
                        var p = (Pen)_penEdge.Clone();
                        p.Color = Color.Red;
                        p.DashStyle = DashStyle.Dash;
                        g.DrawLine(p, p1, p2);
                    }
                    else
                    {

                        Pen p = (Pen)_penEdge.Clone();
                        p.Color = Color.Green;
                        p.DashStyle = DashStyle.Dash;
                        g.DrawLine(p, p1, p2);
                    }
                }
                else
                if (item.IsSelected)
                {
                    var hPen = (Pen)_penEdge.Clone();
                    hPen.Color = Color.Red;
                    g.DrawLine(hPen, p1, p2);
                }
                else
                    g.DrawLine(_penEdge, p1, p2);
            }


            // draw distance
            SizeF size = g.MeasureString(distance, this.Font);
            PointF pp = p1;
            pp.X += p2.X;
            pp.Y += p2.Y;
            pp.X /= 2;
            pp.Y /= 2;
            pp.X -= size.Width / 2;
            pp.Y -= size.Height / 2;
            g.FillEllipse(Brushes.Yellow, pp.X - 5, pp.Y - 5, size.Width + 10, size.Height + 5);
            g.DrawString(distance.ToString(), this.Font, Brushes.Blue, pp);
        }
        #region Save/Load

        public void SaveGraph(string filematrix, string filePoint)
        {

            GraphData data = new GraphData();
            data.IsUndirectedGraph = IsUndirectedGraph;

            for (int i = 0; i < this.Controls.Count; i++)
            {
                Point p = this.Controls[i].Location;
                p.Offset(NODE_RADIUS, NODE_RADIUS);

                data.NodeLocations.Add(p);
            }
            data.graph = _graph;
            try
            {
                data.SaveData(filematrix, filePoint);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        public GraphData LoadGraph(string filematrix, string filePoint)
        {
            try
            {
                GraphData data = new GraphData();
                data.LoadData(filematrix, filePoint);
                _graph = data.graph;
                _graph.GraphChange += _graph_GraphChange;
                _graph.IsUndirected = data.IsUndirectedGraph;
                Point point;
                for (int i = 0; i < data.graph.nodeCollection.Count; i++)
                {
                    if (data.graph.nodeCollection.Count <= data.NodeLocations.Count)
                        point = data.NodeLocations[i];
                    else
                        point = GetRandomLocaition();
                    NodeUI n = CreateNewNode(point);
                    n.node = data.graph.nodeCollection[i];
                }
                return data;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return null;
        }

        #endregion
        Point GetRandomLocaition()
        {
            Random random = new Random();
            Point point = new Point();
            point.X = random.Next(NODE_RADIUS, this.Width - NODE_RADIUS);
            point.Y = random.Next(NODE_RADIUS, this.Height - NODE_RADIUS);
            return point;
        }
        public void Kruskal()
        {
            _list = _graph.Kruskal();
            Invalidate();
        }
        public void Prim(Node node)
        {
            _list = _graph.Prim(node);
            Invalidate();
        }
        public void BFS()
        {
            _graph.BFS(_graph.nodeCollection[0]);
            Invalidate();
        }
        public bool IsLienthong
        {
            get
            {
                return _graph.ISLienthong();
            }
        }
        public bool IsHaiphia
        {
            get
            {
                return !(_graph.IsHaiPhia() == null);
            }
        }

        public List<NodeCollection> Lienthong()
        {
            return _graph.Lienthong();
        }
        public void CreateGraphRandom(int n)
        {
            if (n > 0)
            {
                this.Controls.Clear();
                _graph.CreatGraphRandom(n);
                foreach (var item in _graph.nodeCollection)
                {
                    Point point = GetRandomLocaition();
                    NodeUI node = CreateNewNode(point);
                    node.node = item;
                }
                Invalidate();
            }
        }
        public List<Node> Euler()
        {
            Node node;
            if (SelectedNode == null)
                node = _graph.nodeCollection[0];
            else
                node = SelectedNode.node;
            Invalidate();
            return _graph.Euler(node);
        }
    }
}
