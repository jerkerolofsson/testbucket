namespace TestBucket.Components.Shared;

public class DragAndDropService<T>
{
    public T? Data { get; set; }

    public void StartDrag(T data)
    {
        this.Data = data;
    }

}