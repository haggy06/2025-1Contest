public interface IDragable
{
    public bool Active { get; set; }

    public void DragStart();
    public void Dragging();
    public void DragEnd();
}
