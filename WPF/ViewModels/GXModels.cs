using System;
using GraphX.Common.Models;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using QuickGraph;
using GraphX.Controls;

namespace Graph_Database_WPF 
{

    public class DataVertex : VertexBase
    {
        public string Text { get; set; }
        public override string ToString()
        {
            return Text;
        }
    }
    public class DataEdge : EdgeBase<DataVertex>
    {
        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
        }
        public string Text { get; set; }
    }
    public class DataGraph : BidirectionalGraph<DataVertex, DataEdge> { }
    public class GXLogicCore : GraphX.Logic.Models.GXLogicCore<DataVertex, DataEdge, DataGraph> { }
    public class MyGraphArea : GraphArea<DataVertex, DataEdge, DataGraph> { }
}
