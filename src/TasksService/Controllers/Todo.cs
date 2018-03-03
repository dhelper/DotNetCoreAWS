using System;

namespace TasksService.Controllers
{
    public class Todo
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
    
    public class TodoWithId
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}