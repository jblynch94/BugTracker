using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class AssignUsersViewModel
    {
            public Project? Project { get; set; }
            public MultiSelectList? MemberList { get; set; }
            public List<string>? Members { get; set; }
    }

}

