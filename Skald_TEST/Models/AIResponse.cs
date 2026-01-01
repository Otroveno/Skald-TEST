
using System.Collections.Generic;

namespace AIInfluence.Models
{
    public class AIResponse
    {
        public string DialogueText { get; set; }
        public List<string> Tags { get; set; }
        public bool IsImportant { get; set; }
    }
}
