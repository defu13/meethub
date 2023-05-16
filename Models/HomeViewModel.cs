

namespace mvc_pruebas.Models;
public partial class HomeViewModel
{
    public Event NewEvent { get; set; }
    public List<Event> EventList { get; set; }
    public List<Assistant> AssistantList { get; set; }
}