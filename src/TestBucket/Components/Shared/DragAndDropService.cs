namespace TestBucket.Components.Shared;

public class DragAndDropService<T>
{
    public T? Data { get; private set; }

    /// <summary>
    /// Returns true if there is data of the specified type
    /// </summary>
    public bool HasData => Data is not null;

    public void StartDrag(T data)
    {
        this.Data = data;
    }
    public void StopDrag()
    {
        this.Data = default(T);
    }

}