namespace VectorLibrary
{
    internal class Node
    {
        public int Index { get; set; }
        public double Value { get; set; }
        public Node Next { get; set; }
        public Node(int index, double value, Node next = null)
        { 
            Index = index;
            Value = value;
            Next = next;
        }
        public override string ToString()
        {
            return $"[{Index}] = {Value}";
        }
    }
}