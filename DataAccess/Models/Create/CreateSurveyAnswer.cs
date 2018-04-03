using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Create
{
    public class CreateSurveyAnswer
    {
        public int eventId { get; set; }
        public string authorId { get; set; }
        public SurveyQuestionAnswer[] questions { get; set; }       

        public class SurveyQuestionAnswer
        {
            public int questionId { get; set; }
            public int[] choices { get; set; }
            public string text { get; set; }
            
        }

        public bool ParameterValid()
        {
            return eventId != 0 && authorId != null && questions != null && questions.Count() > 0;
        }
    }
}
