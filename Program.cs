using System;

class Node<T>
{
    public T Value { get; set; }
    public Node(T value)
    {
        Value = value;
    }
}
class Edge<T>
{
    public Node<T> From { get; set; }
    public Node<T> To { get; set; }
    public Edge(Node<T> from, Node<T> to)
    {
        From = from;
        To = to;
    }
}
class Graph<T>
{
    public void AddNode(Node<T> node) 
    {
        // Implementation here
    }
}
