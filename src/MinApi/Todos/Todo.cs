namespace MinApi.Todos
{
    public class Todo
    {
        public long Id { get; init; }
        public string Title { get; init; } = null!;
        public bool Completed { get; init; }
        public DateTime CreatedOn { get; init; }
        public DateTime? CompletedOn { get; init; }
    }
}
