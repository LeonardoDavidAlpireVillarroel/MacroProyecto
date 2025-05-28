public class Connection
{
    public Node from;
    public Node to;
    public float cost;

    public Connection(Node from, Node to, float cost)
    {
        this.from = from;
        this.to = to;
        this.cost = cost;
    }
}
