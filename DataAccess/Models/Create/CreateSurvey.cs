using System.Linq;

namespace DataAccess.Models.Create
{
    public class CreateSurvey
    {
        public int eventId { get; set; }
        public string authorId{ get; set; }
        public SurveyQuestion[] questions { get; set; }

        public bool ParameterValid()
        {
            return questions!=null && questions.Count()!=0 && authorId !=null;
        }
        
        public class SurveyQuestion
        {
            public const string open_text = "open_text";
            public const string multiple_choices = "multiple_choice";
            public const string single_choices = "single_choice";

            public string question { get; set; }
            public string type { get; set; }
            public string[] choices_messages { get; set; }

            public bool ParameterValid()
            {
                return type != null && question != null;
            }
            
        }
    }
}
